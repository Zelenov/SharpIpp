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

            operation.Add(new IppAttribute(Tag.NameWithoutLanguage, "requesting-user-name",request.RequestingUserName));
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
               .ForIppMember(dst => dst.JobId, "job-id")
               .ForIppMember(dst => dst.JobPrinterUri, "job-printer-uri")
               .ForIppMember(dst => dst.JobName, "job-name")
               .ForIppMember(dst => dst.JobOriginatingUserName, "job-originating-user-name")
               .ForIppMember(dst => dst.JobSheets, "job-sheets")
               .ForIppMember(dst => dst.Copies, "copies")
               .ForIppMember(dst => dst.MultipleDocumentHandling, "multiple-document-handling")
               .ForIppMember(dst => dst.PrintQuality, "print-quality")
               .ForIppMember(dst => dst.PrinterResolution, "printer-resolution")
               .ForIppMember(dst => dst.Sides, "sides")
               .ForIppMember(dst => dst.Media, "media")
               .ForIppMember(dst => dst.NumberUp, "number-up")
               .ForIppMember(dst => dst.OrientationRequested, "orientation-requested")
               .ForIppMember(dst => dst.Finishings, "finishings")
               .ForIppMember(dst => dst.JobKOctetsProcessed, "job-k-octets-processed")
               .ForIppMember(dst => dst.JobImpressions, "job-impressions")
               .ForIppMember(dst => dst.JobImpressionsCompleted, "job-impressions-completed")
               .ForIppMember(dst => dst.JobMediaSheets, "job-media-sheets")
               .ForIppMember(dst => dst.JobMediaSheetsCompleted, "job-media-sheets-completed")
               .ForIppMember(dst => dst.JobState, "job-state")
               .ForIppMember(dst => dst.Compression, "compression")
               .ForIppMember(dst => dst.DocumentFormat, "document-format")
               .ForIppMember(dst => dst.DocumentName, "document-name")
               .ForIppMember(dst => dst.IppAttributeFidelity, "ipp-attribute-fidelity")
               .ForIppMember(dst => dst.JobStateMessage, "job-state-message")
               .ForIppMember(dst => dst.JobStateReasons, "job-state-reasons")
               .ForIppMember(dst => dst.DateTimeAtCreation, "date-time-at-creation")
               .ForIppMember(dst => dst.DateTimeAtProcessing, "date-time-at-processing")
               .ForIppMember(dst => dst.DateTimeAtCompleted, "date-time-at-completed")
               .ForIppMember(dst => dst.TimeAtCreation, "time-at-creation")
               .ForIppMember(dst => dst.TimeAtProcessing, "time-at-processing")
               .ForIppMember(dst => dst.TimeAtCompleted, "time-at-completed")
               .ForIppMember(dst => dst.JobPrinterUpTime, "job-printer-up-time");
        }
    }
}