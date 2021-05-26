using System;
using System.Collections.Generic;
using System.Linq;
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
        public IppRequestMessage Construct(GetJobAttributesRequest request) => ConstructIppRequest(request);

        /// <summary>
        ///     Get-Job-Attributes Response
        ///     https://tools.ietf.org/html/rfc2911#section-3.3.4.2
        /// </summary>
        public GetJobAttributesResponse ConstructGetJobAttributesResponse(IIppResponseMessage ippResponse) =>
            Construct<GetJobAttributesResponse>(ippResponse);

        private static void ConfigureGetJobAttributesResponse(SimpleMapper mapper)
        {
            mapper.CreateMap<GetJobAttributesRequest, IppRequestMessage>((src, map) =>
            {
                var dst = new IppRequestMessage {IppOperation = IppOperation.GetJobAttributes};
                mapper.Map<IIppJobRequest, IppRequestMessage>(src, dst);
                var operation = dst.OperationAttributes;
                if (src.RequestedAttributes != null)
                    operation.AddRange(src.RequestedAttributes.Select(requestedAttribute =>
                        new IppAttribute(Tag.Keyword, "requested-attributes", requestedAttribute)));
                dst.OperationAttributes.Populate(src.AdditionalOperationAttributes);
                dst.JobAttributes.Populate(src.AdditionalJobAttributes);
                return dst;
            });

            //https://tools.ietf.org/html/rfc2911#section-4.4
            mapper.CreateMap<IppResponseMessage, GetJobAttributesResponse>((src, map) =>
            {
                var dst = new GetJobAttributesResponse {JobAttributes = map.Map<JobAttributes>(src.AllAttributes())};
                map.Map<IppResponseMessage, IIppResponseMessage>(src, dst);
                return dst;
            });

            mapper.CreateMap<IDictionary<string, IppAttribute[]>, JobAttributes>((src, map) => new JobAttributes
            {
                Compression = map.MapFromDic<Compression?>(src, JobAttribute.Compression),
                Copies = map.MapFromDic<int?>(src, JobAttribute.Copies),
                DateTimeAtCompleted = map.MapFromDic<DateTimeOffset?>(src, JobAttribute.DateTimeAtCompleted),
                DateTimeAtCreation = map.MapFromDic<DateTimeOffset?>(src, JobAttribute.DateTimeAtCreation),
                DateTimeAtProcessing = map.MapFromDic<DateTimeOffset?>(src, JobAttribute.DateTimeAtProcessing),
                DocumentFormat = map.MapFromDic<string?>(src, JobAttribute.DocumentFormat),
                DocumentName = map.MapFromDic<string?>(src, JobAttribute.DocumentName),
                Finishings = map.MapFromDic<Finishings?>(src, JobAttribute.Finishings),
                IppAttributeFidelity = map.MapFromDic<bool?>(src, JobAttribute.IppAttributeFidelity),
                JobId = map.MapFromDic<int?>(src, JobAttribute.JobId),
                JobUri = map.MapFromDic<string?>(src, JobAttribute.JobUri),
                JobImpressions = map.MapFromDic<int?>(src, JobAttribute.JobImpressions),
                JobImpressionsCompleted = map.MapFromDic<int?>(src, JobAttribute.JobImpressionsCompleted),
                JobKOctetsProcessed = map.MapFromDic<int?>(src, JobAttribute.JobKOctetsProcessed),
                JobMediaSheets = map.MapFromDic<int?>(src, JobAttribute.JobMediaSheets),
                JobMediaSheetsCompleted = map.MapFromDic<int?>(src, JobAttribute.JobMediaSheetsCompleted),
                JobName = map.MapFromDic<string?>(src, JobAttribute.JobName),
                JobOriginatingUserName = map.MapFromDic<string?>(src, JobAttribute.JobOriginatingUserName),
                JobOriginatingUserNameLanguage =
                    map.MapFromDicLanguage(src, JobAttribute.JobOriginatingUserNameLanguage),
                JobPrinterUpTime = map.MapFromDic<DateTime?>(src, JobAttribute.JobPrinterUpTime),
                JobPrinterUri = map.MapFromDic<string?>(src, JobAttribute.JobPrinterUri),
                JobSheets = map.MapFromDic<JobSheets?>(src, JobAttribute.JobSheets),
                JobState = map.MapFromDic<JobState?>(src, JobAttribute.JobState),
                JobStateMessage = map.MapFromDic<string?>(src, JobAttribute.JobStateMessage),
                JobStateReasons = map.MapFromDicSetNull<string[]?>(src, JobAttribute.JobStateReasons),
                Media = map.MapFromDic<string?>(src, JobAttribute.Media),
                MultipleDocumentHandling =
                    map.MapFromDic<MultipleDocumentHandling?>(src, JobAttribute.MultipleDocumentHandling),
                NumberUp = map.MapFromDic<int?>(src, JobAttribute.NumberUp),
                OrientationRequested = map.MapFromDic<Orientation?>(src, JobAttribute.OrientationRequested),
                PrinterResolution = map.MapFromDic<Resolution?>(src, JobAttribute.PrinterResolution),
                PrintQuality = map.MapFromDic<PrintQuality?>(src, JobAttribute.PrintQuality),
                Sides = map.MapFromDic<Sides?>(src, JobAttribute.Sides),
                TimeAtCompleted = map.MapFromDic<DateTime?>(src, JobAttribute.TimeAtCompleted),
                TimeAtCreation = map.MapFromDic<DateTime?>(src, JobAttribute.TimeAtCreation),
                TimeAtProcessing = map.MapFromDic<DateTime?>(src, JobAttribute.TimeAtProcessing)
            });
        }
    }
}