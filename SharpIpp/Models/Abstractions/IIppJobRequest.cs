using System;

namespace SharpIpp.Models
{
    public interface IIppJobRequest : IIppRequest
    {
        Uri? JobUrl { get; set; }

        int? JobId { get; set; }
    }
}
