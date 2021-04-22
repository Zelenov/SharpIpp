using System.IO;

namespace SharpIpp.Protocol.Extensions
{
    internal static class BinaryWriterExtensions
    {
        public static void WriteBigEndian(this BinaryWriter writer, short value)
        {
            writer.Write(Bytes.Reverse(value));
        }

        public static void WriteBigEndian(this BinaryWriter writer, int value)
        {
            writer.Write(Bytes.Reverse(value));
        }
    }
}