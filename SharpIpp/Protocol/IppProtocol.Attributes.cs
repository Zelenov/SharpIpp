using System;
using System.IO;
using System.Text;
using SharpIpp.Model;
using SharpIpp.Protocol.Extensions;

namespace SharpIpp.Protocol
{
    internal partial class IppProtocol
    {
        private static readonly byte Plus = Encoding.ASCII.GetBytes("+")[0];
        private static readonly byte Minus = Encoding.ASCII.GetBytes("-")[0];

        public void Write(NoValue _, BinaryWriter stream)
        {
            stream.WriteBigEndian((short) 0);
        }

        public NoValue ReadNoValue(BinaryReader stream)
        {
            var length = stream.ReadInt16BigEndian();
            if (length != 0)
                throw new ArgumentException($"Expected no-value length: 0, actual :{length}");

            return new NoValue();
        }

        public void Write(bool value, BinaryWriter stream)
        {
            stream.WriteBigEndian((short) 1);
            stream.Write(value ? (byte) 0x01 : (byte) 0x00);
        }

        public bool ReadBool(BinaryReader stream)
        {
            var length = stream.ReadInt16BigEndian();
            if (length != 1)
                throw new ArgumentException($"Expected bool value length: 1, actual :{length}");

            var value = stream.ReadByte();
            if (value == 0x00)
                return false;

            if (value == 0x01)
                return true;

            throw new ArgumentException($"boolean value {value} not supported");
        }

        public void Write(DateTimeOffset value, BinaryWriter stream)
        {
            stream.WriteBigEndian((short) value.Year);
            stream.Write((byte) value.Month);
            stream.Write((byte) value.Day);
            stream.Write((byte) value.Hour);
            stream.Write((byte) value.Minute);
            stream.Write((byte) value.Second);
            stream.Write((byte) (value.Millisecond / 100));
            stream.Write(value.Offset > TimeSpan.Zero ? Plus : Minus);
            stream.Write(Math.Abs(value.Offset.Hours));
            stream.Write(Math.Abs(value.Offset.Minutes));
        }

        public DateTimeOffset ReadDateTimeOffset(BinaryReader stream)
        {
            var length = stream.ReadInt16BigEndian();
            if (length != 11)
                throw new ArgumentException($"Expected datetime value length: 11, actual :{length}");

            var year = stream.ReadInt16BigEndian();
            var month = stream.ReadByte();
            var day = stream.ReadByte();
            var hour = stream.ReadByte();
            var minute = stream.ReadByte();
            var second = stream.ReadByte();
            var decisecond = stream.ReadByte();
            var plusMinus = stream.ReadByte();
            var offsetDir = plusMinus == Plus ? 1 :
                plusMinus == Minus ? -1 :
                throw new ArgumentException($"DateTime offset direction {plusMinus} not supported");
            var offsetHour = stream.ReadByte();
            var offsetMinute = stream.ReadByte();
            var dateTimeOffset = new DateTimeOffset(year, month, day, hour, minute, second, decisecond * 100,
                new TimeSpan(offsetHour * offsetDir, offsetMinute, 0));
            return dateTimeOffset;
        }

        public void Write(int value, BinaryWriter stream)
        {
            stream.WriteBigEndian((short) 4);
            stream.WriteBigEndian(value);
        }

        public int ReadInt(BinaryReader stream)
        {
            var length = stream.ReadInt16BigEndian();
            if (length != 4)
                throw new ArgumentException($"Expected integer value length: 4, actual :{length}");

            var value = stream.ReadInt32BigEndian();
            return value;
        }

        public void Write(Range value, BinaryWriter stream)
        {
            stream.WriteBigEndian((short) 8);
            stream.WriteBigEndian(value.Lower);
            stream.WriteBigEndian(value.Upper);
        }

        public Range ReadRange(BinaryReader stream)
        {
            var length = stream.ReadInt16BigEndian();
            if (length != 8)
                throw new ArgumentException($"Expected range value length: 8, actual :{length}");

            var lower = stream.ReadInt32BigEndian();
            var upper = stream.ReadInt32BigEndian();
            return new Range(lower, upper);
        }

        public void Write(Resolution value, BinaryWriter stream)
        {
            stream.WriteBigEndian((short) 9);
            stream.WriteBigEndian(value.Width);
            stream.WriteBigEndian(value.Height);
            stream.Write((byte) value.Units);
        }

        public Resolution ReadResolution(BinaryReader stream)
        {
            var length = stream.ReadInt16BigEndian();
            if (length != 9)
                throw new ArgumentException($"Expected resolution value length: 9, actual :{length}");

            var width = stream.ReadInt32BigEndian();
            var height = stream.ReadInt32BigEndian();
            var units = stream.ReadByte();
            return new Resolution(width, height, (ResolutionUnit) units);
        }

        public void Write(string value, BinaryWriter stream)
        {
            stream.WriteBigEndian((short) value.Length);
            stream.Write(Encoding.ASCII.GetBytes(value));
        }

        public string ReadString(BinaryReader stream)
        {
            var len = stream.ReadInt16BigEndian();
            return Encoding.ASCII.GetString(stream.ReadBytes(len));
        }

        public void Write(StringWithLanguage value, BinaryWriter stream)
        {
            stream.WriteBigEndian((short) (value.Language.Length + value.Value.Length));
            Write(value.Language, stream);
            Write(value.Value, stream);
        }

        public StringWithLanguage ReadStringWithLanguage(BinaryReader stream)
        {
            var _ = stream.ReadInt16BigEndian();
            var language = ReadString(stream);
            var value = ReadString(stream);
            return new StringWithLanguage(language, value);
        }
    }
}
