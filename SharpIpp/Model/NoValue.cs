using System;

namespace SharpIpp.Model
{
    public struct NoValue : IEquatable<NoValue>
    {
        public override string ToString() => "no value";
        public bool Equals(NoValue other) => true;

        public override bool Equals(object obj) => obj is NoValue other && Equals(other);

        public override int GetHashCode() => 0;
        public static NoValue Instance = new NoValue();
    }
}