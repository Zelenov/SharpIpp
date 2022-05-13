using System;
using System.Collections.Generic;

using SharpIpp.Protocol.Models;

namespace SharpIpp.Models
{
    /// <summary>
    ///     <a href="https://datatracker.ietf.org/doc/html/rfc2911#section-3.3.5">Hold-Job Operation</a>
    ///     This OPTIONAL operation allows a client to hold a pending job in the
    ///     queue so that it is not eligible for scheduling.  If the Hold-Job
    ///     operation is supported, then the Release-Job operation MUST be
    ///     supported, and vice-versa.  The OPTIONAL "job-hold-until" operation
    ///     attribute allows a client to specify whether to hold the job
    ///     indefinitely or until a specified time period, if supported.
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
