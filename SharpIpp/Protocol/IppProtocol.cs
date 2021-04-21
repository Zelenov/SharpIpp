using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using SharpIpp.Model;
using SharpIpp.Protocol.Extensions;

[assembly: InternalsVisibleTo("SharpIpp.Tests")]

namespace SharpIpp.Protocol
{
    internal partial class IppProtocol
    {
        private IppResponse ReadStream(Stream stream)
        {
            var res = new IppResponse();
            using var reader = new BinaryReader(stream, Encoding.ASCII, true);
            res.Version = (IppVersion) reader.ReadInt16BigEndian();
            res.StatusCode = (IppStatusCode) reader.ReadInt16BigEndian();
            res.RequestId = reader.ReadInt32BigEndian();
            while (stream.Position != stream.Length)
                if (!ReadSection(reader, res))
                    break;

            return res;
        }

        private bool ReadSection(BinaryReader reader, IppResponse res)
        {
            var delimiter = reader.ReadByte();
            var sectionDelimiter = delimiter;
            //https://tools.ietf.org/html/rfc8010#section-3.5.1
            if (sectionDelimiter > 0x0f)
                return false;

            IppAttribute? prevAttribute = null;
            do
            {
                delimiter = reader.ReadByte();
                switch (delimiter)
                {
                    case 0x03: return true;
                    case var n when n <= 0x0f:
                        sectionDelimiter = delimiter;
                        break;
                    default:
                        var attribute = ReadAttribute((Tag) delimiter, reader, prevAttribute);
                        prevAttribute = attribute;
                        switch (sectionDelimiter)
                        {
                            case (byte) Tag.OperationAttributesTag:
                                res.OperationAttributes.Add(attribute);
                                break;
                            case (byte) Tag.JobAttributesTag:
                                res.JobAttributes.Add(attribute);
                                break;
                            case (byte) Tag.PrinterAttributesTag:
                                res.PrinterAttributes.Add(attribute);
                                break;
                            case (byte) Tag.UnsupportedAttributesTag:
                                res.UnsupportedAttributes.Add(attribute);
                                break;
                            default:
                                res.OtherAttributes.Add(attribute);
                                break;
                        }

                        break;
                }
            } while (true);
        }

        public void Write(BinaryWriter stream, IppAttribute attribute, bool isSet)
        {
            stream.Write((byte) attribute.Tag);
            if (isSet)
            {
                stream.WriteBigEndian((short) 0);
            }
            else
            {
                stream.WriteBigEndian((short) attribute.Name.Length);
                stream.Write(Encoding.ASCII.GetBytes(attribute.Name));
            }

            var value = attribute.Value;
            WriteValue(value, stream);
        }

        public void WriteValue(object value, BinaryWriter stream)
        {
            //https://tools.ietf.org/html/rfc8010#section-3.5.2
            switch (value)
            {
                case NoValue v:
                    Write(v, stream);
                    break;
                case int v:
                    Write(v, stream);
                    break;
                case bool v:
                    Write(v, stream);
                    break;
                case string v:
                    Write(v, stream);
                    break;
                case DateTimeOffset v:
                    Write(v, stream);
                    break;
                case Resolution v:
                    Write(v, stream);
                    break;
                case Range v:
                    Write(v, stream);
                    break;
                default: throw new ArgumentException($"Type {value.GetType()} not supported in ipp");
            }
        }

        public object ReadValue(BinaryReader stream, Tag tag)
        {
            //https://tools.ietf.org/html/rfc8010#section-3.5.2
            switch (tag)
            {
                case Tag.Unsupported:
                case Tag.Unknown:
                case Tag.NoValue:
                    return ReadNoValue(stream);
                case Tag.Integer:
                case Tag.Enum:
                    return ReadInt(stream);
                case Tag.Boolean:
                    return ReadBool(stream);
                case Tag.OctetStringWithAnUnspecifiedFormat:
                    return ReadString(stream);
                case Tag.DateTime:
                    return ReadDateTimeOffset(stream);
                case Tag.Resolution:
                    return ReadResolution(stream);
                case Tag.RangeOfInteger:
                    return ReadRange(stream);
                case Tag.BegCollection:
                    return ReadString(stream); //TODO: collection https://tools.ietf.org/html/rfc8010#section-3.1.6
                case Tag.TextWithLanguage:
                case Tag.NameWithLanguage:
                    return ReadStringWithLanguage(stream);
                case Tag.EndCollection:
                    return ReadNoValue(stream); //TODO: collection https://tools.ietf.org/html/rfc8010#section-3.1.6
                case Tag.TextWithoutLanguage:
                case Tag.NameWithoutLanguage:
                case Tag.Keyword:
                case Tag.Uri:
                case Tag.UriScheme:
                case Tag.Charset:
                case Tag.NaturalLanguage:
                case Tag.MimeMediaType:
                case Tag.MemberAttrName:
                    return ReadString(stream);


                case Tag.OctetStringUnassigned3:
                case Tag.OctetStringUnassigned4:
                case Tag.OctetStringUnassigned5:
                case Tag.OctetStringUnassigned6:
                case Tag.OctetStringUnassigned7:
                case Tag.OctetStringUnassigned8:
                    return ReadString(stream);

                case Tag.IntegerUnassigned1:
                case Tag.IntegerUnassigned2:
                case Tag.IntegerUnassigned3:
                case Tag.IntegerUnassigned4:
                case Tag.IntegerUnassigned5:
                case Tag.IntegerUnassigned6:
                case Tag.IntegerUnassigned7:
                case Tag.IntegerUnassigned8:
                case Tag.IntegerUnassigned9:
                case Tag.IntegerUnassigned10:
                case Tag.IntegerUnassigned11:
                case Tag.IntegerUnassigned12:
                case Tag.IntegerUnassigned13:
                    return ReadInt(stream);

                case Tag.StringUnassigned1:
                case Tag.StringUnassigned2:
                case Tag.StringUnassigned3:
                case Tag.StringUnassigned4:
                case Tag.StringUnassigned5:
                case Tag.StringUnassigned6:
                case Tag.StringUnassigned7:
                case Tag.StringUnassigned8:
                case Tag.StringUnassigned9:
                case Tag.StringUnassigned10:
                case Tag.StringUnassigned11:
                case Tag.StringUnassigned12:
                case Tag.StringUnassigned13:
                case Tag.StringUnassigned14:
                case Tag.StringUnassigned15:
                case Tag.StringUnassigned16:
                case Tag.StringUnassigned17:
                case Tag.StringUnassigned18:
                case Tag.StringUnassigned19:
                case Tag.StringUnassigned20:
                case Tag.StringUnassigned21:
                case Tag.StringUnassigned22:
                case Tag.StringUnassigned23:
                    return ReadString(stream);
                default: throw new ArgumentException($"Ipp tag {tag} not supported");
            }
        }

        public IppAttribute ReadAttribute(Tag tag, BinaryReader stream, IppAttribute? prevAttribute)
        {
            var len = stream.ReadInt16BigEndian();
            var name = Encoding.ASCII.GetString(stream.ReadBytes(len));
            var value = ReadValue(stream, tag);
            var normalizedName = string.IsNullOrEmpty(name) && prevAttribute != null ? prevAttribute.Name : name;
            var attribute = new IppAttribute(tag, normalizedName, value);
            //!!
            //Console.WriteLine($"{tag} {name} {attribute}");
            return attribute;
        }

        public void Write(IEnumerable<IppAttribute> attributes, BinaryWriter writer)
        {
            //operation-attributes-tag https://tools.ietf.org/html/rfc8010#section-3.5.1
            writer.Write((byte) Tag.OperationAttributesTag);
            IppAttribute? prevAttribute = null;
            foreach (var ippAttribute in attributes)
            {
                var isSet = prevAttribute != null && ippAttribute.Name == prevAttribute.Name;
                Write(writer, ippAttribute, isSet);
                prevAttribute = ippAttribute;
            }

            //end-of-attributes-tag https://tools.ietf.org/html/rfc8010#section-3.5.1
            writer.Write((byte) Tag.EndOfAttributesTag);
        }
    }
}