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

                  cfg.CreateMap<int, IppOperation>().ConstructUsing(src => (IppOperation) src);
                  cfg.CreateIppMap<object, int>();
                  cfg.CreateIppMap<object, bool>();
                  cfg.CreateIppMap<object, DateTimeOffset>();
                  cfg.CreateIppMap<object, Range>();
                  cfg.CreateIppMap<object, Resolution>();
                  cfg.CreateIppMap<object, StringWithLanguage>();
                  cfg.CreateIppMap<string, Compression>();
                  cfg.CreateIppMap<int, IppOperation>();


                  cfg.CreateMap<object, string?>().ConstructUsing((src, __) => src is string i ? i : null);
                  cfg.CreateMap<NoValue, string[]?>().ConstructUsing(_ => null);
                  cfg.CreateMap<string, string[]>().ConstructUsing(src => new[] { src });

              /*  cfg.CreateMap<object, int?>().ConstructUsing((src, __) => src is int i ? i : (int?)null);
                cfg.CreateMap<object, string?>().ConstructUsing((src, __) => src is string i ? i : null);
                cfg.CreateMap<NoValue, string[]?>().ConstructUsing(_ => null);
                cfg.CreateMap<NoValue, Compression[]?>().ConstructUsing(_ => null);
                cfg.CreateMap<object, bool?>().ConstructUsing((src, __) => src is bool i ? i : (bool?)null);
                cfg.CreateMap<object, DateTimeOffset?>().ConstructUsing((src, __) => src is DateTimeOffset i ? i : (DateTimeOffset?)null);
                cfg.CreateMap<object, Range?>().ConstructUsing((src, __) => src is Range i ? i : (Range?)null);
                cfg.CreateMap<object, Resolution?>().ConstructUsing((src, __) => src is Resolution i ? i : (Resolution?)null);
                cfg.CreateMap<object, StringWithLanguage?>().ConstructUsing((src, __) => src is StringWithLanguage i ? i : (StringWithLanguage?)null);

                cfg.CreateMap<string, string[]>().ConstructUsing(src => new[] { src });
                cfg.CreateMap<string, Compression[]?>().ConstructUsing((src, ctx) =>
                    ctx.Mapper.Map<Compression[]?>(new[] { src }));
                cfg.CreateMap<string, Compression[]>().ConstructUsing((src, ctx) =>
                    ctx.Mapper.Map<Compression[]>(new[] { src }));
              */




                cfg.CreateMap<object, string?>().ConstructUsing((src, __) => src is string i ? i : null);
                cfg.CreateMap<NoValue, string[]?>().ConstructUsing(_ => null);
                cfg.CreateMap<string, string[]>().ConstructUsing(src => new[] { src });

                ConfigureJobHoldUntil(cfg);
                ConfigureMultipleDocumentHandling(cfg);
                ConfigureSides(cfg);
                ConfigureJobSheets(cfg);
                ConfigureCompression(cfg);
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
    }
}