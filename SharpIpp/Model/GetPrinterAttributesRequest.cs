using System;

namespace SharpIpp.Model
{
    public class GetPrinterAttributesRequest
    {
        public IppVersion IppVersion { get; set; } = IppVersion.V11;
        public int RequestId { get; set; } = 1;
        public Uri PrinterUri { get; set; } = null!;
    }
}