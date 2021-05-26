using System;

namespace SharpIpp.Model
{
    public interface IIppJobRequest : IIppRequest
    {
        Uri PrinterUri { get; set; }
        Uri? JobUrl { get; set; }
        int? JobId { get; set; }
    }
}