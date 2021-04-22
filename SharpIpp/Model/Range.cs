using System;

namespace SharpIpp.Model
{
    public struct Range : IEquatable<Range>
    {
        public int Lower { get; }
        public int Upper { get; }

        public Range(int lower, int upper)
        {
            Lower = lower;
            Upper = upper;
        }

        public override string ToString() => $"{Lower} - {Upper}";

        public bool Equals(Range other) => Lower == other.Lower && Upper == other.Upper;

        public override bool Equals(object obj) => obj is Range other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return (Lower * 397) ^ Upper;
            }
        }
    }
}