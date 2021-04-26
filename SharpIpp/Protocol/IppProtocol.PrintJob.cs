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
        ///     Print-Job Request
        ///     https://tools.ietf.org/html/rfc2911#section-3.2.1.1
        /// </summary>
        /// <param name="request"></param>
        /// <param name="stream"></param>
        public void Write(PrintJobRequest request, Stream stream)
        {
            var r = Mapper.Map<IppRequest>(request);
            var operation = r.OperationAttributes;
            var job = r.JobAttributes;
            operation.Add(new IppAttribute(Tag.Charset, "attributes-charset", "utf-8"));
            operation.Add(new IppAttribute(Tag.NaturalLanguage, "attributes-natural-language", "en"));
            operation.Add(new IppAttribute(Tag.Uri, "printer-uri", request.PrinterUri.ToString()));
            if (request.JobName != null)
                operation.Add(new IppAttribute(Tag.NameWithoutLanguage, "job-name", request.JobName));
            if (request.IppAttributeFidelity != null)
                operation.Add(new IppAttribute(Tag.Boolean, "ipp-attribute-fidelity",
                    request.IppAttributeFidelity.Value));
            if (request.DocumentName != null)
                operation.Add(new IppAttribute(Tag.NameWithoutLanguage, "document-name", request.DocumentName));
            if (request.DocumentFormat != null)
                operation.Add(new IppAttribute(Tag.MimeMediaType, "document-format", request.DocumentFormat));
            if (request.DocumentNaturalLanguage != null)
                operation.Add(new IppAttribute(Tag.NaturalLanguage, "document-natural-language",
                    request.DocumentNaturalLanguage));
            if (request.JobPriority != null)
                job.Add(new IppAttribute(Tag.Integer, "job-priority", request.JobPriority.Value));
            if (request.JobHoldUntil != null)
                job.Add(new IppAttribute(Tag.NameWithoutLanguage, "job-hold-until",
                    Mapper.Map<string>(request.JobHoldUntil.Value)));
            if (request.MultipleDocumentHandling != null)
                job.Add(new IppAttribute(Tag.Keyword, "multiple-document-handling",
                    Mapper.Map<string>(request.MultipleDocumentHandling.Value)));
            if (request.Copies != null)
                job.Add(new IppAttribute(Tag.Integer, "copies", request.Copies.Value));
            if (request.Finishings != null)
                job.Add(new IppAttribute(Tag.Enum, "finishings", (int) request.Finishings.Value));
            if (request.PageRanges != null)
                job.AddRange(request.PageRanges.Select(pageRange =>
                    new IppAttribute(Tag.RangeOfInteger, "page-ranges", pageRange)));
            if (request.Sides != null)
                job.Add(new IppAttribute(Tag.Keyword, "sides", Mapper.Map<string>(request.Sides.Value)));
            if (request.NumberUp != null)
                job.Add(new IppAttribute(Tag.Integer, "number-up", request.NumberUp.Value));
            if (request.OrientationRequested != null)
                job.Add(new IppAttribute(Tag.Enum, "orientation-requested", (int) request.OrientationRequested.Value));
            if (request.Media != null)
                job.Add(new IppAttribute(Tag.Keyword, "media", request.Media));
            if (request.PrinterResolution != null)
                job.Add(new IppAttribute(Tag.Resolution, "printer-resolution", request.PrinterResolution.Value));
            if (request.PrintQuality != null)
                job.Add(new IppAttribute(Tag.Enum, "print-quality", (int) request.PrintQuality.Value));
            if (request.PrintScaling != null)
                job.Add(new IppAttribute(Tag.Keyword, "print-scaling", Mapper.Map<string>(request.PrintScaling.Value)));

            r.OperationAttributes.Populate(request.AdditionalOperationAttributes);
            r.JobAttributes.Populate(request.AdditionalJobAttributes);

            using var writer = new BinaryWriter(stream, Encoding.ASCII, true);
            Write(r, writer);
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

        private static void ConfigurePrintJobRequest(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<PrintJobRequest, IppRequest>()
               .ForMember(dst => dst.IppOperation, opt => opt.MapFrom(_ => IppOperation.PrintJob));

            cfg.CreateMap<IDictionary<string, IppAttribute[]>, PrintJobResponse>()
               .ForMember(dst => dst.AllAttributes, opt => opt.MapFrom(src => src))
               .ForIppMember(dst => dst.JobUri, "job-uri")
               .ForIppMember(dst => dst.JobId, "job-id")
               .ForIppMember(dst => dst.JobState, "job-state")
               .ForIppMemberSet(dst => dst.JobStateReasons, "job-state-reasons")
               .ForIppMember(dst => dst.JobStateMessage, "job-state-message")
               .ForIppMember(dst => dst.NumberOfInterveningJobs, "number-of-intervening-jobs");


        }
    }
}