using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SharpIpp.Model;

#nullable enable
namespace SharpIpp.Protocol.Extensions
{
    internal static class MemberConfigurationExpressionExtensions
    {
        public static void CreateIppMap<T>(this SimpleMapper mapper) where T : notnull
        {
            mapper.CreateIppMap<T, T>((i, _) => i);
        }

        public static void CreateIppMap<TSource, TDestination>(this SimpleMapper mapper,
            Func<TSource, SimpleMapper, TDestination> mapFunc) where TSource : notnull
        {
            Type destType = typeof(TDestination);
            Type srcType = typeof(TSource);
            mapper.CreateMap(mapFunc);
            mapper.CreateMap<NoValue, TDestination[]?>((_, __) => null);
            mapper.CreateMap<object, TDestination>((src, map) =>
            {
                if (src == null)
                    throw new ArgumentException($"Mapping null to non nullable type {typeof(object)} -> {destType}");

                return src is TSource source
                    ? map.Map<TDestination>(source)
                    : throw new ArgumentException($"Mapping not supported {srcType} -> {destType}");
            });

            if (Nullable.GetUnderlyingType(destType) == null)
            {
                var destIsClass = destType.IsClass;
                Type destNullable = destIsClass ? destType : typeof(Nullable<>).MakeGenericType(destType);
                var destNull = destIsClass
                    ? Convert.ChangeType(null, destNullable)
                    : Activator.CreateInstance(destNullable);
                mapper.CreateMap(typeof(NoValue), destNullable, (_, __) => destNull);
                if (!destIsClass)
                    mapper.CreateMap(srcType, destNullable,
                        (src, map) => src == null ? destNull : map.Map<TDestination>(src));
            }

            mapper.CreateMap<TSource, TDestination[]>((src, map) =>
                src != null
                    ? map.Map<TDestination[]>(new[] {src})
                    : throw new ArgumentException($"Mapping null to non nullable type {srcType} -> {destType}"));
            mapper.CreateMap<TSource, TDestination[]?>((src, map) =>
                src == null ? null : map.Map<TDestination[]?>(new[] {src}));
            mapper.CreateMap<TSource[], TDestination[]>((src, map) =>
                src.Select(x => map.Map<TDestination>(x)).ToArray());
            mapper.CreateMap<TSource[], TDestination[]?>((src, map) =>
                src.Select(x => map.Map<TDestination>(x)).ToArray());
            mapper.CreateMap<TSource[]?, TDestination[]?>((src, map) =>
                src?.Select(x => map.Map<TDestination>(x)).ToArray());

            mapper.CreateMap<object[], TDestination[]>((src, map) => src.Select(map.Map<TDestination>).ToArray());
            mapper.CreateMap<object[], TDestination[]?>((src, map) => src.Select(map.Map<TDestination>).ToArray());
            mapper.CreateMap<object[]?, TDestination[]?>((src, map) => src?.Select(map.Map<TDestination>).ToArray());
        }

        public static TDestination MapFromDicSet<TDestination>(this SimpleMapper mapper,
            IDictionary<string, IppAttribute[]> src, string key) where TDestination : IEnumerable
        {
            var mapKey = !src.ContainsKey(key) ? (object) NoValue.Instance : src[key].Select(x => x.Value).ToArray();
            return mapper.Map<TDestination>(mapKey);
        }


        public static TDestination MapFromDicSetNull<TDestination>(this SimpleMapper mapper,
            IDictionary<string, IppAttribute[]> src, string key) where TDestination : IEnumerable?
        {
            var mapKey = !src.ContainsKey(key) ? (object) NoValue.Instance : src[key].Select(x => x.Value).ToArray();
            return mapper.Map<TDestination>(mapKey);
        }

        public static TDestination MapFromDic<TDestination>(this SimpleMapper mapper,
            IDictionary<string, IppAttribute[]> src, string key)
        {
            var mapKey = !src.ContainsKey(key) ? NoValue.Instance : src[key].First().Value;
            return mapper.Map<TDestination>(mapKey);
        }

        public static string? MapFromDicLanguage(this SimpleMapper mapper, IDictionary<string, IppAttribute[]> src,
            string key) =>
            mapper.MapFromDic<StringWithLanguage?>(src, key)?.Language;
    }
}