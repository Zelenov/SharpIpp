using System.Collections.Generic;
using System.Linq;
using SharpIpp.Model;
using SharpIpp.Protocol.Extensions;

namespace SharpIpp.Protocol
{
    internal partial class IppProtocol
    {
        /// <summary>
        ///     Get-Job-Attributes Request
        ///     https://tools.ietf.org/html/rfc2911#section-3.3.4.1
        /// </summary>
        /// <param name="request"></param>
        public IppRequestMessage Construct(GetCUPSPrintersRequest request) => ConstructIppRequest(request);

        /// <summary>
        ///     Get-Job-Attributes Response
        ///     https://tools.ietf.org/html/rfc2911#section-3.3.4.2
        /// </summary>
        public GetCUPSPrintersResponse ConstructGetCUPSPrintersResponse(IIppResponseMessage ippResponse) =>
            Construct<GetCUPSPrintersResponse>(ippResponse);

        private static void ConfigureGetCUPSPrintersResponse(SimpleMapper mapper)
        {
            mapper.CreateMap<GetCUPSPrintersRequest, IppRequestMessage>((src, map) =>
            {
                var dst = new IppRequestMessage {IppOperation = IppOperation.GetCUPSPrinters};
                mapper.Map<IIppPrinterRequest, IppRequestMessage>(src, dst);
                var operation = dst.OperationAttributes;
                if (src.Limit != null)
                    operation.Add(new IppAttribute(Tag.Integer, "requesting-user-name", src.Limit.Value));
                if (src.FirstPrinterName != null)
                    operation.Add(new IppAttribute(Tag.Keyword, "first-printer-name", Mapper.Map<string>(src.FirstPrinterName)));
                if (src.PrinterID != null)
                    operation.Add(new IppAttribute(Tag.Integer, "printer-id", Mapper.Map<string>(src.PrinterID)));
                if (src.PrinterLocation != null)
                    operation.Add(new IppAttribute(Tag.Keyword, "printer-location", Mapper.Map<string>(src.PrinterLocation)));
                if (src.RequestedAttributes != null)
                    operation.AddRange(src.RequestedAttributes.Select(requestedAttribute =>
                        new IppAttribute(Tag.Keyword, "requested-attributes", requestedAttribute)));

                dst.OperationAttributes.Populate(src.AdditionalOperationAttributes);
                dst.JobAttributes.Populate(src.AdditionalJobAttributes);
                return dst;
            });
            mapper.CreateMap<IppResponseMessage, GetCUPSPrintersResponse>((src, map) =>
            {
                var dst = new GetCUPSPrintersResponse { Jobs = map.Map<List<IppSection>, JobAttributes[]>(src.Sections)};
                map.Map<IppResponseMessage, IIppResponseMessage>(src, dst);
                return dst;
            });
            //https://tools.ietf.org/html/rfc2911#section-4.4
            //mapper.CreateMap<List<IppSection>, JobAttributes[]>((src, map) =>
            //    src.Where(x => x.Tag == SectionTag.JobAttributesTag)
            //       .Select(x => map.Map<JobAttributes>(x.AllAttributes()))
            //       .ToArray());
        }
    }
}