#nullable disable
namespace SharpIpp.Mapping
{
    internal interface IMapperApplier
    {
        TDest Map<TDest>(object source);

        TDest Map<TDest>(object source, TDest dest);

        TDest Map<TSource, TDest>(TSource source);

        TDest Map<TSource, TDest>(TSource source, TDest dest);
    }
}
