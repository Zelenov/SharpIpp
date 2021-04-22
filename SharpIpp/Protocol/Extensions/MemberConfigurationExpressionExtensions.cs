using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    }
}