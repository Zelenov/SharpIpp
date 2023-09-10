using System;

namespace SharpIpp.Protocol.Models
{
    public class IppVersion : IEquatable<IppVersion>, IComparable<IppVersion>
    {
        public byte Major { get; internal set; }
        public byte Minor { get; internal set; }
        public IppVersion( short int16BigEndian )
        {
            byte[] bytes = BitConverter.GetBytes( int16BigEndian );
            Major = bytes[ 1 ];
            Minor = bytes[ 0 ];
        }

        public IppVersion( byte major, byte minor  )
        {
            Major = major;
            Minor = minor;
        }

        public static IppVersion V11 { get; } = new( 1, 1 );
        public static IppVersion CUPS10 { get; } = new( 1, 2 );

        public override string ToString() => Major + "." + Minor;

        public decimal ToDecimal() => Major + Minor / 100;

        public short ToInt16BigEndian() => BitConverter.ToInt16( new byte[] { Minor, Major }, 0 );

        public bool Equals( IppVersion other )
        {
            return other != null
                && Major == other.Major
                && Minor == other.Minor;
        }

        public override bool Equals( object? obj )
        {
            return ReferenceEquals( this, obj ) || obj is IppVersion other && Equals( other );
        }

        public override int GetHashCode()
        {
            return Major.GetHashCode() * 17 + Minor.GetHashCode();
        }

        public int CompareTo( IppVersion other )
        {
            if (other == null)
                return 1;
            return ToDecimal().CompareTo( other.ToDecimal() );
        }

        public static bool operator ==( IppVersion? left, IppVersion? right )
        {
            return Equals( left, right );
        }

        public static bool operator !=( IppVersion? left, IppVersion? right )
        {
            return !Equals( left, right );
        }
    }
}
