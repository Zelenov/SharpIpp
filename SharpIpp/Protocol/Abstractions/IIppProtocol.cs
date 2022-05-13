using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SharpIpp.Protocol
{
    internal interface IIppProtocol
    {
        Task<IIppResponseMessage> ReadIppResponseAsync(Stream stream, CancellationToken cancellationToken = default);

        Task WriteIppRequestAsync(IIppRequestMessage ippRequestMessage, Stream stream, CancellationToken cancellationToken = default);
    }
}
