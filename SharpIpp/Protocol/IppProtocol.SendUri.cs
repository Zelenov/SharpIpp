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
        public IppRequestMessage Construct(SendUriRequest request)
        {
            if (request.DocumentUri == null && !request.LastDocument)
                throw new ArgumentException($"{nameof(request.DocumentUri)} must be set for non-last document");

            return ConstructIppRequest(request);
        }

        public SendUriResponse ConstructSendUriResponse(IIppResponseMessage ippResponse) =>
            Construct<SendUriResponse>(ippResponse);

        private static void ConfigureSendUriRequest(SimpleMapper mapper)
        {
            mapper.CreateMap<SendUriRequest, IppRequestMessage>((src, map) =>
            {
                var dst = new IppRequestMessage {IppOperation = IppOperation.SendUri};
                mapper.Map<IIppJobRequest, IppRequestMessage>(src, dst);
                var operation = dst.OperationAttributes;
                operation.Add(new IppAttribute(Tag.Boolean, "last-document", src.LastDocument));
                if (src.DocumentUri != null && !src.LastDocument)
                    operation.Add(new IppAttribute(Tag.Uri, "document-uri", src.DocumentUri.ToString()));
                if (src.DocumentAttributes != null)
                    map.Map(src.DocumentAttributes, dst);
                return dst;
            });

            mapper.CreateMap<IppResponseMessage, SendUriResponse>((src, map) =>
            {
                var dst = new SendUriResponse();
                map.Map<IppResponseMessage, IIppJobResponse>(src, dst);
                return dst;
            });
        }
    }
}