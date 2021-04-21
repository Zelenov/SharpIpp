using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using SharpIpp.Model;

namespace SharpIpp.Protocol.Extensions
{
    internal static class BindingConfigExtensions
    {
        public static void MapFromDicSet<TDestination, TMember>(
            this IMemberConfigurationExpression<IDictionary<string, IppAttribute[]>, TDestination, TMember> config,
            string key) where TMember : IEnumerable
        {
            config.MapFrom(src => !src.ContainsKey(key) ? null as object : src[key].Select(x => x.Value));
        }
        public static void MapFromDicSetNull<TDestination, TMember>(
            this IMemberConfigurationExpression<IDictionary<string, IppAttribute[]>, TDestination, TMember> config,
            string key) where TMember : IEnumerable?
        {
            config.MapFrom(src => !src.ContainsKey(key) ? null as object : src[key].Select(x => x.Value));
        }

        public static void MapFromDic<TDestination, TMember>(
            this IMemberConfigurationExpression<IDictionary<string, IppAttribute[]>, TDestination, TMember> config,
            string key)
        {
            config.MapFrom(src => !src.ContainsKey(key) ? null as object : src[key].First().Value);
        }
    }
}