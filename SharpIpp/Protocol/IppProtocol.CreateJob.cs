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
        public IppRequestMessage Construct(CreateJobRequest request) => ConstructIppRequest(request);

        public CreateJobResponse ConstructCreateJobResponse(IIppResponseMessage ippResponse) =>
            Construct<CreateJobResponse>(ippResponse);

        private static void ConfigureCreateJobRequest(SimpleMapper mapper)
        {
            mapper.CreateMap<CreateJobRequest, IppRequestMessage>((src, map) =>
            {
                var dst = new IppRequestMessage {IppOperation = IppOperation.CreateJob};
                mapper.Map<IIppPrinterRequest, IppRequestMessage>(src, dst);
                if (src.NewJobAttributes != null)
                    map.Map(src.NewJobAttributes, dst);
                return dst;
            });

            mapper.CreateMap<IppResponseMessage, CreateJobResponse>((src, map) =>
            {
                var dst = new CreateJobResponse();
                map.Map<IppResponseMessage, IIppJobResponse>(src, dst);
                return dst;
            });
        }
    }
}