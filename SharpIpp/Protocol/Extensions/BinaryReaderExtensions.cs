using System.IO;

namespace SharpIpp.Protocol.Extensions
{
    public static class BinaryReaderExtensions
    {
        public static short ReadInt16BigEndian(this BinaryReader reader) => Bytes.Reverse(reader.ReadInt16());

        public static int ReadInt32BigEndian(this BinaryReader reader) => Bytes.Reverse(reader.ReadInt32());
    }
}