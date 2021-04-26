using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using SharpIpp.Exceptions;
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
            try
            {
                using var reader = new BinaryReader(stream, Encoding.ASCII, true);
                res.Version = (IppVersion) reader.ReadInt16BigEndian();
                res.StatusCode = (IppStatusCode) reader.ReadInt16BigEndian();
                res.RequestId = reader.ReadInt32BigEndian();
                ReadSection(reader, res);
                return res;
            }
            catch (Exception ex)
            {
                throw new IppResponseException(
                    $"Failed to parse ipp response. Current response parsing ended on: \n{res}", ex, res);
            }
        }

        private void ReadSection(BinaryReader reader, IppResponse res)
        {
            IppAttribute? prevAttribute = null;
            List<IppAttribute>? attributes = null;
            do
            {
                var data = reader.ReadByte();
                var sectionTag = (SectionTag)data;
                switch (sectionTag)
                {
                    //https://tools.ietf.org/html/rfc8010#section-3.5.1
                    case SectionTag.EndOfAttributesTag: return;
                    case SectionTag.Reserved:
                    case SectionTag.OperationAttributesTag:
                    case SectionTag.JobAttributesTag:
                    case SectionTag.PrinterAttributesTag:
                    case SectionTag.UnsupportedAttributesTag:
                        var section = new IppSection{Tag = sectionTag};
                        res.Sections.Add(section);
                        attributes = section.Attributes;
                        break;
                    default:
                        var attribute = ReadAttribute((Tag) data, reader, prevAttribute);
                        prevAttribute = attribute;
                        if (attributes==null)
                            throw new ArgumentException($"Section start tag not found in stream. Expected < 0x06. Actual: {data}");
                        attributes.Add(attribute);
                      
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
                    //TODO: collection https://tools.ietf.org/html/rfc8010#section-3.1.6
                    return ReadString(stream);
                case Tag.TextWithLanguage:
                case Tag.NameWithLanguage:
                    return ReadStringWithLanguage(stream);
                case Tag.EndCollection:
                    //TODO: collection https://tools.ietf.org/html/rfc8010#section-3.1.6
                    return ReadNoValue(stream);
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


                case Tag.OctetStringUnassigned38:
                case Tag.OctetStringUnassigned39:
                case Tag.OctetStringUnassigned3A:
                case Tag.OctetStringUnassigned3B:
                case Tag.OctetStringUnassigned3C:
                case Tag.OctetStringUnassigned3D:
                case Tag.OctetStringUnassigned3E:
                case Tag.OctetStringUnassigned3F:
                    return ReadString(stream);

                case Tag.IntegerUnassigned20:
                case Tag.IntegerUnassigned24:
                case Tag.IntegerUnassigned25:
                case Tag.IntegerUnassigned26:
                case Tag.IntegerUnassigned27:
                case Tag.IntegerUnassigned28:
                case Tag.IntegerUnassigned29:
                case Tag.IntegerUnassigned2A:
                case Tag.IntegerUnassigned2B:
                case Tag.IntegerUnassigned2C:
                case Tag.IntegerUnassigned2D:
                case Tag.IntegerUnassigned2E:
                case Tag.IntegerUnassigned2F:
                    return ReadInt(stream);

                case Tag.StringUnassigned40:
                case Tag.StringUnassigned43:
                case Tag.StringUnassigned4B:
                case Tag.StringUnassigned4C:
                case Tag.StringUnassigned4D:
                case Tag.StringUnassigned4E:
                case Tag.StringUnassigned4F:
                case Tag.StringUnassigned50:
                case Tag.StringUnassigned51:
                case Tag.StringUnassigned52:
                case Tag.StringUnassigned53:
                case Tag.StringUnassigned54:
                case Tag.StringUnassigned55:
                case Tag.StringUnassigned56:
                case Tag.StringUnassigned57:
                case Tag.StringUnassigned58:
                case Tag.StringUnassigned59:
                case Tag.StringUnassigned5A:
                case Tag.StringUnassigned5B:
                case Tag.StringUnassigned5C:
                case Tag.StringUnassigned5D:
                case Tag.StringUnassigned5E:
                case Tag.StringUnassigned5F:
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
            if (string.IsNullOrEmpty(normalizedName))
                throw new ArgumentException("0 length attribute name found not in a 1setOf");

            var attribute = new IppAttribute(tag, normalizedName, value);
            return attribute;
        }

        public void Write(IppRequest request, BinaryWriter writer)
        {
            writer.WriteBigEndian((short) request.IppVersion);
            writer.WriteBigEndian((short) request.IppOperation);
            writer.WriteBigEndian(request.RequestId);

            //operation-attributes-tag https://tools.ietf.org/html/rfc8010#section-3.5.1
            var attributes = request.OperationAttributes.Select(x => (SectionTag.OperationAttributesTag, x))
               .Concat(request.JobAttributes.Select(x => (SectionTag.JobAttributesTag, x)))
               .ToArray();
            if (!attributes.Any())
                return;
            
            IppAttribute? prevAttribute = null;
            SectionTag? prevTag = null;
            foreach (var (ippTag, ippAttribute) in attributes)
            {
                if (prevTag == null || ippTag != prevTag)
                    writer.Write((byte) ippTag);

                var isSet = prevAttribute != null && ippAttribute.Name == prevAttribute.Name;
                Write(writer, ippAttribute, isSet);
                prevAttribute = ippAttribute;
                prevTag = ippTag;
            }

            //end-of-attributes-tag https://tools.ietf.org/html/rfc8010#section-3.5.1
            writer.Write((byte)SectionTag.EndOfAttributesTag);
        }
    }
}