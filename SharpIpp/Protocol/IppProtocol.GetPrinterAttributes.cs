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
        ///     Get-Printer-Attributes Request
        ///     https://tools.ietf.org/html/rfc2911#section-3.2.5.1
        /// </summary>
        /// <param name="request"></param>
        public IppRequestMessage Construct(GetPrinterAttributesRequest request) => ConstructIppRequest(request);

        public GetPrinterAttributesResponse ConstructGetPrinterAttributesResponse(IIppResponseMessage ippResponse) =>
            Construct<GetPrinterAttributesResponse>(ippResponse);

        private static void ConfigureGetPrinterAttributesResponse(SimpleMapper mapper)
        {
            mapper.CreateMap<GetPrinterAttributesRequest, IppRequestMessage>((src, map) =>
            {
                var dst = new IppRequestMessage {IppOperation = IppOperation.GetPrinterAttributes};
                mapper.Map<IIppPrinterRequest, IppRequestMessage>(src, dst);
                var operation = dst.OperationAttributes;
                if (src.RequestedAttributes != null)
                    operation.AddRange(src.RequestedAttributes.Select(requestedAttribute =>
                        new IppAttribute(Tag.Keyword, "requested-attributes", requestedAttribute)));

                dst.OperationAttributes.Populate(src.AdditionalOperationAttributes);
                dst.JobAttributes.Populate(src.AdditionalJobAttributes);
                return dst;
            });

            mapper.CreateMap<IppResponseMessage, GetPrinterAttributesResponse>((src, map) =>
            {
                var dst = map.Map<GetPrinterAttributesResponse>(src.AllAttributes());
                map.Map<IppResponseMessage, IIppResponseMessage>(src, dst);
                return dst;
            });
            //https://tools.ietf.org/html/rfc2911#section-4.4
            mapper.CreateMap<IDictionary<string, IppAttribute[]>, GetPrinterAttributesResponse>((src, map) =>
                new GetPrinterAttributesResponse
                {
                    CharsetConfigured = map.MapFromDic<string?>(src, PrinterAttribute.CharsetConfigured),
                    CharsetSupported = map.MapFromDicSetNull<string[]?>(src, PrinterAttribute.CharsetSupported),
                    ColorSupported = map.MapFromDic<bool?>(src, PrinterAttribute.ColorSupported),
                    CompressionSupported =
                        map.MapFromDicSetNull<Compression[]?>(src, PrinterAttribute.CompressionSupported),
                    DocumentFormatDefault = map.MapFromDic<string?>(src, PrinterAttribute.DocumentFormatDefault),
                    DocumentFormatSupported =
                        map.MapFromDicSetNull<string[]?>(src, PrinterAttribute.DocumentFormatSupported),
                    GeneratedNaturalLanguageSupported =
                        map.MapFromDicSetNull<string[]?>(src, PrinterAttribute.GeneratedNaturalLanguageSupported),
                    IppVersionsSupported =
                        map.MapFromDicSetNull<string[]?>(src, PrinterAttribute.IppVersionsSupported),
                    JobImpressionsSupported = map.MapFromDic<Range?>(src, PrinterAttribute.JobImpressionsSupported),
                    JobKOctetsSupported = map.MapFromDic<Range?>(src, PrinterAttribute.JobKOctetsSupported),
                    JobMediaSheetsSupported = map.MapFromDic<Range?>(src, PrinterAttribute.JobMediaSheetsSupported),
                    MultipleDocumentJobsSupported =
                        map.MapFromDic<bool?>(src, PrinterAttribute.MultipleDocumentJobsSupported),
                    MultipleOperationTimeOut = map.MapFromDic<int?>(src, PrinterAttribute.MultipleOperationTimeOut),
                    NaturalLanguageConfigured =
                        map.MapFromDic<string?>(src, PrinterAttribute.NaturalLanguageConfigured),
                    OperationsSupported =
                        map.MapFromDicSetNull<IppOperation[]?>(src, PrinterAttribute.OperationsSupported),
                    PagesPerMinute = map.MapFromDic<int?>(src, PrinterAttribute.PagesPerMinute),
                    PdlOverrideSupported = map.MapFromDic<string?>(src, PrinterAttribute.PdlOverrideSupported),
                    PagesPerMinuteColor = map.MapFromDic<int?>(src, PrinterAttribute.PagesPerMinuteColor),
                    PrinterCurrentTime = map.MapFromDic<DateTimeOffset?>(src, PrinterAttribute.PrinterCurrentTime),
                    PrinterDriverInstaller = map.MapFromDic<string?>(src, PrinterAttribute.PrinterDriverInstaller),
                    PrinterInfo = map.MapFromDic<string?>(src, PrinterAttribute.PrinterInfo),
                    PrinterIsAcceptingJobs = map.MapFromDic<bool?>(src, PrinterAttribute.PrinterIsAcceptingJobs),
                    PrinterLocation = map.MapFromDic<string?>(src, PrinterAttribute.PrinterLocation),
                    PrinterMakeAndModel = map.MapFromDic<string?>(src, PrinterAttribute.PrinterMakeAndModel),
                    PrinterMessageFromOperator =
                        map.MapFromDic<string?>(src, PrinterAttribute.PrinterMessageFromOperator),
                    PrinterMoreInfo = map.MapFromDic<string?>(src, PrinterAttribute.PrinterMoreInfo),
                    PrinterMoreInfoManufacturer =
                        map.MapFromDic<string?>(src, PrinterAttribute.PrinterMoreInfoManufacturer),
                    PrinterName = map.MapFromDic<string?>(src, PrinterAttribute.PrinterName),
                    PrinterState = map.MapFromDic<PrinterState?>(src, PrinterAttribute.PrinterState),
                    PrinterStateMessage = map.MapFromDic<string?>(src, PrinterAttribute.PrinterStateMessage),
                    PrinterStateReasons =
                        map.MapFromDicSetNull<string[]?>(src, PrinterAttribute.PrinterStateReasons),
                    PrinterUpTime = map.MapFromDic<int?>(src, PrinterAttribute.PrinterUpTime),
                    PrinterUriSupported =
                        map.MapFromDicSetNull<string[]?>(src, PrinterAttribute.PrinterUriSupported),
                    PrintScalingDefault = map.MapFromDic<PrintScaling?>(src, PrinterAttribute.PrintScalingDefault),
                    PrintScalingSupported =
                        map.MapFromDicSetNull<PrintScaling[]?>(src, PrinterAttribute.PrintScalingSupported),
                    QueuedJobCount = map.MapFromDic<int?>(src, PrinterAttribute.QueuedJobCount),
                    ReferenceUriSchemesSupported =
                        map.MapFromDicSetNull<string[]?>(src, PrinterAttribute.ReferenceUriSchemesSupported),
                    UriAuthenticationSupported =
                        map.MapFromDicSetNull<string[]?>(src, PrinterAttribute.UriAuthenticationSupported),
                    UriSecuritySupported =
                        map.MapFromDicSetNull<string[]?>(src, PrinterAttribute.UriSecuritySupported)
                });
        }
    }
}