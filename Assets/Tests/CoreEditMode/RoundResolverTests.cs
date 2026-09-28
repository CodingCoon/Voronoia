using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using VoronationCore;
using VoronationGeometry;

public class RoundResolverTests
{
    [Test]
    public void SameStateCommandsAndSeedProduceSameResult()
    {
        var resolver = new RoundResolver(new LegacyVoronoiCellCalculator());
        MatchState state = InitialState(resolver);
        KnightId knight = state.Factions[0].Knights[0].Id;
        var plan = new RoundPlan(state.RoundNumber);
        plan.AddOrReplace(new ActionCommand(knight.FactionId, knight, ActionKind.ImprovePower));

        RoundResult first = resolver.Resolve(state, plan);
        RoundResult second = resolver.Resolve(state, plan);

        Assert.That(Fingerprint(first), Is.EqualTo(Fingerprint(second)));
        Assert.That(state.RoundNumber, Is.EqualTo(0));
        Assert.That(state.FindKnight(knight).Power, Is.EqualTo(1f));
    }

    [Test]
    public void SplitChildGetsIncomeButNoTurnOrFirstMaintenance()
    {
        var resolver = new RoundResolver(new LegacyVoronoiCellCalculator());
        MatchState state = InitialState(resolver);
        KnightState parent = state.Factions[0].Knights[0];
        var plan = new RoundPlan(0);
        plan.AddOrReplace(new ActionCommand(parent.Id.FactionId, parent.Id, ActionKind.Split, new Vector2(-2, 0)));

        RoundResult result = resolver.Resolve(state, plan);
        FactionState faction = result.After.FindFaction(parent.Id.FactionId);
        KnightState child = faction.Knights.Single(knight => knight.Id != parent.Id);

        Assert.That(child.Age, Is.EqualTo(0));
        Assert.That(result.Commands.Any(command => command.KnightId == child.Id), Is.False);
        Assert.That(result.Bookings.Single(booking => booking.KnightId == child.Id &&
            booking.Kind == BookingKind.Maintenance).Amount, Is.Zero);
        Assert.That(result.Bookings.Single(booking => booking.KnightId == child.Id &&
            booking.Kind == BookingKind.Income).Amount, Is.GreaterThan(0));
    }

    [Test]
    public void TargetMustStayInsideCellWithBoundaryMargin()
    {
        var resolver = new RoundResolver(new LegacyVoronoiCellCalculator());
        MatchState state = InitialState(resolver);
        KnightState knight = state.Factions[0].Knights[0];

        CommandValidation boundary = resolver.ValidateCommand(state, 0,
            new ActionCommand(knight.Id.FactionId, knight.Id, ActionKind.Move, Vector2.zero));
        CommandValidation interior = resolver.ValidateCommand(state, 0,
            new ActionCommand(knight.Id.FactionId, knight.Id, ActionKind.Move, new Vector2(-2, 0)));
        CommandValidation splitAtParent = resolver.ValidateCommand(state, 0,
            new ActionCommand(knight.Id.FactionId, knight.Id, ActionKind.Split, knight.Position));

        Assert.That(boundary.Reason, Is.EqualTo(CommandRejection.TargetTooCloseToBoundary));
        Assert.That(interior.IsValid, Is.True);
        Assert.That(splitAtParent.Reason, Is.EqualTo(CommandRejection.SplitTooCloseToParent));
    }

    [Test]
    public void FailureDuringSecondCellCalculationDoesNotChangeSource()
    {
        var calculator = new FailOnCallCalculator(2);
        var resolver = new RoundResolver(calculator);
        FactionId humanId = new FactionId(1);
        FactionId aiId = new FactionId(2);
        var state = new MatchState(0, 9, 10, humanId, MatchOutcome.Running, new[]
        {
            new FactionState(humanId, "Human", Color.green, true, -1000, 3, new[]
            {
                new KnightState(new KnightId(humanId, 1), new Vector2(-5, -2), age: 3),
                new KnightState(new KnightId(humanId, 2), new Vector2(-5, 2), age: 1)
            }),
            new FactionState(aiId, "AI", Color.red, false, 100, 2, new[]
            {
                new KnightState(new KnightId(aiId, 1), new Vector2(5, 0))
            })
        });

        Assert.Throws<RoundResolutionException>(() => resolver.Resolve(state, new RoundPlan(0)));
        Assert.That(state.Factions[0].Money, Is.EqualTo(-1000));
        Assert.That(state.Factions[0].Knights.Count, Is.EqualTo(2));
        Assert.That(state.RoundNumber, Is.Zero);
    }

    [Test]
    public void NegativeBalancesRemoveOneKnightEachAndAllRemovedIsDraw()
    {
        var resolver = new RoundResolver(new LegacyVoronoiCellCalculator());
        FactionId humanId = new FactionId(1);
        FactionId aiId = new FactionId(2);
        var state = new MatchState(0, 10, 10, humanId, MatchOutcome.Running, new[]
        {
            new FactionState(humanId, "Human", Color.green, true, -1000, 2,
                new[] { new KnightState(new KnightId(humanId, 1), new Vector2(-3, 0)) }),
            new FactionState(aiId, "AI", Color.red, false, -1000, 2,
                new[] { new KnightState(new KnightId(aiId, 1), new Vector2(3, 0)) })
        });

        RoundResult result = resolver.Resolve(state, new RoundPlan(0));

        Assert.That(result.After.Factions, Is.Empty);
        Assert.That(result.After.Outcome, Is.EqualTo(MatchOutcome.Draw));
        Assert.That(result.Bookings.Count(booking => booking.Kind == BookingKind.DebtRelief), Is.EqualTo(2));
        Assert.That(result.Events.Count(item => item.Kind == RoundEventKind.KnightRemoved), Is.EqualTo(2));
    }

    [Test]
    public void SessionRejectsDuplicateResolveAndOldPlan()
    {
        var resolver = new RoundResolver(new LegacyVoronoiCellCalculator());
        MatchState state = InitialState(resolver);
        var session = new MatchSession(state, resolver);
        session.Resolve();

        Assert.Throws<InvalidOperationException>(() => session.Resolve());
        session.PresentationCompleted();
        session.Continue();
        Assert.That(session.Plan.StateVersion, Is.EqualTo(1));
        KnightId knight = session.State.Factions[0].Knights[0].Id;
        CommandValidation stale = resolver.ValidateCommand(session.State, 0,
            new ActionCommand(knight.FactionId, knight, ActionKind.None));
        Assert.That(stale.Reason, Is.EqualTo(CommandRejection.WrongRound));
    }

    [Test]
    public void AccountingRemovesExactlyOldestKnightAndPreservesNextId()
    {
        var resolver = new RoundResolver(new FailOnCallCalculator(-1));
        FactionId humanId = new FactionId(1);
        FactionId aiId = new FactionId(2);
        KnightId olderLowId = new KnightId(humanId, 1);
        var state = new MatchState(0, 20, 10, humanId, MatchOutcome.Running, new[]
        {
            new FactionState(humanId, "Human", Color.green, true, -1000, 3, new[]
            {
                new KnightState(olderLowId, new Vector2(-5, -2), age: 5),
                new KnightState(new KnightId(humanId, 2), new Vector2(-5, 2), age: 5)
            }),
            new FactionState(aiId, "AI", Color.red, false, 1000, 2,
                new[] { new KnightState(new KnightId(aiId, 1), new Vector2(5, 0)) })
        });

        RoundResult result = resolver.Resolve(state, new RoundPlan(0));
        FactionState human = result.After.FindFaction(humanId);

        Assert.That(human.Knights.Count, Is.EqualTo(1));
        Assert.That(human.FindKnight(olderLowId), Is.Null);
        Assert.That(human.Money, Is.Zero);
        Assert.That(human.NextKnightNumber, Is.EqualTo(3));
        RoundBooking relief = result.Bookings.Single(booking => booking.Kind == BookingKind.DebtRelief &&
            booking.FactionId == humanId);
        Assert.That(relief.Amount, Is.EqualTo(-result.FactionSummaries.Single(summary =>
            summary.FactionId == humanId).MoneyAfterAccounting));
    }

    [Test]
    public void FailureOnFirstCalculationPreservesRoundAndIdCounter()
    {
        var resolver = new RoundResolver(new FailOnCallCalculator(1));
        FactionId humanId = new FactionId(1);
        var state = new MatchState(0, 30, 10, humanId, MatchOutcome.Running, new[]
        {
            new FactionState(humanId, "Human", Color.green, true, 100, 2,
                new[] { new KnightState(new KnightId(humanId, 1), Vector2.zero) })
        });

        Assert.Throws<RoundResolutionException>(() => resolver.Resolve(state, new RoundPlan(0)));
        Assert.That(state.RoundNumber, Is.Zero);
        Assert.That(state.Factions[0].NextKnightNumber, Is.EqualTo(2));
        Assert.That(state.Factions[0].Knights.Count, Is.EqualTo(1));
    }

    [Test]
    public void StateCanonicalizesFactionAndKnightOrder()
    {
        FactionId firstId = new FactionId(1);
        FactionId secondId = new FactionId(2);
        var state = new MatchState(0, 1, 10, firstId, MatchOutcome.Running, new[]
        {
            new FactionState(secondId, "Second", Color.red, false, 0, 2,
                new[] { new KnightState(new KnightId(secondId, 1), Vector2.one) }),
            new FactionState(firstId, "First", Color.green, true, 0, 3, new[]
            {
                new KnightState(new KnightId(firstId, 2), Vector2.right),
                new KnightState(new KnightId(firstId, 1), Vector2.left)
            })
        });

        Assert.That(state.Factions.Select(faction => faction.Id.Value), Is.EqualTo(new[] { 1, 2 }));
        Assert.That(state.Factions[0].Knights.Select(knight => knight.Id.Number), Is.EqualTo(new[] { 1, 2 }));
    }

    [Test]
    public void ConflictingPlannedEndPositionRejectsOnlyNewCommand()
    {
        FactionId factionId = new FactionId(1);
        KnightId firstId = new KnightId(factionId, 1);
        KnightId secondId = new KnightId(factionId, 2);
        var sharedCell = new[]
        {
            new Vector2(-5, -5), new Vector2(-5, 5), new Vector2(5, 5), new Vector2(5, -5)
        };
        var state = new MatchState(0, 1, 10, factionId, MatchOutcome.Running, new[]
        {
            new FactionState(factionId, "Human", Color.green, true, 100, 3, new[]
            {
                new KnightState(firstId, new Vector2(-2, 0), cell: sharedCell, area: 100),
                new KnightState(secondId, new Vector2(2, 0), cell: sharedCell, area: 100)
            })
        });
        var session = new MatchSession(state, new RoundResolver(new FailOnCallCalculator(-1)));

        Assert.That(session.PlanAction(new ActionCommand(factionId, firstId, ActionKind.Move, Vector2.zero)).IsValid,
            Is.True);
        CommandValidation conflict = session.PlanAction(
            new ActionCommand(factionId, secondId, ActionKind.Move, Vector2.zero));

        Assert.That(conflict.Reason, Is.EqualTo(CommandRejection.ConflictingEndPosition));
        Assert.That(session.Plan.Commands.Count, Is.EqualTo(1));
        Assert.That(session.Phase, Is.EqualTo(SessionPhase.Planning));
    }

    private static MatchState InitialState(RoundResolver resolver)
    {
        FactionId humanId = new FactionId(1);
        FactionId aiId = new FactionId(2);
        var state = new MatchState(0, 1234, 10, humanId, MatchOutcome.Running, new[]
        {
            new FactionState(humanId, "Human", Color.green, true, 100, 2,
                new[] { new KnightState(new KnightId(humanId, 1), new Vector2(-3, 0)) }),
            new FactionState(aiId, "AI", Color.red, false, 100, 2,
                new[] { new KnightState(new KnightId(aiId, 1), new Vector2(3, 0)) })
        });
        return resolver.CalculateInitialCells(state);
    }

    private static string Fingerprint(RoundResult result)
    {
        return string.Join("|", result.After.Factions.SelectMany(faction => faction.Knights)
            .OrderBy(knight => knight.Id)
            .Select(knight => knight.Id + ":" + knight.Position + ":" + knight.Power + ":" + knight.Age)) +
            "/" + string.Join("|", result.Bookings.Select(booking => booking.FactionId + ":" +
                booking.KnightId + ":" + booking.Kind + ":" + booking.Amount)) +
            "/" + string.Join("|", result.Events.Select(item => item.Kind + ":" + item.KnightId));
    }

    private sealed class FailOnCallCalculator : ICellCalculator
    {
        private readonly int failureCall;
        private int calls;
        public FailOnCallCalculator(int failureCall) { this.failureCall = failureCall; }

        public IReadOnlyList<CalculatedCell> Calculate(float halfMapSize, IReadOnlyList<CellSite> sites)
        {
            calls++;
            if (calls == failureCall) throw new InvalidOperationException("Injected failure");
            var result = new List<CalculatedCell>();
            for (int i = 0; i < sites.Count; i++)
            {
                float x = -9 + i * 2;
                var points = new[]
                {
                    new Vector2(x, -1), new Vector2(x, 1), new Vector2(x + 1, 1), new Vector2(x + 1, -1)
                };
                result.Add(new CalculatedCell(sites[i].KnightId, points, 2));
            }
            return result;
        }
    }
}
