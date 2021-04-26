using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using AutoMapper;
using SharpIpp.Model;

namespace SharpIpp.Protocol.Extensions
{
    internal static class MemberConfigurationExpressionExtensions
    {
        public static void MapFromDicSet<TDestination, TMember>(
            this IMemberConfigurationExpression<IDictionary<string, IppAttribute[]>, TDestination, TMember> config,
            string key) where TMember : IEnumerable
        {
            config.MapFrom(src => !src.ContainsKey(key) ? (object)NoValue.Instance : src[key].Select(x => x.Value));
        }
        public static void CreateIppMap<TSource, TDestination>(this IProfileExpression cfg, bool simpleType = false) where TDestination: struct
        {
            cfg.CreateMap<NoValue, TDestination?>().ConvertUsing((_, __) => null);
            cfg.CreateMap<NoValue, TDestination[]?>().ConvertUsing((_, __) => null);
            if (simpleType)
                cfg.CreateMap<object, TDestination?>().ConvertUsing((src, __) => src is TDestination i ? i : (TDestination?)null);
            else
                cfg.CreateMap<object, TDestination?>().ConvertUsing((src, dst, ctx) => ctx.Mapper.Map<TDestination>(src));
            if (typeof(TSource) != typeof(object))
            {
                cfg.CreateMap<TSource, TDestination[]>()
                   .ConvertUsing((src, dst, ctx) => ctx.Mapper.Map<TDestination[]>(new[] {src}));
                cfg.CreateMap<TSource, TDestination[]?>()
                   .ConvertUsing((src, dst, ctx) => ctx.Mapper.Map<TDestination[]?>(new[] {src}));
            }
        }

        public static void MapFromDicSetNull<TDestination, TMember>(
            this IMemberConfigurationExpression<IDictionary<string, IppAttribute[]>, TDestination, TMember> config,
            string key) where TMember : IEnumerable?
        {
            config.MapFrom(src => !src.ContainsKey(key) ? (object)NoValue.Instance : src[key].Select(x => x.Value));
        }

        public static void MapFromDic<TDestination, TMember>(
            this IMemberConfigurationExpression<IDictionary<string, IppAttribute[]>, TDestination, TMember> config,
            string key)
        {
            config.MapFrom(src => !src.ContainsKey(key) ? (object)NoValue.Instance : src[key].First().Value);
        }
        public static IMappingExpression<IDictionary<string, IppAttribute[]>, TDestination> ForIppMember<TDestination, TMember>(
            this IMappingExpression<IDictionary<string, IppAttribute[]>, TDestination> mappingExpression,
            Expression<Func<TDestination, TMember>> destinationMember, string key)
        {
            return mappingExpression.ForMember(destinationMember, opt => opt.MapFromDic(key));
        }
        public static IMappingExpression<IDictionary<string, IppAttribute[]>, TDestination> ForIppMemberSet<TDestination, TMember>(
            this IMappingExpression<IDictionary<string, IppAttribute[]>, TDestination> mappingExpression,
            Expression<Func<TDestination, TMember>> destinationMember, string key) where TMember : IEnumerable
        {
            return mappingExpression.ForMember(destinationMember, opt => opt.MapFromDicSet(key));
        }
        public static IMappingExpression<IDictionary<string, IppAttribute[]>, TDestination> ForIppMemberSetNull<TDestination, TMember>(
            this IMappingExpression<IDictionary<string, IppAttribute[]>, TDestination> mappingExpression,
            Expression<Func<TDestination, TMember>> destinationMember, string key) where TMember : IEnumerable?
        {
            return mappingExpression.ForMember(destinationMember, opt => opt.MapFromDicSetNull(key));
        }
    }
}