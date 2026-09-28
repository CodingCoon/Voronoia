using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VoronationCore
{
    public sealed class RoundResolutionException : Exception
    {
        public CommandRejection Rejection { get; }
        public RoundResolutionException(string message, CommandRejection rejection = CommandRejection.None,
            Exception innerException = null) : base(message, innerException) { Rejection = rejection; }
    }

    public sealed class RoundResolver
    {
        public const float TargetMargin = 0.05f;
        public const float PositionConflictDistance = 0.05f;
        private const float MovePricePerDistance = 10f;
        private const float SplitPrice = 25f;
        private const float UpgradePriceFactor = 40f;
        private const float UpgradeIncrease = 0.1f;
        private const float MaintenancePerAge = 40f;

        private readonly ICellCalculator cellCalculator;
        private readonly IRandomSourceFactory randomFactory;

        public RoundResolver(ICellCalculator cellCalculator, IRandomSourceFactory randomFactory = null)
        {
            this.cellCalculator = cellCalculator ?? throw new ArgumentNullException(nameof(cellCalculator));
            this.randomFactory = randomFactory ?? new DeterministicRandomSourceFactory();
        }

        public CommandValidation ValidateCommand(MatchState state, int planVersion, ActionCommand command)
        {
            if (state == null || command == null) return CommandValidation.Rejected(CommandRejection.InvalidAction);
            if (state.RoundNumber != planVersion) return CommandValidation.Rejected(CommandRejection.WrongRound, command.KnightId);
            if (state.Outcome != MatchOutcome.Running) return CommandValidation.Rejected(CommandRejection.MatchEnded, command.KnightId);
            FactionState faction = state.FindFaction(command.OwnerId);
            if (faction == null) return CommandValidation.Rejected(CommandRejection.UnknownFaction, command.KnightId);
            KnightState knight = state.FindKnight(command.KnightId);
            if (knight == null) return CommandValidation.Rejected(CommandRejection.UnknownKnight, command.KnightId);
            if (command.KnightId.FactionId != command.OwnerId) return CommandValidation.Rejected(CommandRejection.WrongOwner, command.KnightId);
            bool needsTarget = command.Kind == ActionKind.Move || command.Kind == ActionKind.Split;
            if (needsTarget && !command.Target.HasValue) return CommandValidation.Rejected(CommandRejection.MissingTarget, command.KnightId);
            if (!needsTarget && command.Target.HasValue) return CommandValidation.Rejected(CommandRejection.UnexpectedTarget, command.KnightId);
            if (!Enum.IsDefined(typeof(ActionKind), command.Kind)) return CommandValidation.Rejected(CommandRejection.InvalidAction, command.KnightId);
            if (!needsTarget) return CommandValidation.Valid();
            Vector2 target = command.Target.Value;
            if (!CoreGeometry.IsFinite(target)) return CommandValidation.Rejected(CommandRejection.NonFiniteTarget, command.KnightId);
            if (!CoreGeometry.Contains(knight.Cell, target)) return CommandValidation.Rejected(CommandRejection.TargetOutsideOwnCell, command.KnightId);
            if (CoreGeometry.BoundaryDistance(knight.Cell, target) < TargetMargin)
                return CommandValidation.Rejected(CommandRejection.TargetTooCloseToBoundary, command.KnightId);
            if (command.Kind == ActionKind.Split && Vector2.Distance(knight.Position, target) < TargetMargin)
                return CommandValidation.Rejected(CommandRejection.SplitTooCloseToParent, command.KnightId);
            return CommandValidation.Valid();
        }

        public CommandValidation ValidateProjectedPositions(MatchState state, RoundPlan plan, KnightId candidateId)
        {
            var positions = new List<(KnightId SourceId, Vector2 Position)>();
            foreach (FactionState faction in state.Factions)
            {
                foreach (KnightState knight in faction.Knights)
                {
                    ActionCommand command = plan.Find(knight.Id);
                    Vector2 position = command != null && command.Kind == ActionKind.Move
                        ? command.Target.Value
                        : knight.Position;
                    positions.Add((knight.Id, position));
                    if (command != null && command.Kind == ActionKind.Split)
                        positions.Add((knight.Id, command.Target.Value));
                }
            }
            for (int i = 0; i < positions.Count; i++)
                for (int j = 0; j < i; j++)
                    if (Vector2.Distance(positions[i].Position, positions[j].Position) < PositionConflictDistance)
                        return CommandValidation.Rejected(CommandRejection.ConflictingEndPosition, candidateId);
            return CommandValidation.Valid();
        }

        public RoundResult Resolve(MatchState source, RoundPlan plan)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (plan == null) throw new ArgumentNullException(nameof(plan));
            if (source.RoundNumber != plan.StateVersion)
                throw new RoundResolutionException("Plan belongs to a different round.", CommandRejection.WrongRound);
            if (source.Outcome != MatchOutcome.Running)
                throw new RoundResolutionException("Cannot resolve an ended match.", CommandRejection.MatchEnded);

            MatchState before = source.DeepCopy();
            MatchState working = source.DeepCopy();
            RoundPlan frozenPlan = plan.Snapshot();
            var plannedByKnight = new Dictionary<KnightId, ActionCommand>();
            foreach (ActionCommand command in frozenPlan.Commands)
            {
                CommandValidation validation = ValidateCommand(before, frozenPlan.StateVersion, command);
                if (!validation.IsValid)
                    throw new RoundResolutionException("Rejected command: " + validation.Reason, validation.Reason);
                if (!plannedByKnight.TryAdd(command.KnightId, command))
                    throw new RoundResolutionException("A knight has more than one command.", CommandRejection.InvalidAction);
            }

            // Creating a fresh source is part of the deterministic round contract. No rule currently consumes randomness.
            randomFactory.Create(before.Seed, before.RoundNumber);
            var originalIds = before.Factions.SelectMany(faction => faction.Knights).Select(knight => knight.Id)
                .OrderBy(id => id).ToList();
            var accepted = new List<ActionCommand>(originalIds.Count);
            var bookings = new List<RoundBooking>();
            var events = new List<RoundEvent>();
            var actionCosts = new Dictionary<KnightId, float>();

            foreach (KnightId id in originalIds)
            {
                ActionCommand command = plannedByKnight.TryGetValue(id, out ActionCommand planned)
                    ? planned
                    : new ActionCommand(id.FactionId, id, ActionKind.None);
                accepted.Add(command);
                ApplyCommand(working, command, actionCosts, events);
            }

            EnsureDistinctPositions(working);
            IReadOnlyList<CalculatedCell> accountingCells = CalculateAndAssignCells(working);
            var accountingMoney = new Dictionary<FactionId, float>();
            foreach (FactionState faction in working.Factions.OrderBy(item => item.Id))
            {
                float total = faction.Money;
                foreach (KnightState knight in faction.Knights.OrderBy(item => item.Id))
                {
                    float actionCost = actionCosts.TryGetValue(knight.Id, out float cost) ? cost : 0f;
                    if (actionCost != 0f)
                        bookings.Add(new RoundBooking(faction.Id, knight.Id, BookingKind.ActionCost, actionCost, "Aktion"));
                    float income = knight.Area * knight.IncomeFactor;
                    float maintenance = -MaintenancePerAge * knight.Age;
                    ValidateFinite(actionCost, "action cost");
                    ValidateFinite(income, "income");
                    ValidateFinite(maintenance, "maintenance");
                    bookings.Add(new RoundBooking(faction.Id, knight.Id, BookingKind.Income, income, "Einnahmen"));
                    bookings.Add(new RoundBooking(faction.Id, knight.Id, BookingKind.Maintenance, maintenance, "Unterhalt"));
                    total += actionCost + income + maintenance;
                }
                ValidateFinite(total, "account balance");
                faction.Money = total;
                accountingMoney.Add(faction.Id, total);
            }

            bool removedAny = false;
            var factionIds = working.Factions.Select(faction => faction.Id).OrderBy(id => id).ToList();
            foreach (FactionId factionId in factionIds)
            {
                FactionState faction = working.FindFaction(factionId);
                if (faction == null || faction.Money >= 0 || faction.Knights.Count == 0) continue;
                KnightState removed = faction.Knights.OrderByDescending(knight => knight.Age)
                    .ThenBy(knight => knight.Id.Number).First();
                float relief = -faction.Money;
                faction.RemoveKnight(removed.Id);
                faction.Money = 0;
                bookings.Add(new RoundBooking(faction.Id, removed.Id, BookingKind.DebtRelief, relief, "Schuldenerlass"));
                events.Add(new RoundEvent(RoundEventKind.KnightRemoved, faction.Id, removed.Id));
                removedAny = true;
            }

            foreach (FactionId factionId in factionIds)
            {
                FactionState faction = working.FindFaction(factionId);
                if (faction == null || faction.Knights.Count != 0) continue;
                working.RemoveFaction(factionId);
                events.Add(new RoundEvent(RoundEventKind.FactionRemoved, factionId));
            }

            working.Outcome = DetermineOutcome(working);
            if (removedAny && working.Outcome == MatchOutcome.Running) CalculateAndAssignCells(working);
            working.RoundNumber++;
            events.Add(new RoundEvent(RoundEventKind.RoundResolved));
            if (working.Outcome != MatchOutcome.Running)
                events.Add(new RoundEvent(RoundEventKind.MatchEnded));

            var summaries = new List<FactionRoundSummary>();
            foreach (FactionState oldFaction in before.Factions.OrderBy(faction => faction.Id))
            {
                FactionState finalFaction = working.FindFaction(oldFaction.Id);
                float afterAccounting = accountingMoney[oldFaction.Id];
                float finalMoney = finalFaction?.Money ?? (afterAccounting < 0 ? 0 : afterAccounting);
                summaries.Add(new FactionRoundSummary(oldFaction.Id, oldFaction.Money, afterAccounting, finalMoney));
            }
            var deltas = CreateTerritoryDeltas(before, working, accountingCells);
            return new RoundResult(before, working.DeepCopy(), accepted.AsReadOnly(), accountingCells,
                bookings.AsReadOnly(), events.AsReadOnly(), summaries.AsReadOnly(), deltas.AsReadOnly());
        }

        public MatchState CalculateInitialCells(MatchState source)
        {
            MatchState result = source.DeepCopy();
            CalculateAndAssignCells(result);
            return result;
        }

        private static void ApplyCommand(MatchState state, ActionCommand command,
            IDictionary<KnightId, float> actionCosts, ICollection<RoundEvent> events)
        {
            FactionState faction = state.FindFaction(command.OwnerId);
            KnightState knight = faction.FindKnight(command.KnightId);
            knight.Age++;
            float cost = 0f;
            KnightId? createdId = null;
            switch (command.Kind)
            {
                case ActionKind.None:
                    break;
                case ActionKind.Move:
                    cost = -MovePricePerDistance * Vector2.Distance(knight.Position, command.Target.Value);
                    knight.Position = command.Target.Value;
                    break;
                case ActionKind.ImprovePower:
                    cost = -UpgradePriceFactor * (knight.Power + UpgradeIncrease);
                    knight.Power += UpgradeIncrease;
                    break;
                case ActionKind.ImproveIncome:
                    cost = -UpgradePriceFactor * (knight.IncomeFactor + UpgradeIncrease);
                    knight.IncomeFactor += UpgradeIncrease;
                    break;
                case ActionKind.Split:
                    cost = -SplitPrice;
                    knight.Power -= (knight.Power - 1f) / 2f;
                    knight.IncomeFactor -= (knight.IncomeFactor - 1f) / 2f;
                    createdId = new KnightId(faction.Id, faction.NextKnightNumber++);
                    faction.AddKnight(new KnightState(createdId.Value, command.Target.Value,
                        knight.Power, knight.IncomeFactor));
                    events.Add(new RoundEvent(RoundEventKind.KnightCreated, faction.Id, createdId, knight.Id,
                        ActionKind.Split));
                    break;
                default:
                    throw new RoundResolutionException("Unknown action.", CommandRejection.InvalidAction);
            }
            ValidateFinite(cost, "action cost");
            actionCosts[knight.Id] = cost;
            events.Add(new RoundEvent(RoundEventKind.ActionApplied, faction.Id, knight.Id, createdId, command.Kind));
        }

        private IReadOnlyList<CalculatedCell> CalculateAndAssignCells(MatchState state)
        {
            var sites = state.Factions.SelectMany(faction => faction.Knights)
                .OrderBy(knight => knight.Id)
                .Select(knight => new CellSite(knight.Id, knight.Position, knight.Power)).ToList();
            IReadOnlyList<CalculatedCell> cells;
            try { cells = cellCalculator.Calculate(state.HalfMapSize, sites); }
            catch (Exception exception) { throw new RoundResolutionException("Cell calculation failed.", innerException: exception); }
            if (cells == null || cells.Count != sites.Count)
                throw new RoundResolutionException("Cell calculation returned an incomplete result.");
            var seen = new HashSet<KnightId>();
            foreach (CalculatedCell cell in cells)
            {
                KnightState knight = state.FindKnight(cell.KnightId);
                if (knight == null || !seen.Add(cell.KnightId))
                    throw new RoundResolutionException("Cell calculation returned invalid ownership.");
                if (cell.Points == null || cell.Points.Count < 3 || !CoreGeometry.IsFinite(cell.Area) || cell.Area <= CoreGeometry.Epsilon)
                    throw new RoundResolutionException("Cell calculation returned an unusable cell.");
                foreach (Vector2 point in cell.Points)
                    if (!CoreGeometry.IsFinite(point) || Mathf.Abs(point.x) > state.HalfMapSize + CoreGeometry.Epsilon ||
                        Mathf.Abs(point.y) > state.HalfMapSize + CoreGeometry.Epsilon)
                        throw new RoundResolutionException("Cell calculation returned a point outside the map.");
                knight.SetCell(cell.Points, cell.Area);
            }
            return cells;
        }

        private static void EnsureDistinctPositions(MatchState state)
        {
            var knights = state.Factions.SelectMany(faction => faction.Knights).OrderBy(knight => knight.Id).ToList();
            for (int i = 0; i < knights.Count; i++)
                for (int j = 0; j < i; j++)
                    if (Vector2.Distance(knights[i].Position, knights[j].Position) < PositionConflictDistance)
                        throw new RoundResolutionException("Two knights would occupy the same position.",
                            CommandRejection.ConflictingEndPosition);
        }

        private static MatchOutcome DetermineOutcome(MatchState state)
        {
            bool player = state.Factions.Any(faction => faction.IsPlayer && faction.Knights.Count > 0);
            bool ai = state.Factions.Any(faction => !faction.IsPlayer && faction.Knights.Count > 0);
            if (player) return ai ? MatchOutcome.Running : MatchOutcome.PlayerWon;
            return ai ? MatchOutcome.PlayerLost : MatchOutcome.Draw;
        }

        private static List<TerritoryDelta> CreateTerritoryDeltas(MatchState before, MatchState after,
            IReadOnlyList<CalculatedCell> accountingCells)
        {
            var ids = new HashSet<KnightId>();
            foreach (FactionState faction in before.Factions)
                foreach (KnightState knight in faction.Knights) ids.Add(knight.Id);
            foreach (FactionState faction in after.Factions)
                foreach (KnightState knight in faction.Knights) ids.Add(knight.Id);
            var accounting = accountingCells.ToDictionary(cell => cell.KnightId, cell => cell.Area);
            var result = new List<TerritoryDelta>(ids.Count);
            foreach (KnightId id in ids.OrderBy(item => item))
                result.Add(new TerritoryDelta(id, before.FindKnight(id)?.Area ?? 0,
                    accounting.TryGetValue(id, out float accountingArea) ? accountingArea : 0,
                    after.FindKnight(id)?.Area ?? 0));
            return result;
        }

        private static void ValidateFinite(float value, string label)
        {
            if (!CoreGeometry.IsFinite(value)) throw new RoundResolutionException("Non-finite " + label + ".");
        }
    }
}
