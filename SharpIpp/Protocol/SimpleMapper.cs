#nullable disable
using System;
using System.Collections.Generic;

namespace SharpIpp.Protocol
{
    internal class SimpleMapper
    {
        private readonly Dictionary<(Type src, Type dst), Func<object, SharpIpp.Protocol.SimpleMapper, object>> _dictionary = new Dictionary<(Type src, Type dst), Func<object, SharpIpp.Protocol.SimpleMapper, object>>();

        public void CreateMap<TSource, TDest>(Func<TSource, SimpleMapper, TDest> mapFunc)
        {
            CreateMap(typeof(TSource), typeof(TDest), (src, mapper) => mapFunc((TSource) src, mapper));
        }
        public void CreateMap(Type sourceType, Type destType, Func<object, SimpleMapper, object> mapFunc)
        {
            var key = (sourceType, destType);
            var value = mapFunc;
            _dictionary[key] = value;
        }

        public TDest Map<TDest>(object source)
        {
            var sourceType = source.GetType();
            var destType = typeof(TDest);
            if (Equals(sourceType, destType))
                return (TDest) source;

            foreach (var mapPair in PossiblePairs(sourceType, destType))
            {
                switch (mapPair.type)
                {
                    case MapType.Cast: return (TDest) source;
                    case MapType.Simple:
                        if (!_dictionary.TryGetValue(mapPair.map, out var mapFunc))
                            continue;

                        return (TDest)mapFunc(source, this);
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
            foreach (var ifc in sourceType.GetInterfaces()){
                yield return ((ifc, destType), MapType.Simple);
            }
        }

        private enum MapType
        {
            Simple,
            Cast
        }
    }
}