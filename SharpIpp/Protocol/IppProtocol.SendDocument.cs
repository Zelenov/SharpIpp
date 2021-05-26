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
        public IppRequestMessage Construct(SendDocumentRequest request)
        {
            if (request.Document == null && !request.LastDocument)
                throw new ArgumentException($"{nameof(request.Document)} must be set for non-last document");

            return ConstructIppRequest(request);
        }

        public SendDocumentResponse ConstructSendDocumentResponse(IIppResponseMessage ippResponse) =>
            Construct<SendDocumentResponse>(ippResponse);


        private static void ConfigureSendDocumentRequest(SimpleMapper mapper)
        {
            mapper.CreateMap<SendDocumentRequest, IppRequestMessage>((src, map) =>
            {
                var dst = new IppRequestMessage
                {
                    IppOperation = IppOperation.SendDocument, Document = src.LastDocument ? null : src.Document
                };
                mapper.Map<IIppJobRequest, IppRequestMessage>(src, dst);
                var operation = dst.OperationAttributes;
                operation.Add(new IppAttribute(Tag.Boolean, "last-document", src.LastDocument));
                if (src.DocumentAttributes != null)
                    map.Map(src.DocumentAttributes, dst);
                return dst;
            });

            mapper.CreateMap<IppResponseMessage, SendDocumentResponse>((src, map) =>
            {
                var dst = new SendDocumentResponse();
                map.Map<IppResponseMessage, IIppJobResponse>(src, dst);
                return dst;
            });
        }
    }
}