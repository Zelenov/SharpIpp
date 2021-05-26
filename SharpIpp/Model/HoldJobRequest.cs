using System;
using System.Collections.Generic;

namespace SharpIpp.Model
{
    /// <summary>
    ///     https://tools.ietf.org/html/rfc2911#section-3.3.5
    /// </summary>
    public class HoldJobRequest : IIppJobRequest
    {
        public IppVersion Version { get; set; } = IppVersion.V11;
        public int RequestId { get; set; } = 1;
        public Uri PrinterUri { get; set; } = null!;
        public Uri? JobUrl { get; set; }
        public int? JobId { get; set; }
        public string? RequestingUserName { get; set; }
        public IEnumerable<IppAttribute>? AdditionalOperationAttributes { get; set; }
        public IEnumerable<IppAttribute>? AdditionalJobAttributes { get; set; }
    }
}