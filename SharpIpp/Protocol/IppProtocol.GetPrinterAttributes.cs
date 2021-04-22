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
        ///     Get-Printer-Attributes Request
        ///     https://tools.ietf.org/html/rfc2911#section-3.2.5.1
        /// </summary>
        /// <param name="request"></param>
        /// <param name="stream"></param>
        public void Write(GetPrinterAttributesRequest request, Stream stream)
        {
            var r = Mapper.Map<IppRequest>(request);
            var operation = r.OperationAttributes;

            operation.Add(new IppAttribute(Tag.Charset, "attributes-charset", "utf-8"));
            operation.Add(new IppAttribute(Tag.NaturalLanguage, "attributes-natural-language", "en"));
            operation.Add(new IppAttribute(Tag.Uri, "printer-uri", request.PrinterUri.ToString()));

            r.OperationAttributes.Populate(request.AdditionalOperationAttributes);
            r.JobAttributes.Populate(request.AdditionalJobAttributes);
            using var writer = new BinaryWriter(stream, Encoding.ASCII, true);

            Write(r, writer);
        }


        public GetPrinterAttributesResponse ReadGetPrinterAttributes(Stream stream)
        {
            var response = ReadStream(stream);
            if (!response.IsSuccessfulStatusCode)
                throw new IppResponseException(
                    $"Printer returned error code in Get-Printer-Attributes response\n{response}", response);

            var attributes = response.Attributes;
            var printJobResponse = Mapper.Map<GetPrinterAttributesResponse>(attributes);
            return printJobResponse;
        }

        private static void ConfigureGetPrinterAttributesResponse(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<GetPrinterAttributesRequest, IppRequest>()
               .ForMember(dst => dst.IppOperation, opt => opt.MapFrom(_ => IppOperation.GetPrinterAttributes));

            //https://tools.ietf.org/html/rfc2911#section-4.4
            cfg.CreateMap<IDictionary<string, IppAttribute[]>, GetPrinterAttributesResponse>()
              
                .ForMember(dst => dst.AllAttributes, opt => opt.MapFrom(src => src))
               .ForMember(dst => dst.CharsetSupported, opt => opt.MapFromDicSet("charset-supported"))
               .ForMember(dst => dst.PrinterUriSupported, opt => opt.MapFromDicSet("printer-uri-supported"))
               .ForMember(dst => dst.UriSecuritySupported, opt => opt.MapFromDicSet("uri-security-supported"))
               .ForMember(dst => dst.UriAuthenticationSupported,
                    opt => opt.MapFromDicSet("uri-authentication-supported"))
               .ForMember(dst => dst.PrinterName, opt => opt.MapFromDic("printer-name"))
               .ForMember(dst => dst.PrinterLocation, opt => opt.MapFromDic("printer-location"))
               .ForMember(dst => dst.PrinterInfo, opt => opt.MapFromDic("printer-info"))
               .ForMember(dst => dst.PrinterMoreInfo, opt => opt.MapFromDic("printer-more-info"))
               .ForMember(dst => dst.PrinterDriverInstaller, opt => opt.MapFromDic("printer-driver-installer"))
               .ForMember(dst => dst.PrinterMakeAndModel, opt => opt.MapFromDic("printer-make-and-model"))
               .ForMember(dst => dst.PrinterMoreInfoManufacturer,
                    opt => opt.MapFromDic("printer-more-info-manufacturer"))
               .ForMember(dst => dst.PrinterState, opt => opt.MapFromDic("printer-state"))
               .ForMember(dst => dst.PrinterStateReasons, opt => opt.MapFromDicSet("printer-state-reasons"))
               .ForMember(dst => dst.PrinterStateMessage, opt => opt.MapFromDic("printer-state-message"))
               .ForMember(dst => dst.IppVersionsSupported, opt => opt.MapFromDicSet("ipp-versions-supported"))
               .ForMember(dst => dst.OperationsSupported, opt => opt.MapFromDicSet("operations-supported"))
               .ForMember(dst => dst.MultipleDocumentJobsSupported,
                    opt => opt.MapFromDic("multiple-document-jobs-supported"))
               .ForMember(dst => dst.CharsetConfigured, opt => opt.MapFromDic("charset-configured"))
               .ForMember(dst => dst.CharsetSupported, opt => opt.MapFromDicSet("charset-supported"))
               .ForMember(dst => dst.NaturalLanguageConfigured, opt => opt.MapFromDic("natural-language-configured/"))
               .ForMember(dst => dst.GeneratedNaturalLanguageSupported,
                    opt => opt.MapFromDicSet("generated-natural-language-supported"))
               .ForMember(dst => dst.DocumentFormatDefault, opt => opt.MapFromDic("document-format-default"))
               .ForMember(dst => dst.DocumentFormatSupported, opt => opt.MapFromDicSet("document-format-supported"))
               .ForMember(dst => dst.PrinterIsAcceptingJobs, opt => opt.MapFromDic("printer-is-accepting-jobs"))
               .ForMember(dst => dst.QueuedJobCount, opt => opt.MapFromDic("queued-job-count"))
               .ForMember(dst => dst.PrinterMessageFromOperator, opt => opt.MapFromDic("printer-message-from-operator"))
               .ForMember(dst => dst.ColorSupported, opt => opt.MapFromDic("color-supported"))
               .ForMember(dst => dst.ReferenceUriSchemesSupported,
                    opt => opt.MapFromDicSetNull("reference-uri-schemes-supported"))
               .ForMember(dst => dst.PdlOverrideSupported, opt => opt.MapFromDic("pdl-override-supported"))
               .ForMember(dst => dst.PrinterUpTime, opt => opt.MapFromDic("printer-up-time"))
               .ForMember(dst => dst.PrinterCurrentTime, opt => opt.MapFromDic("printer-current-time"))
               .ForMember(dst => dst.MultipleOperationTimeOut, opt => opt.MapFromDic("multiple-operation-time-out/"))
               .ForMember(dst => dst.CompressionSupported, opt => opt.MapFromDicSet("compression-supported"))
               .ForMember(dst => dst.JobKOctetsSupported, opt => opt.MapFromDic("job-k-octets-supported"))
               .ForMember(dst => dst.JobImpressionsSupported, opt => opt.MapFromDic("job-impressions-supported"))
               .ForMember(dst => dst.JobMediaSheetsSupported, opt => opt.MapFromDic("job-media-sheets-supported"))
               .ForMember(dst => dst.PagesPerMinute, opt => opt.MapFromDic("pages-per-minute"))
               .ForMember(dst => dst.PagesPerMinuteColor, opt => opt.MapFromDic("pages-per-minute-color"));
        }
    }
}