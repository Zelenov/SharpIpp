#nullable disable

using System;
using System.Collections.Generic;

namespace SharpIpp.Mapping
{
    internal class SimpleMapper : IMapper
    {
        private readonly Dictionary<(Type src, Type dst), Func<object, object, SimpleMapper, object>> _dictionary =
            new Dictionary<(Type src, Type dst), Func<object, object, SimpleMapper, object>>();

        public void CreateMap<TSource, TDest>(Func<TSource, IMapperApplier, TDest> mapFunc)
        {
            CreateMap(typeof(TSource), typeof(TDest), (src, mapper) => mapFunc((TSource)src, mapper));
        }

        public void CreateMap<TSource, TDest>(Func<TSource, TDest, IMapperApplier, TDest> mapFunc)
        {
            CreateMap(typeof(TSource),
                typeof(TDest),
                (src, dst, mapper) => mapFunc((TSource)src, (TDest)dst, mapper));
        }

        public void CreateMap(Type sourceType, Type destType, Func<object, IMapperApplier, object> mapFunc)
        {
            CreateMap(sourceType, destType, (src, dst, mapper) => mapFunc(src, mapper));
        }

        public void CreateMap(Type sourceType, Type destType, Func<object, object, IMapperApplier, object> mapFunc)
        {
            var key = (sourceType, destType);
            var value = mapFunc;
            _dictionary[key] = value;
        }

        public TDest Map<TDest>(object source)
        {
            return Map<TDest>(source, default);
        }

        public TDest Map<TDest>(object source, TDest dest)
        {
            return Map(source, source.GetType(), dest);
        }

        public TDest Map<TSource, TDest>(TSource source)
        {
            return Map<TSource, TDest>(source, default);
        }

        public TDest Map<TSource, TDest>(TSource source, TDest dest)
        {
            return Map(source, typeof(TSource), dest);
        }

        private TDest Map<TDest>(object source, Type sourceType, TDest dest)
        {
            var destType = typeof(TDest);

            if (sourceType == destType)
            {
                return (TDest)source;
            }

            foreach (var (map, type) in PossiblePairs(sourceType, destType))
            {
                switch (type)
                {
                    case MapType.Cast: return (TDest)source;
                    case MapType.Simple:
                        if (!_dictionary.TryGetValue(map, out var mapFunc))
                        {
                            continue;
                        }

                        return (TDest)mapFunc(source, dest, this);
                }
            }

            throw new ArgumentException($"No mapping found for types {sourceType} -> {destType}. Source: {source}");
        }

        private IEnumerable<((Type src, Type dst) map, MapType type)> PossiblePairs(Type sourceType, Type destType)
        {
            var underlying = Nullable.GetUnderlyingType(destType);

            if (underlying != null && underlying == sourceType)
            {
                yield return ((sourceType, destType), MapType.Cast);
            }

            yield return ((sourceType, destType), MapType.Simple);

            foreach (var ifc in sourceType.GetInterfaces())
            {
                yield return ((ifc, destType), MapType.Simple);
            }
        }

        private enum MapType
        {
            Simple,
            Cast,
        }
    }
}
