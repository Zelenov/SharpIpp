using System;

using SharpIpp.Models;
using SharpIpp.Protocol.Models;

namespace SharpIpp.Mapping.Profiles
{
    // ReSharper disable once UnusedMember.Global
    internal class PrintUriProfile : IProfile
    {
        public void CreateMaps(IMapperConstructor mapper)
        {
            mapper.CreateMap<PrintUriRequest, IppRequestMessage>((src, map) =>
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (src.DocumentUri == null)
                {
                    throw new ArgumentException($"{nameof(src.DocumentUri)} must be set");
                }

                var dst = new IppRequestMessage { IppOperation = IppOperation.PrintUri };
                map.Map<IIppPrinterRequest, IppRequestMessage>(src, dst);
                var operation = dst.OperationAttributes;
                operation.Add(new IppAttribute(Tag.Uri, "document-uri", src.DocumentUri.ToString()));

                if (src.NewJobAttributes != null)
                {
                    map.Map(src.NewJobAttributes, dst);
                }

                if (src.DocumentAttributes != null)
                {
                    map.Map(src.DocumentAttributes, dst);
                }

                return dst;
            });

            mapper.CreateMap<IppResponseMessage, PrintUriResponse>((src, map) =>
            {
                var dst = new PrintUriResponse();
                map.Map<IppResponseMessage, IIppJobResponse>(src, dst);
                return dst;
            });
        }
    }
}
