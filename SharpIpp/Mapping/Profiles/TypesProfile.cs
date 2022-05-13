using System;

using SharpIpp.Protocol.Models;

namespace SharpIpp.Mapping.Profiles
{
    // ReSharper disable once UnusedMember.Global
    internal class TypesProfile : IProfile
    {
        public void CreateMaps(IMapperConstructor mapper)
        {
            mapper.CreateIppMap<int>();
            mapper.CreateIppMap<bool>();
            mapper.CreateIppMap<string>();
            mapper.CreateIppMap<DateTimeOffset>();
            mapper.CreateIppMap<Range>();
            mapper.CreateIppMap<Resolution>();
            mapper.CreateIppMap<StringWithLanguage>();

            var unixStartTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified);
            mapper.CreateIppMap<int, DateTime>((src, map) => unixStartTime.AddSeconds(src));
            mapper.CreateIppMap<int, IppOperation>((src, map) => (IppOperation)(short)src);
            mapper.CreateIppMap<int, Finishings>((src, map) => (Finishings)src);
            mapper.CreateIppMap<int, IppStatusCode>((src, map) => (IppStatusCode)src);
            mapper.CreateIppMap<int, JobState>((src, map) => (JobState)src);
            mapper.CreateIppMap<int, Orientation>((src, map) => (Orientation)src);
            mapper.CreateIppMap<int, PrinterState>((src, map) => (PrinterState)src);
            mapper.CreateIppMap<int, PrintQuality>((src, map) => (PrintQuality)src);
            mapper.CreateIppMap<int, ResolutionUnit>((src, map) => (ResolutionUnit)src);
            mapper.CreateIppMap<int, PrinterType>((src, map) => (PrinterType)src);

            //All name parameters can come as StringWithLanguage or string
            //mappers for string\language mapping 
            mapper.CreateIppMap<StringWithLanguage, string>((src, map) => src.Value);
            mapper.CreateIppMap<string, StringWithLanguage?>((src, map) => null);


            ConfigureJobHoldUntil(mapper);
            ConfigureMultipleDocumentHandling(mapper);
            ConfigureSides(mapper);
            ConfigureJobSheets(mapper);
            ConfigureCompression(mapper);
            ConfigurePrintScaling(mapper);
            ConfigureWhichJobs(mapper);
        }

        private void ConfigureJobHoldUntil(IMapperConstructor map)
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
                _ => JobHoldUntil.Unsupported,
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
                _ => "unsupported",
            });
        }

        private void ConfigureMultipleDocumentHandling(IMapperConstructor map)
        {
            map.CreateIppMap<string, MultipleDocumentHandling>((src, ctx) => src switch
            {
                "single-document" => MultipleDocumentHandling.SingleDocument,
                "separate-documents-uncollated-copies" => MultipleDocumentHandling.SeparateDocumentsUncollatedCopies,
                "separate-documents-collated-copies" => MultipleDocumentHandling.SeparateDocumentsCollatedCopies,
                "single-document-new-sheet" => MultipleDocumentHandling.SingleDocumentNewSheet,
                _ => MultipleDocumentHandling.Unsupported,
            });

            map.CreateMap<MultipleDocumentHandling, string>((src, ctx) => src switch
            {
                MultipleDocumentHandling.SingleDocument => "single-document",
                MultipleDocumentHandling.SeparateDocumentsUncollatedCopies => "separate-documents-uncollated-copies",
                MultipleDocumentHandling.SeparateDocumentsCollatedCopies => "separate-documents-collated-copies",
                MultipleDocumentHandling.SingleDocumentNewSheet => "single-document-new-sheet",
                _ => "unsupported",
            });
        }

        private void ConfigureSides(IMapperConstructor map)
        {
            map.CreateIppMap<string, Sides>((src, ctx) => src switch
            {
                "one-sided" => Sides.OneSided,
                "two-sided-long-edge" => Sides.TwoSidedLongEdge,
                "two-sided-short-edge" => Sides.TwoSidedShortEdge,
                _ => Sides.Unsupported,
            });

            map.CreateMap<Sides, string>((src, ctx) => src switch
            {
                Sides.OneSided => "one-sided",
                Sides.TwoSidedLongEdge => "two-sided-long-edge",
                Sides.TwoSidedShortEdge => "two-sided-short-edge",
                _ => "unsupported",
            });
        }

        private void ConfigureJobSheets(IMapperConstructor map)
        {
            map.CreateIppMap<string, JobSheets>((src, ctx) => src switch
            {
                "none" => JobSheets.None,
                "standard" => JobSheets.Standard,
                _ => JobSheets.Unsupported,
            });

            map.CreateMap<JobSheets, string>((src, ctx) => src switch
            {
                JobSheets.None => "none",
                JobSheets.Standard => "standard",
                _ => "unsupported",
            });
        }

        private void ConfigureCompression(IMapperConstructor map)
        {
            map.CreateIppMap<string, Compression>((src, ctx) => src switch
            {
                "none" => Compression.None,
                "deflate" => Compression.Deflate,
                "gzip" => Compression.Gzip,
                _ => Compression.Unsupported,
            });

            map.CreateMap<Compression, string>((src, ctx) => src switch
            {
                Compression.None => "none",
                Compression.Deflate => "deflate",
                Compression.Gzip => "gzip",
                _ => "unsupported",
            });
        }

        private void ConfigurePrintScaling(IMapperConstructor map)
        {
            map.CreateIppMap<string, PrintScaling>((src, ctx) => src switch
            {
                "auto" => PrintScaling.Auto,
                "auto-fit" => PrintScaling.AutoFit,
                "fill" => PrintScaling.Fill,
                "fit" => PrintScaling.Fit,
                "none" => PrintScaling.None,
                _ => PrintScaling.Unsupported,
            });

            map.CreateMap<PrintScaling, string>((src, ctx) => src switch
            {
                PrintScaling.Auto => "auto",
                PrintScaling.AutoFit => "auto-fit",
                PrintScaling.Fill => "fill",
                PrintScaling.Fit => "fit",
                PrintScaling.None => "None",
                _ => "unsupported",
            });
        }

        private void ConfigureWhichJobs(IMapperConstructor map)
        {
            map.CreateIppMap<string, WhichJobs>((src, ctx) => src switch
            {
                "completed" => WhichJobs.Completed,
                "not-completed" => WhichJobs.NotCompleted,
                _ => WhichJobs.Unsupported,
            });

            map.CreateMap<WhichJobs, string>((src, ctx) => src switch
            {
                WhichJobs.Completed => "completed",
                WhichJobs.NotCompleted => "not-completed",
                _ => "unsupported",
            });
        }
    }
}
