using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            printJobResponse.IppVersion = response.Version;
            printJobResponse.RequestId = response.RequestId;
            return printJobResponse;
        }

        private static void ConfigurePrintJobRequest(SimpleMapper mapper)
        {
            mapper.CreateMap<PrintJobRequest, IppRequest>((src, map) => new IppRequest
            {
               IppOperation = IppOperation.PrintJob, IppVersion = src.IppVersion, RequestId = src.RequestId
            });

            mapper.CreateMap<IDictionary<string, IppAttribute[]>, PrintJobResponse>((src, map) => new PrintJobResponse
            {
                AllAttributes = src,
                JobUri = map.MapFromDic<string>(src, "job-uri"),
                JobId = map.MapFromDic<int>(src, "job-id"),
                JobState = map.MapFromDic<JobState>(src, "job-state"),
                JobStateReasons = map.MapFromDicSet<string[]>(src, "job-state-reasons"),
                JobStateMessage = map.MapFromDic<string?>(src, "job-state-message"),
                NumberOfInterveningJobs = map.MapFromDic<int?>(src, "number-of-intervening-jobs"),
            });

        }
    }
}