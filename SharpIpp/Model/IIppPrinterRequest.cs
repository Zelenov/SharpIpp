using System;

namespace SharpIpp.Model
{
    public interface IIppPrinterRequest : IIppRequest
    {
        public Uri PrinterUri { get; set; }
    }
}