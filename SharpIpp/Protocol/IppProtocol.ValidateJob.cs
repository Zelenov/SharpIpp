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
        public IppRequestMessage Construct(ValidateJobRequest request)
        {
            if (request.Document == null)
                throw new ArgumentException($"{nameof(request.Document)} must be set");

            return ConstructIppRequest(request);
        }

        public ValidateJobResponse ConstructValidateJobResponse(IIppResponseMessage ippResponse) =>
            Construct<ValidateJobResponse>(ippResponse);

        private static void ConfigureValidateJobRequest(SimpleMapper mapper)
        {
            mapper.CreateMap<ValidateJobRequest, IppRequestMessage>((src, map) =>
            {
                var dst = new IppRequestMessage {IppOperation = IppOperation.ValidateJob, Document = src.Document};
                mapper.Map<IIppPrinterRequest, IppRequestMessage>(src, dst);
                if (src.NewJobAttributes != null)
                    map.Map(src.NewJobAttributes, dst);
                if (src.DocumentAttributes != null)
                    map.Map(src.DocumentAttributes, dst);
                return dst;
            });

            mapper.CreateMap<IppResponseMessage, ValidateJobResponse>((src, map) =>
            {
                var dst = new ValidateJobResponse();
                map.Map<IppResponseMessage, IIppResponseMessage>(src, dst);
                return dst;
            });
        }
    }
}