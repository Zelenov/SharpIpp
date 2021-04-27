using System;
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
            printJobResponse.IppVersion = response.Version;
            printJobResponse.RequestId = response.RequestId;
            return printJobResponse;
        }

        private static void ConfigureGetJobAttributesResponse(SimpleMapper mapper)
        {
            mapper.CreateMap<GetJobAttributesRequest, IppRequest>((src, map) => new IppRequest
            {
                IppOperation = IppOperation.GetJobAttributes,
                IppVersion = src.IppVersion,
                RequestId = src.RequestId
            });

            //https://tools.ietf.org/html/rfc2911#section-4.4
            mapper.CreateMap<IDictionary<string, IppAttribute[]>, GetJobAttributesResponse>((src, map) =>
                new GetJobAttributesResponse
                {
                    AllAttributes = src,
                    Compression = map.MapFromDic<Compression?>(src, "compression"),
                    Copies = map.MapFromDic<int?>(src, "copies"),
                    DateTimeAtCompleted = map.MapFromDic<DateTimeOffset?>(src, "date-time-at-completed"),
                    DateTimeAtCreation = map.MapFromDic<DateTimeOffset?>(src, "date-time-at-creation"),
                    DateTimeAtProcessing = map.MapFromDic<DateTimeOffset?>(src, "date-time-at-processing"),
                    DocumentFormat = map.MapFromDic<string?>(src, "document-format"),
                    DocumentName = map.MapFromDic<string?>(src, "document-name"),
                    Finishings = map.MapFromDic<Finishings?>(src, "finishings"),
                    IppAttributeFidelity = map.MapFromDic<bool?>(src, "ipp-attribute-fidelity"),
                    JobId = map.MapFromDic<int?>(src, "job-id"),
                    JobImpressions = map.MapFromDic <int?> (src, "job-impressions"),
                    JobImpressionsCompleted = map.MapFromDic<int?>(src, "job-impressions-completed"),
                    JobKOctetsProcessed = map.MapFromDic<int?>(src, "job-k-octets-processed"),
                    JobMediaSheets = map.MapFromDic<int?>(src, "job-media-sheets"),
                    JobMediaSheetsCompleted = map.MapFromDic<int?>(src, "job-media-sheets-completed"),
                    JobName = map.MapFromDic<string?>(src, "job-name"),
                    JobOriginatingUserName = map.MapFromDic<StringWithLanguage?>(src, "job-originating-user-name"),
                    JobPrinterUpTime = map.MapFromDic<int?>(src, "job-printer-up-time"),
                    JobPrinterUri = map.MapFromDic<string?>(src, "job-printer-uri"),
                    JobSheets = map.MapFromDic<JobSheets?>(src, "job-sheets"),
                    JobState = map.MapFromDic<JobState?>(src, "job-state"),
                    JobStateMessage = map.MapFromDic<string?>(src, "job-state-message"),
                    JobStateReasons = map.MapFromDicSetNull<string[]?>(src, "job-state-reasons"),
                    Media = map.MapFromDic<string?>(src, "media"),
                    MultipleDocumentHandling = map.MapFromDic<MultipleDocumentHandling?>(src, "multiple-document-handling"),
                    NumberUp = map.MapFromDic<int?>(src, "number-up"),
                    OrientationRequested = map.MapFromDic<Orientation?>(src, "orientation-requested"),
                    PrinterResolution = map.MapFromDic<Resolution?>(src, "printer-resolution"),
                    PrintQuality = map.MapFromDic<PrintQuality?>(src, "print-quality"),
                    Sides = map.MapFromDic<Sides?>(src, "sides"),
                    TimeAtCompleted = map.MapFromDic<int?>(src, "time-at-completed"),
                    TimeAtCreation = map.MapFromDic<int?>(src, "time-at-creation"),
                    TimeAtProcessing = map.MapFromDic<int?>(src, "time-at-processing")
                });
        }
    }
}