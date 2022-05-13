using System;
using System.Collections.Generic;

using SharpIpp.Protocol.Models;

namespace SharpIpp.Models
{
    /// <summary>
    ///     <a href="https://tools.ietf.org/html/rfc2911#section-3.3.3">Cancel-Job Operation</a>
    ///     This REQUIRED operation allows a client to cancel a Print Job from
    ///     the time the job is created up to the time it is completed, canceled,
    ///     or aborted.  Since a Job might already be printing by the time a
    ///     Cancel-Job is received, some media sheet pages might be printed
    ///     before the job is actually terminated.
    /// </summary>
    public class CancelJobRequest : IIppJobRequest
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
