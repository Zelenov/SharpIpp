using System;
using SharpIpp.Model;
using SharpIpp.Protocol.Extensions;

namespace SharpIpp.Protocol
{
    internal partial class IppProtocol
    {
        private static readonly SimpleMapper Mapper = new SimpleMapper();

        static IppProtocol()
        {
            Mapper.CreateIppMap<int>();
            Mapper.CreateIppMap<bool>();
            Mapper.CreateIppMap<string>();
            Mapper.CreateIppMap<DateTimeOffset>();
            Mapper.CreateIppMap<Range>();
            Mapper.CreateIppMap<Resolution>();
            Mapper.CreateIppMap<StringWithLanguage>();


            Mapper.CreateIppMap<int, IppOperation>((src, map) => (IppOperation)(short)src);
            Mapper.CreateIppMap<int, Finishings>((src, map) => (Finishings)src);
            Mapper.CreateIppMap<int, IppStatusCode>((src, map) => (IppStatusCode)src);
            Mapper.CreateIppMap<int, JobState>((src, map) => (JobState)src);
            Mapper.CreateIppMap<int, Orientation>((src, map) => (Orientation)src);
            Mapper.CreateIppMap<int, PrinterState>((src, map) => (PrinterState)src);
            Mapper.CreateIppMap<int, PrintQuality>((src, map) => (PrintQuality)src);
            Mapper.CreateIppMap<int, ResolutionUnit>((src, map) => (ResolutionUnit)src);


            ConfigureJobHoldUntil(Mapper);
            ConfigureMultipleDocumentHandling(Mapper);
            ConfigureSides(Mapper);
            ConfigureJobSheets(Mapper);
            ConfigureCompression(Mapper);
            ConfigurePrintScaling(Mapper);
            ConfigurePrintJobRequest(Mapper);
            ConfigureGetPrinterAttributesResponse(Mapper);
            ConfigureGetJobAttributesResponse(Mapper);
            ConfigureGetJobsResponse(Mapper);
        }

        private static void ConfigureJobHoldUntil(SimpleMapper map)
        {
            map.CreateIppMap<string, JobHoldUntil>((src, ctx) => src switch
            {
                "no-hold" => JobHoldUntil.NoHold,
                "indefinite" => JobHoldUntil.Indefinite,
                "day-time" => JobHoldUntil.DayTime,
                "evening" => JobHoldUntil.Evening,
                "night" => JobHoldUntil.Night,
                "weekend" => JobHoldUntil.Weekend,
                "second-shift" => JobHoldUntil.SecondShift,
                "third-shift" => JobHoldUntil.ThirdShift,
                _ => JobHoldUntil.Unsupported
            });

            map.CreateMap<JobHoldUntil, string>((src, ctx) => src switch
            {
                JobHoldUntil.NoHold => "no-hold",
                JobHoldUntil.Indefinite => "indefinite",
                JobHoldUntil.DayTime => "day-time",
                JobHoldUntil.Evening => "evening",
                JobHoldUntil.Night => "night",
                JobHoldUntil.Weekend => "weekend",
                JobHoldUntil.SecondShift => "second-shift",
                JobHoldUntil.ThirdShift => "third-shift",
                _ => "unsupported"
            });
        }

        private static void ConfigureMultipleDocumentHandling(SimpleMapper map)
        {
            map.CreateIppMap<string, MultipleDocumentHandling>((src, ctx) => src switch
            {
                "single-document" => MultipleDocumentHandling.SingleDocument,
                "separate-documents-uncollated-copies" =>
                MultipleDocumentHandling.SeparateDocumentsUncollatedCopies,
                "separate-documents-collated-copies" => MultipleDocumentHandling.SeparateDocumentsCollatedCopies,
                "single-document-new-sheet" => MultipleDocumentHandling.SingleDocumentNewSheet,
                _ => MultipleDocumentHandling.Unsupported
            });

            map.CreateMap<MultipleDocumentHandling, string>((src, ctx) => src switch
            {
                MultipleDocumentHandling.SingleDocument => "single-document",
                MultipleDocumentHandling.SeparateDocumentsUncollatedCopies =>
                "separate-documents-uncollated-copies",
                MultipleDocumentHandling.SeparateDocumentsCollatedCopies => "separate-documents-collated-copies",
                MultipleDocumentHandling.SingleDocumentNewSheet => "single-document-new-sheet",
                _ => "unsupported"
            });
        }

        private static void ConfigureSides(SimpleMapper map)
        {
            map.CreateIppMap<string, Sides>((src, ctx) => src switch
            {
                "one-sided" => Sides.OneSided,
                "two-sided-long-edge" => Sides.TwoSidedLongEdge,
                "two-sided-short-edge" => Sides.TwoSidedShortEdge,
                _ => Sides.Unsupported
            });

            map.CreateMap<Sides, string>((src, ctx) => src switch
            {
                Sides.OneSided => "one-sided",
                Sides.TwoSidedLongEdge => "two-sided-long-edge",
                Sides.TwoSidedShortEdge => "two-sided-short-edge",
                _ => "unsupported"
            });
        }

        private static void ConfigureJobSheets(SimpleMapper map)
        {
            map.CreateIppMap<string, JobSheets>((src, ctx) => src switch
            {
                "none" => JobSheets.None,
                "standard" => JobSheets.Standard,
                _ => JobSheets.Unsupported
            });

            map.CreateMap<JobSheets, string>((src, ctx) => src switch
            {
                JobSheets.None => "none",
                JobSheets.Standard => "standard",
                _ => "unsupported"
            });
        }

        private static void ConfigureCompression(SimpleMapper map)
        {
            map.CreateIppMap<string, Compression>((src, ctx) => src switch
            {
                "none" => Compression.None,
                "deflate" => Compression.Deflate,
                "gzip" => Compression.Gzip,
                _ => Compression.Unsupported
            });

            map.CreateMap<Compression, string>((src, ctx) => src switch
            {
                Compression.None => "none",
                Compression.Deflate => "deflate",
                Compression.Gzip => "gzip",
                _ => "unsupported"
            });
        }
        private static void ConfigurePrintScaling(SimpleMapper map)
        {
            map.CreateIppMap<string, PrintScaling>((src, ctx) =>
                    src switch
                    {
                        "auto" => PrintScaling.Auto,
                        "auto-fit" => PrintScaling.AutoFit,
                        "fill" => PrintScaling.Fill,
                        "fit" => PrintScaling.Fit,
                        "none" => PrintScaling.None,
                        _ => PrintScaling.Unsupported
                    });

            map.CreateMap<PrintScaling, string>((src, ctx) => src switch
            {
                PrintScaling.Auto => "auto",
                PrintScaling.AutoFit => "auto-fit",
                PrintScaling.Fill => "fill",
                PrintScaling.Fit => "fit",
                PrintScaling.None => "None",
                _ => "unsupported"
            });
        }
    }
}