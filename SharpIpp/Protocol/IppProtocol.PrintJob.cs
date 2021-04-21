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
        /// Print-Job Request
        /// https://tools.ietf.org/html/rfc2911#section-3.2.1.1
        /// </summary>
        /// <param name="request"></param>
        /// <param name="stream"></param>
        public void Write(PrintJobRequest request, Stream stream)
        {
            using var writer = new BinaryWriter(stream, Encoding.ASCII, true);
            var attributes = new IEnumerable<IppAttribute>?[]
            {
                new [] { new IppAttribute(Tag.Charset, "attributes-charset", "utf-8")},
                new [] { new IppAttribute(Tag.NaturalLanguage, "attributes-natural-language", "en")},
                new [] { new IppAttribute(Tag.Uri, "printer-uri", request.PrinterUri.ToString())},
                request.JobName == null ? null : new [] { new IppAttribute(Tag.NameWithoutLanguage, "job-name", request.JobName)},
                request.IppAttributeFidelity == null ? null : new [] { new IppAttribute(Tag.Boolean, "ipp-attribute-fidelity", request.IppAttributeFidelity)},
                request.DocumentName == null ? null : new [] { new IppAttribute(Tag.NameWithoutLanguage, "document-name", request.DocumentName)},
                request.DocumentFormat == null ? null : new [] { new IppAttribute(Tag.MimeMediaType, "document-format", request.DocumentFormat)},
                request.DocumentNaturalLanguage == null ? null : new [] { new IppAttribute(Tag.NaturalLanguage, "document-natural-language", request.DocumentNaturalLanguage)},
                request.JobPriority == null ? null : new [] { new IppAttribute(Tag.Integer, "job-priority", request.JobPriority)},
                request.JobHoldUntil == null ? null : new [] { new IppAttribute(Tag.NameWithoutLanguage, "job-hold-until", Mapper.Map<string>(request.JobHoldUntil))},
                request.MultipleDocumentHandling == null ? null : new [] { new IppAttribute(Tag.NameWithoutLanguage, "multiple-document-handling", Mapper.Map<string>(request.MultipleDocumentHandling))},
                request.Copies == null ? null : new [] { new IppAttribute(Tag.Integer, "copies", request.Copies)},
                request.Finishings == null ? null : new [] { new IppAttribute(Tag.Integer, "finishings", Mapper.Map<string>(request.Finishings))},
                request.PageRanges?.Select(pageRange=> new IppAttribute(Tag.Integer, "page-ranges", pageRange)),
                request.Sides == null ? null : new [] { new IppAttribute(Tag.NameWithoutLanguage, "sides", Mapper.Map<string>(request.Sides)) },
                request.NumberUp == null ? null : new [] { new IppAttribute(Tag.Integer, "number-up", request.NumberUp) },
                request.OrientationRequested == null ? null : new [] { new IppAttribute(Tag.NameWithoutLanguage, "orientation-requested", Mapper.Map<string>(request.OrientationRequested)) },
                request.Media == null ? null : new [] { new IppAttribute(Tag.NameWithoutLanguage, "media", Mapper.Map<string>(request.Media)) },
                request.PrinterResolution == null ? null : new [] { new IppAttribute(Tag.NameWithoutLanguage, "printer-resolution", request.PrinterResolution) },
                request.PrintQuality == null ? null : new [] { new IppAttribute(Tag.NameWithoutLanguage, "print-quality", Mapper.Map<string>(request.PrintQuality)) },
            }.Where(s => s != null).SelectMany(s=>s).Cast<IppAttribute>().ToArray();

            writer.WriteBigEndian((short) request.IppVersion);
            writer.WriteBigEndian((short) IppOperationType.PrintJob);
            writer.WriteBigEndian(request.RequestId);
            Write(attributes, writer);
            request.Document.CopyTo(writer.BaseStream);
        }
        public PrintJobResponse ReadPrintJobResponse(Stream stream)
        {
            var response = ReadStream(stream);
            if (!response.IsSuccessfulStatusCode)
                throw new IppResponseException($"Printer returned error code in Print-Job response\n{response}",
                    response);

            var attributes = response.Attributes;
            var printJobResponse = Mapper.Map<PrintJobResponse>(attributes);
            return printJobResponse;
        }
        static void ConfigurePrintJobRequest(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<IDictionary<string, IppAttribute[]>, PrintJobResponse>()
               .ForMember(dst => dst.JobUri, opt => opt.MapFromDic("job-uri"))
               .ForMember(dst => dst.JobId, opt => opt.MapFromDic("job-id"))
               .ForMember(dst => dst.JobState, opt => opt.MapFromDic("job-state"))
               .ForMember(dst => dst.JobStateReasons, opt => opt.MapFromDicSet("job-state-reasons"))
               .ForMember(dst => dst.JobStateMessage, opt => opt.MapFromDic("job-state-message"))
               .ForMember(dst => dst.NumberOfInterveningJobs, opt => opt.MapFromDic("number-of-intervening-jobs"));
        }




    }
}