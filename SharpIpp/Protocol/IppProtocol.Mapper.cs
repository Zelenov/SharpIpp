using System;
using System.Collections.Generic;
using AutoMapper;
using SharpIpp.Model;
using SharpIpp.Protocol.Extensions;

namespace SharpIpp.Protocol
{
    internal partial class IppProtocol
    {
        private static readonly IMapper Mapper;

        static IppProtocol()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AllowNullCollections = true;
                cfg.AllowNullDestinationValues = true;


                cfg.CreateMap<int, IppOperation>().ConstructUsing(src => (IppOperation)src);
                cfg.CreateIppMap<object, int>(true);
                cfg.CreateIppMap<object, bool>(true);
                cfg.CreateIppMap<object, DateTimeOffset>(true);
                cfg.CreateIppMap<object, Range>(true);
                cfg.CreateIppMap<object, Resolution>(true);
                cfg.CreateIppMap<object, StringWithLanguage>(true);

                cfg.CreateIppMap<int, IppOperation>();
                cfg.CreateIppMap<int, Finishings>();
                cfg.CreateIppMap<int, IppStatusCode>();
                cfg.CreateIppMap<int, JobState>();
                cfg.CreateIppMap<int, Orientation>();
                cfg.CreateIppMap<int, PrinterState>();
                cfg.CreateIppMap<int, PrintQuality>();
                cfg.CreateIppMap<int, ResolutionUnit>();

                cfg.CreateIppMap<string, JobHoldUntil>();
                cfg.CreateIppMap<string, JobSheets>();
                cfg.CreateIppMap<string, MultipleDocumentHandling>();
                cfg.CreateIppMap<string, Sides>();
                cfg.CreateIppMap<string, Compression>();
                cfg.CreateIppMap<string, PrintScaling>();


                cfg.CreateMap<object, string?>().ConstructUsing((src, __) => src is string i ? i : null);
                cfg.CreateMap<NoValue, string[]?>().ConstructUsing(_ => null);
                cfg.CreateMap<string, string[]>().ConstructUsing(src => new[] {src});

                ConfigureJobHoldUntil(cfg);
                ConfigureMultipleDocumentHandling(cfg);
                ConfigureSides(cfg);
                ConfigureJobSheets(cfg);
                ConfigureCompression(cfg);
                ConfigurePrintScaling(cfg);
                ConfigurePrintJobRequest(cfg);
                ConfigureGetPrinterAttributesResponse(cfg);
                ConfigureGetJobAttributesResponse(cfg);
                ConfigureGetJobsResponse(cfg);
            });

            Mapper = config.CreateMapper();
        }

        private static void ConfigureJobHoldUntil(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<object, JobHoldUntil>()
               .ConstructUsing((src, ctx) => src switch
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

            cfg.CreateMap<JobHoldUntil, string>()
               .ConstructUsing((src, ctx) => src switch
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

        private static void ConfigureMultipleDocumentHandling(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<object, MultipleDocumentHandling>()
               .ConstructUsing((src, ctx) => src switch
                {
                    "single-document" => MultipleDocumentHandling.SingleDocument,
                    "separate-documents-uncollated-copies" =>
                    MultipleDocumentHandling.SeparateDocumentsUncollatedCopies,
                    "separate-documents-collated-copies" => MultipleDocumentHandling.SeparateDocumentsCollatedCopies,
                    "single-document-new-sheet" => MultipleDocumentHandling.SingleDocumentNewSheet,
                    _ => MultipleDocumentHandling.Unsupported
                });

            cfg.CreateMap<MultipleDocumentHandling, string>()
               .ConstructUsing((src, ctx) => src switch
                {
                    MultipleDocumentHandling.SingleDocument => "single-document",
                    MultipleDocumentHandling.SeparateDocumentsUncollatedCopies =>
                    "separate-documents-uncollated-copies",
                    MultipleDocumentHandling.SeparateDocumentsCollatedCopies => "separate-documents-collated-copies",
                    MultipleDocumentHandling.SingleDocumentNewSheet => "single-document-new-sheet",
                    _ => "unsupported"
                });
        }

        private static void ConfigureSides(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<object, Sides>()
               .ConstructUsing((src, ctx) => src switch
                {
                    "one-sided" => Sides.OneSided,
                    "two-sided-long-edge" => Sides.TwoSidedLongEdge,
                    "two-sided-short-edge" => Sides.TwoSidedShortEdge,
                    _ => Sides.Unsupported
                });

            cfg.CreateMap<Sides, string>()
               .ConstructUsing((src, ctx) => src switch
                {
                    Sides.OneSided => "one-sided",
                    Sides.TwoSidedLongEdge => "two-sided-long-edge",
                    Sides.TwoSidedShortEdge => "two-sided-short-edge",
                    _ => "unsupported"
                });
        }

        private static void ConfigureJobSheets(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<object, JobSheets>()
               .ConstructUsing((src, ctx) => src switch
                {
                    "none" => JobSheets.None,
                    "standard" => JobSheets.Standard,
                    _ => JobSheets.Unsupported
                });

            cfg.CreateMap<JobSheets, string>()
               .ConstructUsing((src, ctx) => src switch
                {
                    JobSheets.None => "none",
                    JobSheets.Standard => "standard",
                    _ => "unsupported"
                });
        }

        private static void ConfigureCompression(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<object, Compression>()
               .ConstructUsing((src, ctx) => src switch
                {
                    "none" => Compression.None,
                    "deflate" => Compression.Deflate,
                    "gzip" => Compression.Gzip,
                    _ => Compression.Unsupported
                });

            cfg.CreateMap<Compression, string>()
               .ConstructUsing((src, ctx) => src switch
                {
                    Compression.None => "none",
                    Compression.Deflate => "deflate",
                    Compression.Gzip => "gzip",
                    _ => "unsupported"
                });
        }
        private static void ConfigurePrintScaling(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<object, PrintScaling>()
               .ConstructUsing((src, ctx) =>
                    src switch
                {
                    "auto" => PrintScaling.Auto,
                    "auto-fit" => PrintScaling.AutoFit,
                    "fill" => PrintScaling.Fill,
                    "fit" => PrintScaling.Fit,
                    "none" => PrintScaling.None,
                    _ => PrintScaling.Unsupported
                });

            cfg.CreateMap<PrintScaling, string>()
               .ConstructUsing((src, ctx) => src switch
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