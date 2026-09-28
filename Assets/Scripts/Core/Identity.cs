using System;

namespace VoronationCore
{
    [Serializable]
    public readonly struct FactionId : IEquatable<FactionId>, IComparable<FactionId>
    {
        public int Value { get; }

        public FactionId(int value)
        {
            if (value < 1) throw new ArgumentOutOfRangeException(nameof(value));
            Value = value;
        }

        public int CompareTo(FactionId other) => Value.CompareTo(other.Value);
        public bool Equals(FactionId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is FactionId other && Equals(other);
        public override int GetHashCode() => Value;
        public override string ToString() => "F" + Value;
        public static bool operator ==(FactionId left, FactionId right) => left.Equals(right);
        public static bool operator !=(FactionId left, FactionId right) => !left.Equals(right);
    }

    [Serializable]
    public readonly struct KnightId : IEquatable<KnightId>, IComparable<KnightId>
    {
        public FactionId FactionId { get; }
        public int Number { get; }

        public KnightId(FactionId factionId, int number)
        {
            if (number < 1) throw new ArgumentOutOfRangeException(nameof(number));
            FactionId = factionId;
            Number = number;
        }

        public int CompareTo(KnightId other)
        {
            int faction = FactionId.CompareTo(other.FactionId);
            return faction != 0 ? faction : Number.CompareTo(other.Number);
        }

        public bool Equals(KnightId other) => FactionId == other.FactionId && Number == other.Number;
        public override bool Equals(object obj) => obj is KnightId other && Equals(other);
        public override int GetHashCode() => (FactionId.GetHashCode() * 397) ^ Number;
        public override string ToString() => FactionId + ":K" + Number;
        public static bool operator ==(KnightId left, KnightId right) => left.Equals(right);
        public static bool operator !=(KnightId left, KnightId right) => !left.Equals(right);
    }
}
