using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace VoronationCore
{
    public enum BookingKind { ActionCost, Income, Maintenance, DebtRelief }

    public sealed class RoundBooking
    {
        public FactionId FactionId { get; }
        public KnightId? KnightId { get; }
        public BookingKind Kind { get; }
        public float Amount { get; }
        public string Label { get; }

        public RoundBooking(FactionId factionId, KnightId? knightId, BookingKind kind, float amount, string label)
        {
            FactionId = factionId;
            KnightId = knightId;
            Kind = kind;
            Amount = amount;
            Label = label;
        }
    }

    public enum RoundEventKind
    {
        ActionApplied,
        KnightCreated,
        KnightRemoved,
        FactionRemoved,
        RoundResolved,
        MatchEnded
    }

    public sealed class RoundEvent
    {
        public RoundEventKind Kind { get; }
        public FactionId? FactionId { get; }
        public KnightId? KnightId { get; }
        public KnightId? RelatedKnightId { get; }
        public ActionKind ActionKind { get; }

        public RoundEvent(RoundEventKind kind, FactionId? factionId = null, KnightId? knightId = null,
            KnightId? relatedKnightId = null, ActionKind actionKind = ActionKind.None)
        {
            Kind = kind;
            FactionId = factionId;
            KnightId = knightId;
            RelatedKnightId = relatedKnightId;
            ActionKind = actionKind;
        }
    }

    public sealed class TerritoryDelta
    {
        public KnightId KnightId { get; }
        public float BeforeArea { get; }
        public float AccountingArea { get; }
        public float FinalArea { get; }

        public TerritoryDelta(KnightId knightId, float beforeArea, float accountingArea, float finalArea)
        {
            KnightId = knightId;
            BeforeArea = beforeArea;
            AccountingArea = accountingArea;
            FinalArea = finalArea;
        }
    }

    public sealed class FactionRoundSummary
    {
        public FactionId FactionId { get; }
        public float StartingMoney { get; }
        public float MoneyAfterAccounting { get; }
        public float FinalMoney { get; }

        public FactionRoundSummary(FactionId factionId, float startingMoney, float moneyAfterAccounting, float finalMoney)
        {
            FactionId = factionId;
            StartingMoney = startingMoney;
            MoneyAfterAccounting = moneyAfterAccounting;
            FinalMoney = finalMoney;
        }
    }

    public sealed class RoundResult
    {
        public MatchState Before { get; }
        public MatchState After { get; }
        public IReadOnlyList<ActionCommand> Commands { get; }
        public IReadOnlyList<CalculatedCell> AccountingCells { get; }
        public IReadOnlyList<RoundBooking> Bookings { get; }
        public IReadOnlyList<RoundEvent> Events { get; }
        public IReadOnlyList<FactionRoundSummary> FactionSummaries { get; }
        public IReadOnlyList<TerritoryDelta> TerritoryDeltas { get; }

        public RoundResult(MatchState before, MatchState after, IReadOnlyList<ActionCommand> commands,
            IReadOnlyList<CalculatedCell> accountingCells, IReadOnlyList<RoundBooking> bookings,
            IReadOnlyList<RoundEvent> events, IReadOnlyList<FactionRoundSummary> factionSummaries,
            IReadOnlyList<TerritoryDelta> territoryDeltas)
        {
            Before = before;
            After = after;
            Commands = commands;
            AccountingCells = accountingCells;
            Bookings = bookings;
            Events = events;
            FactionSummaries = factionSummaries;
            TerritoryDeltas = territoryDeltas;
        }

        public string ToLogText()
        {
            var text = new StringBuilder();
            text.Append("Round ").Append(Before.RoundNumber).Append(" -> ").Append(After.RoundNumber)
                .Append("; outcome=").Append(After.Outcome);
            foreach (RoundBooking booking in Bookings)
                text.Append("\nBOOK ").Append(booking.FactionId).Append('/').Append(booking.KnightId)
                    .Append(' ').Append(booking.Kind).Append(' ')
                    .Append(booking.Amount.ToString("R", CultureInfo.InvariantCulture));
            foreach (RoundEvent roundEvent in Events)
                text.Append("\nEVENT ").Append(roundEvent.Kind).Append(' ')
                    .Append(roundEvent.KnightId?.ToString() ?? "-");
            return text.ToString();
        }
    }
}
