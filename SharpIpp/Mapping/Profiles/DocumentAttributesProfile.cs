using SharpIpp.Protocol.Models;

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
                    operation.Add(new IppAttribute(Tag.NameWithoutLanguage, "document-name", src.DocumentName));
                }

                if (src.DocumentFormat != null)
                {
                    operation.Add(new IppAttribute(Tag.MimeMediaType, "document-format", src.DocumentFormat));
                }

                if (src.DocumentNaturalLanguage != null)
                {
                    operation.Add(new IppAttribute(Tag.NaturalLanguage,
                        "document-natural-language",
                        src.DocumentNaturalLanguage));
                }

                if (src.Compression != null)
                {
                    operation.Add(new IppAttribute(Tag.Keyword, "compression", map.Map<string>(src.Compression)));
                }

                return dst;
            });
        }
    }
}
