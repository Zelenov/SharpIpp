using SharpIpp.Protocol;
using SharpIpp.Protocol.Models;
using System;
using System.Linq;

namespace SharpIpp.Mapping.Profiles
{
    // ReSharper disable once UnusedMember.Global
    internal class DocumentAttributesProfile : IProfile
    {
        public void CreateMaps(IMapperConstructor mapper)
        {
            mapper.CreateMap<DocumentAttributes, IppRequestMessage>((src, dst, map) =>
            {
                var operation = dst.OperationAttributes;

                if (src.DocumentName != null)
                {
                    operation.Add(new IppAttribute(Tag.NameWithoutLanguage, JobAttribute.DocumentName, src.DocumentName));
                }

                if (src.DocumentFormat != null)
                {
                    operation.Add(new IppAttribute(Tag.MimeMediaType, JobAttribute.DocumentFormat, src.DocumentFormat));
                }

                if (src.DocumentNaturalLanguage != null)
                {
                    operation.Add(new IppAttribute(Tag.NaturalLanguage,
                        JobAttribute.DocumentNaturalLanguage,
                        src.DocumentNaturalLanguage));
                }

                if (src.Compression != null)
                {
                    operation.Add(new IppAttribute(Tag.Keyword, JobAttribute.Compression, map.Map<string>(src.Compression)));
                }

                return dst;
            });

            mapper.CreateMap<IIppRequestMessage, DocumentAttributes>( ( src, dst, map ) =>
            {
                dst.DocumentName = src.OperationAttributes.FirstOrDefault( x => x.Name == JobAttribute.DocumentName )?.Value as string;
                dst.DocumentFormat = src.OperationAttributes.FirstOrDefault( x => x.Name == JobAttribute.DocumentFormat )?.Value as string;
                dst.DocumentNaturalLanguage = src.OperationAttributes.FirstOrDefault( x => x.Name == JobAttribute.DocumentNaturalLanguage )?.Value as string;
                dst.Compression = src.OperationAttributes.FirstOrDefault( x => x.Name == JobAttribute.Compression )?.Value is int compression && Enum.IsDefined( typeof( Compression ), compression ) ? (Compression)compression : null;
                return dst;
            } );
        }
    }
}
