using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            if (request.RequestedAttributes != null)
                operation.AddRange(request.RequestedAttributes.Select(requestedAttribute =>
                    new IppAttribute(Tag.Keyword, "requested-attributes", requestedAttribute)));

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
               .ForIppMemberSetNull(dst => dst.CharsetSupported, PrinterAttribute.CharsetSupported)
               .ForIppMemberSetNull(dst => dst.PrinterUriSupported, PrinterAttribute.PrinterUriSupported)
               .ForIppMemberSetNull(dst => dst.UriSecuritySupported, PrinterAttribute.UriSecuritySupported)
               .ForIppMemberSetNull(dst => dst.UriAuthenticationSupported, PrinterAttribute.UriAuthenticationSupported)
               .ForIppMember(dst => dst.PrinterName, PrinterAttribute.PrinterName)
               .ForIppMember(dst => dst.PrinterLocation, PrinterAttribute.PrinterLocation)
               .ForIppMember(dst => dst.PrinterInfo, PrinterAttribute.PrinterInfo)
               .ForIppMember(dst => dst.PrinterMoreInfo, PrinterAttribute.PrinterMoreInfo)
               .ForIppMember(dst => dst.PrinterDriverInstaller, PrinterAttribute.PrinterDriverInstaller)
               .ForIppMember(dst => dst.PrinterMakeAndModel, PrinterAttribute.PrinterMakeAndModel)
               .ForIppMember(dst => dst.PrinterMoreInfoManufacturer, PrinterAttribute.PrinterMoreInfoManufacturer)
               .ForIppMember(dst => dst.PrinterState, PrinterAttribute.PrinterState)
               .ForIppMemberSetNull(dst => dst.PrinterStateReasons, PrinterAttribute.PrinterStateReasons)
               .ForIppMember(dst => dst.PrinterStateMessage, PrinterAttribute.PrinterStateMessage)
               .ForIppMemberSetNull(dst => dst.IppVersionsSupported, PrinterAttribute.IppVersionsSupported)
               .ForIppMemberSetNull(dst => dst.OperationsSupported, PrinterAttribute.OperationsSupported)
               .ForIppMember(dst => dst.MultipleDocumentJobsSupported, PrinterAttribute.MultipleDocumentJobsSupported)
               .ForIppMember(dst => dst.CharsetConfigured, PrinterAttribute.CharsetConfigured)
               .ForIppMember(dst => dst.NaturalLanguageConfigured, PrinterAttribute.NaturalLanguageConfigured)
               .ForIppMemberSetNull(dst => dst.GeneratedNaturalLanguageSupported,
                    PrinterAttribute.GeneratedNaturalLanguageSupported)
               .ForIppMember(dst => dst.DocumentFormatDefault, PrinterAttribute.DocumentFormatDefault)
               .ForIppMemberSetNull(dst => dst.DocumentFormatSupported, PrinterAttribute.DocumentFormatSupported)
               .ForIppMember(dst => dst.PrinterIsAcceptingJobs, PrinterAttribute.PrinterIsAcceptingJobs)
               .ForIppMember(dst => dst.QueuedJobCount, PrinterAttribute.QueuedJobCount)
               .ForIppMember(dst => dst.PrinterMessageFromOperator, PrinterAttribute.PrinterMessageFromOperator)
               .ForIppMember(dst => dst.ColorSupported, PrinterAttribute.ColorSupported)
               .ForIppMemberSetNull(dst => dst.ReferenceUriSchemesSupported,
                    PrinterAttribute.ReferenceUriSchemesSupported)
               .ForIppMember(dst => dst.PdlOverrideSupported, PrinterAttribute.PdlOverrideSupported)
               .ForIppMember(dst => dst.PrinterUpTime, PrinterAttribute.PrinterUpTime)
               .ForIppMember(dst => dst.PrinterCurrentTime, PrinterAttribute.PrinterCurrentTime)
               .ForIppMember(dst => dst.MultipleOperationTimeOut, PrinterAttribute.MultipleOperationTimeOut)
               .ForIppMemberSetNull(dst => dst.CompressionSupported, PrinterAttribute.CompressionSupported)
               .ForIppMember(dst => dst.JobKOctetsSupported, PrinterAttribute.JobKOctetsSupported)
               .ForIppMember(dst => dst.JobImpressionsSupported, PrinterAttribute.JobImpressionsSupported)
               .ForIppMember(dst => dst.JobMediaSheetsSupported, PrinterAttribute.JobMediaSheetsSupported)
               .ForIppMember(dst => dst.PagesPerMinute, PrinterAttribute.PagesPerMinute)
               .ForIppMemberSetNull(dst => dst.PrintScalingSupported, PrinterAttribute.PrintScalingSupported)
               .ForIppMember(dst => dst.PrintScalingDefault, PrinterAttribute.PrintScalingDefault)
                ;
        }
    }
}