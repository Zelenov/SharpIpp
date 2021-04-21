using System;
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
        private static readonly IMapper Mapper;

        static IppProtocol()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AllowNullCollections = true;
                cfg.AllowNullDestinationValues = true;
                
                cfg.CreateMap<object, IppOperationType>().ConstructUsing(src => (IppOperationType)(int)src);
                ConfigureJobHoldUntil(cfg);
                ConfigureMultipleDocumentHandling(cfg);
                ConfigureSides(cfg);
                ConfigurePrintJobRequest(cfg);
                ConfigureGetPrinterAttributesResponse(cfg);
           
            });

            Mapper = config.CreateMapper();
        }

        static void ConfigureJobHoldUntil(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<string, JobHoldUntil>().ConstructUsing((src, ctx) => src switch
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

            cfg.CreateMap<JobHoldUntil, string>().ConstructUsing((src, ctx) => src switch
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
        static void ConfigureMultipleDocumentHandling(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<string, MultipleDocumentHandling>().ConstructUsing((src, ctx) => src switch
            {
                "single-document" => MultipleDocumentHandling.SingleDocument,
                "separate-documents-uncollated-copies" => MultipleDocumentHandling.SeparateDocumentsUncollatedCopies,
                "separate-documents-collated-copies" => MultipleDocumentHandling.SeparateDocumentsCollatedCopies,
                "single-document-new-sheet" => MultipleDocumentHandling.SingleDocumentNewSheet,
                _ => MultipleDocumentHandling.Unsupported
            });

            cfg.CreateMap<MultipleDocumentHandling, string>().ConstructUsing((src, ctx) => src switch
            {
                MultipleDocumentHandling.SingleDocument => "single-document",
                MultipleDocumentHandling.SeparateDocumentsUncollatedCopies => "separate-documents-uncollated-copies",
                MultipleDocumentHandling.SeparateDocumentsCollatedCopies => "separate-documents-collated-copies",
                MultipleDocumentHandling.SingleDocumentNewSheet => "single-document-new-sheet",
                _ => "unsupported"
            });
        }

        static void ConfigureSides(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<string, Sides>().ConstructUsing((src, ctx) => src switch
            {
                "one-sided" => Sides.OneSided,
                "two-sided-long-edge" => Sides.TwoSidedLongEdge,
                "two-sided-short-edge" => Sides.TwoSidedShortEdge,
                _ => Sides.Unsupported
            });

            cfg.CreateMap<Sides, string>().ConstructUsing((src, ctx) => src switch
            {
                Sides.OneSided => "one-sided",
                Sides.TwoSidedLongEdge => "two-sided-long-edge",
                Sides.TwoSidedShortEdge => "two-sided-short-edge",
                _ => "unsupported"
            });
        }
    }
}