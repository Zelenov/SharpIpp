using System;
using System.Collections.Generic;
using System.Linq;
using SharpIpp.Model;
using SharpIpp.Protocol.Extensions;

namespace SharpIpp.Protocol
{
    internal partial class IppProtocol
    {
        private static void ConfigureIIppRequest(SimpleMapper mapper)
        {
            mapper.CreateMap<IIppRequest, IppRequestMessage>((src, dst, map) =>
            {
                dst.Version = src.Version;
                dst.RequestId = src.RequestId;
                var operation = dst.OperationAttributes;
                operation.Add(new IppAttribute(Tag.Charset, "attributes-charset", "utf-8"));
                operation.Add(new IppAttribute(Tag.NaturalLanguage, "attributes-natural-language", "en"));
                if (src.RequestingUserName != null)
                    operation.Add(new IppAttribute(Tag.NameWithoutLanguage, "requesting-user-name",
                        src.RequestingUserName));
                return dst;
            });
        }

        private static void ConfigureIIppJobRequest(SimpleMapper mapper)
        {
            mapper.CreateMap<IIppJobRequest, IppRequestMessage>((src, dst, map) =>
            {
                mapper.Map<IIppRequest, IppRequestMessage>(src, dst);
                var operation = dst.OperationAttributes;
                if (src.JobUrl != null)
                {
                    operation.Add(new IppAttribute(Tag.Uri, "job-uri", src.JobUrl.ToString()));
                }
                else if (src.JobId != null)
                {
                    operation.Add(new IppAttribute(Tag.Uri, "printer-uri", src.PrinterUri.ToString()));
                    operation.Add(new IppAttribute(Tag.Integer, "job-id", src.JobId.Value));
                }
                else
                {
                    throw new ArgumentException(
                        $"JobTarget must have {nameof(GetJobAttributesRequest.JobUrl)} or {nameof(GetJobAttributesRequest.JobId)} set");
                }

                return dst;
            });
        }

        private static void ConfigureIIppPrinterRequest(SimpleMapper mapper)
        {
            mapper.CreateMap<IIppPrinterRequest, IppRequestMessage>((src, dst, map) =>
            {
                mapper.Map<IIppRequest, IppRequestMessage>(src, dst);
                var operation = dst.OperationAttributes;
                operation.Add(new IppAttribute(Tag.Uri, "printer-uri", src.PrinterUri.ToString()));
                return dst;
            });
        }


        private static void ConfigureDocumentAttributes(SimpleMapper mapper)
        {
            mapper.CreateMap<DocumentAttributes, IppRequestMessage>((src, dst, map) =>
            {
                var operation = dst.OperationAttributes;
                if (src.DocumentName != null)
                    operation.Add(new IppAttribute(Tag.NameWithoutLanguage, "document-name", src.DocumentName));
                if (src.DocumentFormat != null)
                    operation.Add(new IppAttribute(Tag.MimeMediaType, "document-format", src.DocumentFormat));
                if (src.DocumentNaturalLanguage != null)
                    operation.Add(new IppAttribute(Tag.NaturalLanguage, "document-natural-language",
                        src.DocumentNaturalLanguage));
                if (src.Compression != null)
                    operation.Add(new IppAttribute(Tag.Keyword, "compression", map.Map<string>(src.Compression)));
                return dst;
            });
        }

        private static void ConfigureNewJobAttributes(SimpleMapper mapper)
        {
            mapper.CreateMap<NewJobAttributes, IppRequestMessage>((src, dst, map) =>
            {
                var operation = dst.OperationAttributes;
                var job = dst.JobAttributes;
                if (src.JobName != null)
                    operation.Add(new IppAttribute(Tag.NameWithoutLanguage, "job-name", src.JobName));
                if (src.IppAttributeFidelity != null)
                    operation.Add(new IppAttribute(Tag.Boolean, "ipp-attribute-fidelity",
                        src.IppAttributeFidelity.Value));
                if (src.JobPriority != null)
                    job.Add(new IppAttribute(Tag.Integer, "job-priority", src.JobPriority.Value));
                if (src.JobHoldUntil != null)
                    job.Add(new IppAttribute(Tag.NameWithoutLanguage, "job-hold-until",
                        mapper.Map<string>(src.JobHoldUntil.Value)));
                if (src.MultipleDocumentHandling != null)
                    job.Add(new IppAttribute(Tag.Keyword, "multiple-document-handling",
                        mapper.Map<string>(src.MultipleDocumentHandling.Value)));
                if (src.Copies != null)
                    job.Add(new IppAttribute(Tag.Integer, "copies", src.Copies.Value));
                if (src.Finishings != null)
                    job.Add(new IppAttribute(Tag.Enum, "finishings", (int) src.Finishings.Value));
                if (src.PageRanges != null)
                    job.AddRange(src.PageRanges.Select(pageRange =>
                        new IppAttribute(Tag.RangeOfInteger, "page-ranges", pageRange)));
                if (src.Sides != null)
                    job.Add(new IppAttribute(Tag.Keyword, "sides", mapper.Map<string>(src.Sides.Value)));
                if (src.NumberUp != null)
                    job.Add(new IppAttribute(Tag.Integer, "number-up", src.NumberUp.Value));
                if (src.OrientationRequested != null)
                    job.Add(new IppAttribute(Tag.Enum, "orientation-srced", (int) src.OrientationRequested.Value));
                if (src.Media != null)
                    job.Add(new IppAttribute(Tag.Keyword, "media", src.Media));
                if (src.PrinterResolution != null)
                    job.Add(new IppAttribute(Tag.Resolution, "printer-resolution", src.PrinterResolution.Value));
                if (src.PrintQuality != null)
                    job.Add(new IppAttribute(Tag.Enum, "print-quality", (int) src.PrintQuality.Value));
                if (src.PrintScaling != null)
                    job.Add(new IppAttribute(Tag.Keyword, "print-scaling", mapper.Map<string>(src.PrintScaling.Value)));

                dst.OperationAttributes.Populate(src.AdditionalOperationAttributes);
                dst.JobAttributes.Populate(src.AdditionalJobAttributes);
                return dst;
            });
        }

        private static void ConfigureIIppResponse(SimpleMapper mapper)
        {
            mapper.CreateMap<IppResponseMessage, IIppResponseMessage>((src, dst, map) =>
            {
                dst.Version = src.Version;
                dst.RequestId = src.RequestId;
                dst.Sections.AddRange(src.Sections);
                return dst;
            });
        }

        private static void ConfigureIIppJobResponse(SimpleMapper mapper)
        {
            mapper.CreateMap<IppResponseMessage, IIppJobResponse>((src, dst, map) =>
            {
                mapper.Map<IppResponseMessage, IIppResponseMessage>(src, dst);
                mapper.Map(src.AllAttributes(), dst);
                return dst;
            });
            mapper.CreateMap<IDictionary<string, IppAttribute[]>, IIppJobResponse>((src, dst, map) =>
            {
                dst.JobUri = map.MapFromDic<string>(src, JobAttribute.JobUri);
                dst.JobId = map.MapFromDic<int>(src, JobAttribute.JobId);
                dst.JobState = map.MapFromDic<JobState>(src, JobAttribute.JobState);
                dst.JobStateReasons = map.MapFromDicSet<string[]>(src, JobAttribute.JobStateReasons);
                dst.JobStateMessage = map.MapFromDic<string?>(src, JobAttribute.JobStateMessage);
                dst.NumberOfInterveningJobs = map.MapFromDic<int?>(src, JobAttribute.NumberOfInterveningJobs);
                return dst;
            });
        }
    }
}