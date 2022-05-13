using System.Threading;
using System.Threading.Tasks;

using SharpIpp.Models;

namespace SharpIpp
{
    public partial class SharpIppClient
    {
        /// <inheritdoc />
        public Task<CUPSGetPrintersResponse> GetCUPSPrintersAsync(CUPSGetPrintersRequest request, CancellationToken cancellationToken) => 
            SendAsync(request, ConstructIppRequest, Construct<CUPSGetPrintersResponse>, cancellationToken);
    }
}
