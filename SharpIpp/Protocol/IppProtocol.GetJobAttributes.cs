using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AutoMapper;
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
        public void Write(GetJobAttributesRequest request, Stream stream)
        {
            var r = Mapper.Map<IppRequest>(request);
            var operation = r.OperationAttributes;

            operation.Add(new IppAttribute(Tag.Charset, "attributes-charset", "utf-8"));
            operation.Add(new IppAttribute(Tag.NaturalLanguage, "attributes-natural-language", "en"));
            if (request.JobUrl != null)
            {
                operation.Add(new IppAttribute(Tag.Uri, "job-uri", request.JobUrl.ToString()));
            }
            else if (request.JobId != null)
            {
                operation.Add(new IppAttribute(Tag.Uri, "printer-uri", request.PrinterUri.ToString()));
                operation.Add(new IppAttribute(Tag.Integer, "job-id", request.JobId.Value));
            }
            else
            {
                throw new ArgumentException(
                    $"JobTarget must have {nameof(GetJobAttributesRequest.JobUrl)} or {nameof(GetJobAttributesRequest.JobId)} set");
            }

            operation.Add(new IppAttribute(Tag.NameWithoutLanguage, "requesting-user-name",
                request.RequestingUserName));
            r.OperationAttributes.Populate(request.AdditionalOperationAttributes);
            r.JobAttributes.Populate(request.AdditionalJobAttributes);

            using var writer = new BinaryWriter(stream, Encoding.ASCII, true);
            Write(r, writer);
        }

        /// <summary>
        ///     Get-Job-Attributes Response
        ///     https://tools.ietf.org/html/rfc2911#section-3.3.4.2
        /// </summary>
        public GetJobAttributesResponse ReadGetJobAttributes(Stream stream)
        {
            var response = ReadStream(stream);
            if (!response.IsSuccessfulStatusCode)
                throw new IppResponseException($"Job returned error code in Get-Job-Attributes response\n{response}",
                    response);

            var attributes = response.Attributes;
            var printJobResponse = Mapper.Map<GetJobAttributesResponse>(attributes);
            return printJobResponse;
        }

        private static void ConfigureGetJobAttributesResponse(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<GetJobAttributesRequest, IppRequest>()
               .ForMember(dst => dst.IppOperation, opt => opt.MapFrom(_ => IppOperation.GetJobAttributes));

            //https://tools.ietf.org/html/rfc2911#section-4.4
            cfg.CreateMap<IDictionary<string, IppAttribute[]>, GetJobAttributesResponse>()
               .ForMember(dst => dst.AllAttributes, opt => opt.MapFrom(src => src))
               .ForMember(dst => dst.JobId, opt => opt.MapFromDic("job-id"))
               .ForMember(dst => dst.JobPrinterUri, opt => opt.MapFromDic("job-printer-uri"))
               .ForMember(dst => dst.JobName, opt => opt.MapFromDic("job-name"))
               .ForMember(dst => dst.JobOriginatingUserName, opt => opt.MapFromDic("job-originating-user-name"))
               .ForMember(dst => dst.JobSheets, opt => opt.MapFromDic("job-sheets"))
               .ForMember(dst => dst.Copies, opt => opt.MapFromDic("copies"))
               .ForMember(dst => dst.MultipleDocumentHandling, opt => opt.MapFromDic("multiple-document-handling"))
               .ForMember(dst => dst.PrintQuality, opt => opt.MapFromDic("print-quality"))
               .ForMember(dst => dst.PrinterResolution, opt => opt.MapFromDic("printer-resolution"))
               .ForMember(dst => dst.Sides, opt => opt.MapFromDic("sides"))
               .ForMember(dst => dst.Media, opt => opt.MapFromDic("media"))
               .ForMember(dst => dst.NumberUp, opt => opt.MapFromDic("number-up"))
               .ForMember(dst => dst.OrientationRequested, opt => opt.MapFromDic("orientation-requested"))
               .ForMember(dst => dst.Finishings, opt => opt.MapFromDic("finishings"))
               .ForMember(dst => dst.JobKOctetsProcessed, opt => opt.MapFromDic("job-k-octets-processed"))
               .ForMember(dst => dst.JobImpressions, opt => opt.MapFromDic("job-impressions"))
               .ForMember(dst => dst.JobImpressionsCompleted, opt => opt.MapFromDic("job-impressions-completed"))
               .ForMember(dst => dst.JobMediaSheets, opt => opt.MapFromDic("job-media-sheets"))
               .ForMember(dst => dst.JobMediaSheetsCompleted, opt => opt.MapFromDic("job-media-sheets-completed"))
               .ForMember(dst => dst.JobState, opt => opt.MapFromDic("job-state"))
               .ForMember(dst => dst.Compression, opt => opt.MapFromDic("compression"))
               .ForMember(dst => dst.DocumentFormat, opt => opt.MapFromDic("document-format"))
               .ForMember(dst => dst.DocumentName, opt => opt.MapFromDic("document-name"))
               .ForMember(dst => dst.IppAttributeFidelity, opt => opt.MapFromDic("ipp-attribute-fidelity"))
               .ForMember(dst => dst.JobStateMessage, opt => opt.MapFromDic("job-state-message"))
               .ForMember(dst => dst.JobStateReasons, opt => opt.MapFromDic("job-state-reasons"))
               .ForMember(dst => dst.DateTimeAtCreation, opt => opt.MapFromDic("date-time-at-creation"))
               .ForMember(dst => dst.DateTimeAtProcessing, opt => opt.MapFromDic("date-time-at-processing"))
               .ForMember(dst => dst.DateTimeAtCompleted, opt => opt.MapFromDic("date-time-at-completed"))
               .ForMember(dst => dst.TimeAtCreation, opt => opt.MapFromDic("time-at-creation"))
               .ForMember(dst => dst.TimeAtProcessing, opt => opt.MapFromDic("time-at-processing"))
               .ForMember(dst => dst.TimeAtCompleted, opt => opt.MapFromDic("time-at-completed"))
               .ForMember(dst => dst.JobPrinterUpTime, opt => opt.MapFromDic("job-printer-up-time"));
        }
    }
}