using System.Collections.Generic;
using System.IO;
using System.Text;
using SharpIpp.Exceptions;
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
        /// <param name="stream"></param>
        public void Write(GetJobsRequest request, Stream stream)
        {
            var r = Mapper.Map<IppRequest>(request);
            var operation = r.OperationAttributes;

            operation.Add(new IppAttribute(Tag.Charset, "attributes-charset", "utf-8"));
            operation.Add(new IppAttribute(Tag.NaturalLanguage, "attributes-natural-language", "en"));

            operation.Add(new IppAttribute(Tag.Uri, "printer-uri", request.PrinterUri.ToString()));
            if (request.RequestingUserName != null)
                operation.Add(new IppAttribute(Tag.NameWithoutLanguage, "requesting-user-name", request.RequestingUserName));
            operation.Add(new IppAttribute(Tag.Keyword, "requested-attributes",
                "job-id"));

            r.OperationAttributes.Populate(request.AdditionalOperationAttributes);
            r.JobAttributes.Populate(request.AdditionalJobAttributes);

            using var writer = new BinaryWriter(stream, Encoding.ASCII, true);
            Write(r, writer);
        }

        /// <summary>
        ///     Get-Job-Attributes Response
        ///     https://tools.ietf.org/html/rfc2911#section-3.3.4.2
        /// </summary>
        public GetJobsResponse ReadGetJobs(Stream stream)
        {
            var response = ReadStream(stream);
            if (!response.IsSuccessfulStatusCode)
                throw new IppResponseException($"Job returned error code in Get-Job-Attributes response\n{response}",
                    response);

            var attributes = response.Attributes;
            var printJobResponse = Mapper.Map<GetJobsResponse>(attributes);
            printJobResponse.IppVersion = response.Version;
            printJobResponse.RequestId = response.RequestId;
            return printJobResponse;
        }

        private static void ConfigureGetJobsResponse(SimpleMapper mapper)
        {
            mapper.CreateMap<GetJobsRequest, IppRequest>((src, map) => new IppRequest
            {
                IppOperation = IppOperation.GetJobs,
                IppVersion = src.IppVersion,
                RequestId = src.RequestId
            });

            //https://tools.ietf.org/html/rfc2911#section-4.4
            mapper.CreateMap<IDictionary<string, IppAttribute[]>, GetJobsResponse>((src, map) => new GetJobsResponse
            {
                AllAttributes = src
            });
        }
    }
}