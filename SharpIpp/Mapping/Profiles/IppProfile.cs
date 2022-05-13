using SharpIpp.Models;
using SharpIpp.Protocol;
using SharpIpp.Protocol.Models;

namespace SharpIpp.Mapping.Profiles
{
    // ReSharper disable once UnusedMember.Global
    internal class IppProfile : IProfile
    {
        public void CreateMaps(IMapperConstructor mapper)
        {
            mapper.CreateMap<IIppRequest, IppRequestMessage>((src, dst, map) =>
            {
                dst.Version = src.Version;
                dst.RequestId = src.RequestId;
                var operation = dst.OperationAttributes;
                operation.Add(new IppAttribute(Tag.Charset, "attributes-charset", "utf-8"));
                operation.Add(new IppAttribute(Tag.NaturalLanguage, "attributes-natural-language", "en"));

                if (src.RequestingUserName != null)
                {
                    operation.Add(new IppAttribute(Tag.NameWithoutLanguage,
                        "requesting-user-name",
                        src.RequestingUserName));
                }

                return dst;
            });

            mapper.CreateMap<IppResponseMessage, IIppResponseMessage>((src, dst, map) =>
            {
                dst.Version = src.Version;
                dst.RequestId = src.RequestId;
                dst.Sections.AddRange(src.Sections);
                return dst;
            });

            mapper.CreateMap<IIppPrinterRequest, IppRequestMessage>((src, dst, map) =>
            {
                map.Map<IIppRequest, IppRequestMessage>(src, dst);
                var operation = dst.OperationAttributes;
                operation.Add(new IppAttribute(Tag.Uri, "printer-uri", src.PrinterUri.ToString()));
                return dst;
            });
        }
    }
}
