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
        public IppRequestMessage Construct(PrintJobRequest request)
        {
            if (request.Document == null)
                throw new ArgumentException($"{nameof(request.Document)} must be set");

            return ConstructIppRequest(request);
        }

        public PrintJobResponse ConstructPrintJobResponse(IIppResponseMessage ippResponse) =>
            Construct<PrintJobResponse>(ippResponse);


        private static void ConfigurePrintJobRequest(SimpleMapper mapper)
        {
            mapper.CreateMap<PrintJobRequest, IppRequestMessage>((src, map) =>
            {
                var dst = new IppRequestMessage {IppOperation = IppOperation.PrintJob, Document = src.Document};
                mapper.Map<IIppPrinterRequest, IppRequestMessage>(src, dst);
                if (src.NewJobAttributes != null)
                    map.Map(src.NewJobAttributes, dst);
                if (src.DocumentAttributes != null)
                    map.Map(src.DocumentAttributes, dst);
                return dst;
            });

            mapper.CreateMap<IppResponseMessage, PrintJobResponse>((src, map) =>
            {
                var dst = new PrintJobResponse();
                map.Map<IppResponseMessage, IIppJobResponse>(src, dst);
                return dst;
            });
        }
    }
}