namespace VoronationCore
{
    public interface IRandomSource
    {
        int Next(int minimumInclusive, int maximumExclusive);
    }

    public interface IRandomSourceFactory
    {
        IRandomSource Create(int matchSeed, int roundNumber);
    }

    public sealed class DeterministicRandomSourceFactory : IRandomSourceFactory
    {
        public IRandomSource Create(int matchSeed, int roundNumber)
        {
            unchecked
            {
                uint seed = (uint)matchSeed * 747796405u + (uint)roundNumber * 2891336453u + 277803737u;
                return new XorShiftRandomSource(seed == 0 ? 1u : seed);
            }
        }
    }

    internal sealed class XorShiftRandomSource : IRandomSource
    {
        private uint state;
        public XorShiftRandomSource(uint seed) { state = seed; }

        public int Next(int minimumInclusive, int maximumExclusive)
        {
            if (maximumExclusive <= minimumInclusive) throw new System.ArgumentOutOfRangeException(nameof(maximumExclusive));
            state ^= state << 13;
            state ^= state >> 17;
            state ^= state << 5;
            return minimumInclusive + (int)(state % (uint)(maximumExclusive - minimumInclusive));
        }
    }
}
