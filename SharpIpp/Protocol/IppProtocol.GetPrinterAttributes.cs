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
               .ForIppMemberSet(dst => dst.CharsetSupported, "charset-supported")
               .ForIppMemberSet(dst => dst.PrinterUriSupported, "printer-uri-supported")
               .ForIppMemberSet(dst => dst.UriSecuritySupported, "uri-security-supported")
               .ForIppMemberSet(dst => dst.UriAuthenticationSupported, "uri-authentication-supported")
               .ForIppMember(dst => dst.PrinterName, "printer-name")
               .ForIppMember(dst => dst.PrinterLocation, "printer-location")
               .ForIppMember(dst => dst.PrinterInfo, "printer-info")
               .ForIppMember(dst => dst.PrinterMoreInfo, "printer-more-info")
               .ForIppMember(dst => dst.PrinterDriverInstaller, "printer-driver-installer")
               .ForIppMember(dst => dst.PrinterMakeAndModel, "printer-make-and-model")
               .ForIppMember(dst => dst.PrinterMoreInfoManufacturer, "printer-more-info-manufacturer")
               .ForIppMember(dst => dst.PrinterState, "printer-state")
               .ForIppMemberSet(dst => dst.PrinterStateReasons, "printer-state-reasons")
               .ForIppMember(dst => dst.PrinterStateMessage, "printer-state-message")
               .ForIppMemberSet(dst => dst.IppVersionsSupported, "ipp-versions-supported")
               .ForIppMemberSet(dst => dst.OperationsSupported, "operations-supported")
               .ForIppMember(dst => dst.MultipleDocumentJobsSupported, "multiple-document-jobs-supported")
               .ForIppMember(dst => dst.CharsetConfigured, "charset-configured")
               .ForIppMemberSet(dst => dst.CharsetSupported, "charset-supported")
               .ForIppMember(dst => dst.NaturalLanguageConfigured, "natural-language-configured/")
               .ForIppMemberSet(dst => dst.GeneratedNaturalLanguageSupported, "generated-natural-language-supported")
               .ForIppMember(dst => dst.DocumentFormatDefault, "document-format-default")
               .ForIppMemberSet(dst => dst.DocumentFormatSupported, "document-format-supported")
               .ForIppMember(dst => dst.PrinterIsAcceptingJobs, "printer-is-accepting-jobs")
               .ForIppMember(dst => dst.QueuedJobCount, "queued-job-count")
               .ForIppMember(dst => dst.PrinterMessageFromOperator, "printer-message-from-operator")
               .ForIppMember(dst => dst.ColorSupported, "color-supported")
               .ForIppMemberSetNull(dst => dst.ReferenceUriSchemesSupported, "reference-uri-schemes-supported")
               .ForIppMember(dst => dst.PdlOverrideSupported, "pdl-override-supported")
               .ForIppMember(dst => dst.PrinterUpTime, "printer-up-time")
               .ForIppMember(dst => dst.PrinterCurrentTime, "printer-current-time")
               .ForIppMember(dst => dst.MultipleOperationTimeOut, "multiple-operation-time-out/")
               .ForIppMemberSet(dst => dst.CompressionSupported, "compression-supported")
               .ForIppMember(dst => dst.JobKOctetsSupported, "job-k-octets-supported")
               .ForIppMember(dst => dst.JobImpressionsSupported, "job-impressions-supported")
               .ForIppMember(dst => dst.JobMediaSheetsSupported, "job-media-sheets-supported")
               .ForIppMember(dst => dst.PagesPerMinute, "pages-per-minute")
               .ForIppMember(dst => dst.PagesPerMinuteColor, "pages-per-minute-color");
        }
    }
}