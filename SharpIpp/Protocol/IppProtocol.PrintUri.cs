using System;
using SharpIpp.Model;

namespace SharpIpp.Protocol
{
    internal partial class IppProtocol
    {
        /// <summary>
        ///     Print-Job Request
        ///     https://tools.ietf.org/html/rfc2911#section-3.2.1.1
        /// </summary>
        /// <param name="request"></param>
        public IppRequestMessage Construct(PrintUriRequest request)
        {
            if (request.DocumentUri == null)
                throw new ArgumentException($"{nameof(request.DocumentUri)} must be set");

            return ConstructIppRequest(request);
        }

        public PrintUriResponse ConstructPrintUriResponse(IIppResponseMessage ippResponse) =>
            Construct<PrintUriResponse>(ippResponse);

        private static void ConfigurePrintUriRequest(SimpleMapper mapper)
        {
            mapper.CreateMap<PrintUriRequest, IppRequestMessage>((src, map) =>
            {
                var dst = new IppRequestMessage {IppOperation = IppOperation.PrintUri};
                mapper.Map<IIppPrinterRequest, IppRequestMessage>(src, dst);
                var operation = dst.OperationAttributes;
                operation.Add(new IppAttribute(Tag.Uri, "document-uri", src.DocumentUri.ToString()));
                if (src.NewJobAttributes != null)
                    map.Map(src.NewJobAttributes, dst);
                if (src.DocumentAttributes != null)
                    map.Map(src.DocumentAttributes, dst);
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