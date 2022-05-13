using System.Collections.Generic;
using System.Linq;

using SharpIpp.Models;
using SharpIpp.Protocol;
using SharpIpp.Protocol.Extensions;
using SharpIpp.Protocol.Models;

namespace SharpIpp.Mapping.Profiles
{
    // ReSharper disable once UnusedMember.Global
    internal class GetCUPSPrintersProfile : IProfile
    {
        public void CreateMaps(IMapperConstructor mapper)
        {
            mapper.CreateMap<CUPSGetPrintersRequest, IppRequestMessage>((src, map) =>
            {
                var dst = new IppRequestMessage { IppOperation = IppOperation.GetCUPSPrinters };
                map.Map<IIppPrinterRequest, IppRequestMessage>(src, dst);
                var operation = dst.OperationAttributes;

                if (src.Limit != null)
                {
                    operation.Add(new IppAttribute(Tag.Integer, "requesting-user-name", src.Limit.Value));
                }

                if (src.FirstPrinterName != null)
                {
                    operation.Add(new IppAttribute(Tag.Keyword, "first-printer-name", map.Map<string>(src.FirstPrinterName)));
                }

                if (src.PrinterId != null)
                {
                    operation.Add(new IppAttribute(Tag.Integer, "printer-id", map.Map<string>(src.PrinterId)));
                }

                if (src.PrinterLocation != null)
                {
                    operation.Add(new IppAttribute(Tag.Keyword, "printer-location", map.Map<string>(src.PrinterLocation)));
                }

                if (src.PrinterType != null)
                {
                    operation.Add(new IppAttribute(Tag.Enum, "printer-type", (int)src.PrinterType.Value));
                }

                if (src.PrinterTypeMask != null)
                {
                    operation.Add(new IppAttribute(Tag.Enum, "printer-type-mask", (int)src.PrinterTypeMask.Value));
                }

                if (src.RequestedAttributes != null)
                {
                    operation.AddRange(src.RequestedAttributes.Select(requestedAttribute =>
                        new IppAttribute(Tag.Keyword, "requested-attributes", requestedAttribute)));
                }

                dst.OperationAttributes.Populate(src.AdditionalOperationAttributes);
                dst.JobAttributes.Populate(src.AdditionalJobAttributes);
                return dst;
            });

            mapper.CreateMap<IppResponseMessage, CUPSGetPrintersResponse>((src, map) =>
            {
                var dst = new CUPSGetPrintersResponse { Jobs = map.Map<List<IppSection>, JobAttributes[]>(src.Sections) };
                map.Map<IppResponseMessage, IIppResponseMessage>(src, dst);
                return dst;
            });
        }
    }
}
