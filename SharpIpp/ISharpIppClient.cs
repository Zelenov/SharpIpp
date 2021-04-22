using System;
using System.Threading.Tasks;
using SharpIpp.Model;

namespace SharpIpp
{
    public interface ISharpIppClient: IDisposable
    {
        Task<PrintJobResponse> PrintJobAsync(PrintJobRequest request);
        Task<GetPrinterAttributesResponse> GetPrinterAttributesAsync(GetPrinterAttributesRequest request);
        Task<GetJobAttributesResponse> GetJobAttributesAsync(GetJobAttributesRequest request);
    }
}