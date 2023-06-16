using System.IO;

namespace SharpIpp.Protocol.Extensions
{
    internal static class BinaryReaderExtensions
    {
        public static short ReadInt16BigEndian(this BinaryReader reader)
        {
            return Bytes.Reverse(reader.ReadInt16());
        }

        public static int ReadInt32BigEndian(this BinaryReader reader)
        {
            return Bytes.Reverse(reader.ReadInt32());
        }

        public static short ReadInt16BigEndianAsync( this BinaryReader reader )
        {
            return Bytes.Reverse( reader.ReadInt16() );
        }

        public static int ReadInt32BigEndianAsync( this BinaryReader reader )
        {
            return Bytes.Reverse( reader.ReadInt32() );
        }
    }
}
