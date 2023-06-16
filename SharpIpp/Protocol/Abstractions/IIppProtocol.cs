using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SharpIpp.Protocol
{
    internal interface IIppProtocol
    {
        Task<IIppRequestMessage> ReadIppRequestAsync(Stream inputStream, CancellationToken cancellationToken = default);
        Task<IIppResponseMessage> ReadIppResponseAsync(Stream stream, CancellationToken cancellationToken = default);
        Task WriteIppRequestAsync(IIppRequestMessage ippRequestMessage, Stream stream, CancellationToken cancellationToken = default);
        Task WriteIppResponseAsync(IIppResponseMessage message, Stream stream, CancellationToken cancellationToken = default);
    }
}
