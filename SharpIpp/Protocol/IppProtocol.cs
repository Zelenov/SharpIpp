using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using SharpIpp.Exceptions;
using SharpIpp.Protocol.Extensions;
using SharpIpp.Protocol.Models;

[assembly: InternalsVisibleTo("SharpIpp.Tests")]

namespace SharpIpp.Protocol
{
    /// <summary>
    ///     Ipp protocol reader-writer.
    ///     Ipp protocol only supports common types:
    ///     <see cref="int"/>
    ///     <see cref="bool"/>
    ///     <see cref="string" />
    ///     <see cref="DateTimeOffset" />
    ///     <see cref="NoValue" />
    ///     <see cref="Range" />
    ///     <see cref="Resolution" />
    ///     <see cref="StringWithLanguage" />
    ///     all other types must be mapped via IMapper in-onto these
    /// </summary>
    internal partial class IppProtocol : IIppProtocol
    {
        public async Task WriteIppRequestAsync(IIppRequestMessage ippRequestMessage, Stream stream, CancellationToken cancellationToken = default)
        {
            if (ippRequestMessage == null)
            {
                throw new ArgumentException($"{nameof(ippRequestMessage)}");
            }

            if (stream == null)
            {
                throw new ArgumentException($"{nameof(stream)}");
            }

            using var writer = new BinaryWriter(stream, Encoding.ASCII, true);
            writer.WriteBigEndian( (short)ippRequestMessage.Version );
            writer.WriteBigEndian( (short)ippRequestMessage.IppOperation );
            writer.WriteBigEndian( ippRequestMessage.RequestId );
            WriteSection(ippRequestMessage, writer);

            if (ippRequestMessage.Document != null)
            {
                await ippRequestMessage.Document.CopyToAsync(stream, 81920, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<IIppRequestMessage> ReadIppRequestAsync( Stream inputStream, CancellationToken cancellationToken = default )
        {
            using var reader = new BinaryReader( inputStream, Encoding.ASCII, true );
            return await ReadIppRequestAsync( reader, cancellationToken );
        }

        public Task<IIppResponseMessage> ReadIppResponseAsync(Stream stream, CancellationToken cancellationToken = default)
        {
            var res = new IppResponseMessage();

            try
            {
                using var reader = new BinaryReader(stream, Encoding.ASCII, true);
                res.Version = (IppVersion)reader.ReadInt16BigEndian();
                res.StatusCode = (IppStatusCode)reader.ReadInt16BigEndian();
                res.RequestId = reader.ReadInt32BigEndian();
                ReadSection(reader, res);
                return Task.FromResult((IIppResponseMessage)res);
            }
            catch (Exception ex)
            {
                throw new IppResponseException($"Failed to parse ipp response. Current response parsing ended on: \n{res}", ex, res);
            }
        }

        private void ReadSection(BinaryReader reader, IIppResponseMessage res)
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
                        var section = new IppSection { Tag = sectionTag };
                        res.Sections.Add(section);
                        attributes = section.Attributes;
                        break;
                    default:
                        var attribute = ReadAttribute((Tag)data, reader, prevAttribute);
                        prevAttribute = attribute;

                        if (attributes == null)
                        {
                            throw new ArgumentException($"Section start tag not found in stream. Expected < 0x06. Actual: {data}");
                        }

                        attributes.Add(attribute);

                        break;
                }
            }
            while (true);
        }

        private void ReadSection( BinaryReader reader, IIppRequestMessage res )
        {
            IppAttribute? prevAttribute = null;
            List<IppAttribute>? attributes = null;
            do
            {
                var data = reader.ReadByte();
                var sectionTag = (SectionTag)data;

                switch ( sectionTag )
                {
                    case SectionTag.OperationAttributesTag:
                        attributes = res.OperationAttributes;
                        break;
                    case SectionTag.JobAttributesTag:
                        attributes = res.JobAttributes;
                        break;
                    case SectionTag.EndOfAttributesTag:
                        return;
                    default:
                        if ( attributes == null )
                        {
                            reader.BaseStream.Position--;
                            return;
                        }
                        var attribute = ReadAttribute( (Tag)data, reader, prevAttribute );
                        prevAttribute = attribute;
                        attributes.Add( attribute );
                        break;
                }
            }
            while ( true );
        }

        public void Write(BinaryWriter stream, IppAttribute attribute, bool isSet)
        {
            stream.Write((byte)attribute.Tag);

            if (isSet)
            {
                stream.WriteBigEndian((short)0);
            }
            else
            {
                stream.WriteBigEndian((short)attribute.Name.Length);
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
                default: throw new ArgumentException($"Type {value?.GetType()} not supported in ipp");
            }
        }

        public Task WriteIppResponseAsync( IIppResponseMessage ippResponseMessage, Stream stream, CancellationToken cancellationToken = default )
        {
            if ( ippResponseMessage == null )
            {
                throw new ArgumentException( $"{nameof( ippResponseMessage )}" );
            }
            if ( stream == null )
            {
                throw new ArgumentException( $"{nameof( stream )}" );
            }
            using var writer = new BinaryWriter( stream, Encoding.ASCII, true );
            writer.WriteBigEndian( (short)ippResponseMessage.Version );
            writer.WriteBigEndian( (short)ippResponseMessage.StatusCode );
            writer.WriteBigEndian( ippResponseMessage.RequestId );
            WriteSection( ippResponseMessage, writer );
            return Task.CompletedTask;
        }

        public object ReadValue(BinaryReader stream, Tag tag)
        {
            //https://tools.ietf.org/html/rfc8010#section-3.5.2
            return tag switch
            {
                Tag.Unsupported => ReadNoValue(stream),
                Tag.Unknown => ReadNoValue(stream),
                Tag.NoValue => ReadNoValue(stream),
                Tag.Integer => ReadInt(stream),
                Tag.Enum => ReadInt(stream),
                Tag.Boolean => ReadBool(stream),
                Tag.OctetStringWithAnUnspecifiedFormat => ReadString(stream),
                Tag.DateTime => ReadDateTimeOffset(stream),
                Tag.Resolution => ReadResolution(stream),
                Tag.RangeOfInteger => ReadRange(stream),
                Tag.BegCollection =>
                    //TODO: collection https://tools.ietf.org/html/rfc8010#section-3.1.6
                    ReadString(stream),
                Tag.TextWithLanguage => ReadStringWithLanguage(stream),
                Tag.NameWithLanguage => ReadStringWithLanguage(stream),
                Tag.EndCollection =>
                    //TODO: collection https://tools.ietf.org/html/rfc8010#section-3.1.6
                    ReadNoValue(stream),
                Tag.TextWithoutLanguage => ReadString(stream),
                Tag.NameWithoutLanguage => ReadString(stream),
                Tag.Keyword => ReadString(stream),
                Tag.Uri => ReadString(stream),
                Tag.UriScheme => ReadString(stream),
                Tag.Charset => ReadString(stream),
                Tag.NaturalLanguage => ReadString(stream),
                Tag.MimeMediaType => ReadString(stream),
                Tag.MemberAttrName => ReadString(stream),
                Tag.OctetStringUnassigned38 => ReadString(stream),
                Tag.OctetStringUnassigned39 => ReadString(stream),
                Tag.OctetStringUnassigned3A => ReadString(stream),
                Tag.OctetStringUnassigned3B => ReadString(stream),
                Tag.OctetStringUnassigned3C => ReadString(stream),
                Tag.OctetStringUnassigned3D => ReadString(stream),
                Tag.OctetStringUnassigned3E => ReadString(stream),
                Tag.OctetStringUnassigned3F => ReadString(stream),
                Tag.IntegerUnassigned20 => ReadInt(stream),
                Tag.IntegerUnassigned24 => ReadInt(stream),
                Tag.IntegerUnassigned25 => ReadInt(stream),
                Tag.IntegerUnassigned26 => ReadInt(stream),
                Tag.IntegerUnassigned27 => ReadInt(stream),
                Tag.IntegerUnassigned28 => ReadInt(stream),
                Tag.IntegerUnassigned29 => ReadInt(stream),
                Tag.IntegerUnassigned2A => ReadInt(stream),
                Tag.IntegerUnassigned2B => ReadInt(stream),
                Tag.IntegerUnassigned2C => ReadInt(stream),
                Tag.IntegerUnassigned2D => ReadInt(stream),
                Tag.IntegerUnassigned2E => ReadInt(stream),
                Tag.IntegerUnassigned2F => ReadInt(stream),
                Tag.StringUnassigned40 => ReadString(stream),
                Tag.StringUnassigned43 => ReadString(stream),
                Tag.StringUnassigned4B => ReadString(stream),
                Tag.StringUnassigned4C => ReadString(stream),
                Tag.StringUnassigned4D => ReadString(stream),
                Tag.StringUnassigned4E => ReadString(stream),
                Tag.StringUnassigned4F => ReadString(stream),
                Tag.StringUnassigned50 => ReadString(stream),
                Tag.StringUnassigned51 => ReadString(stream),
                Tag.StringUnassigned52 => ReadString(stream),
                Tag.StringUnassigned53 => ReadString(stream),
                Tag.StringUnassigned54 => ReadString(stream),
                Tag.StringUnassigned55 => ReadString(stream),
                Tag.StringUnassigned56 => ReadString(stream),
                Tag.StringUnassigned57 => ReadString(stream),
                Tag.StringUnassigned58 => ReadString(stream),
                Tag.StringUnassigned59 => ReadString(stream),
                Tag.StringUnassigned5A => ReadString(stream),
                Tag.StringUnassigned5B => ReadString(stream),
                Tag.StringUnassigned5C => ReadString(stream),
                Tag.StringUnassigned5D => ReadString(stream),
                Tag.StringUnassigned5E => ReadString(stream),
                Tag.StringUnassigned5F => ReadString(stream),
                _ => throw new ArgumentException($"Ipp tag {tag} not supported")
            };
        }

        public IppAttribute ReadAttribute(Tag tag, BinaryReader stream, IppAttribute? prevAttribute)
        {
            var len = stream.ReadInt16BigEndian();
            var name = Encoding.ASCII.GetString(stream.ReadBytes(len));
            var value = ReadValue(stream, tag);
            var normalizedName = string.IsNullOrEmpty(name) && prevAttribute != null ? prevAttribute.Name : name;

            if (string.IsNullOrEmpty(normalizedName))
            {
                throw new ArgumentException("0 length attribute name found not in a 1setOf");
            }

            var attribute = new IppAttribute(tag, normalizedName, value);
            return attribute;
        }

        public async Task<IIppRequestMessage> ReadIppRequestAsync( BinaryReader reader, CancellationToken cancellationToken = default )
        {
            IppRequestMessage message = new IppRequestMessage
            {
                Version = (IppVersion)reader.ReadInt16BigEndian(),
                IppOperation = (IppOperation)reader.ReadInt16BigEndian(),
                RequestId = reader.ReadInt32BigEndian()
            };
            ReadSection( reader, message );
            message.Document = new MemoryStream();
            await reader.BaseStream.CopyToAsync( message.Document );
            return message;
        }

        public void WriteSection(IIppRequestMessage requestMessage, BinaryWriter writer)
        {
            //operation-attributes-tag https://tools.ietf.org/html/rfc8010#section-3.5.1
            var attributes = requestMessage.OperationAttributes.Select(x => (SectionTag.OperationAttributesTag, x))
                .Concat(requestMessage.JobAttributes.Select(x => (SectionTag.JobAttributesTag, x)))
                .ToArray();

            if (!attributes.Any())
            {
                return;
            }

            IppAttribute? prevAttribute = null;
            SectionTag? prevTag = null;

            foreach (var (ippTag, ippAttribute) in attributes)
            {
                if (prevTag == null || ippTag != prevTag)
                {
                    writer.Write((byte)ippTag);
                }

                var isSet = prevAttribute != null && ippAttribute.Name == prevAttribute.Name;
                Write(writer, ippAttribute, isSet);
                prevAttribute = ippAttribute;
                prevTag = ippTag;
            }

            //end-of-attributes-tag https://tools.ietf.org/html/rfc8010#section-3.5.1
            writer.Write((byte)SectionTag.EndOfAttributesTag);
        }

        public void WriteSection( IIppResponseMessage responseMessage, BinaryWriter writer )
        {
            IppAttribute? prevAttribute = null;
            SectionTag? prevTag = null;
            foreach ( var ippSection in responseMessage.Sections )
            {
                if ( prevTag == null || ippSection.Tag != prevTag )
                {
                    writer.Write( (byte)ippSection.Tag );
                }
                foreach(var ippAttribute in ippSection.Attributes )
                {
                    var isSet = prevAttribute != null && ippAttribute.Name == prevAttribute.Name;
                    Write( writer, ippAttribute, isSet );
                    prevAttribute = ippAttribute;
                }
                prevTag = ippSection.Tag;
            }
            //end-of-attributes-tag https://tools.ietf.org/html/rfc8010#section-3.5.1
            writer.Write( (byte)SectionTag.EndOfAttributesTag );
        }
    }
}
