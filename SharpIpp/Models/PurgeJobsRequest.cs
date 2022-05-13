using System;
using System.Collections.Generic;

using SharpIpp.Protocol.Models;

namespace SharpIpp.Models
{
    /// <summary>
    ///     <a href="https://tools.ietf.org/html/rfc2911#section-3.2.9">Purge-Jobs Operation</a>
    ///     This OPTIONAL operation allows a client to remove all jobs from an
    ///     IPP Printer object, regardless of their job states, including jobs in
    ///     the Printer object's Job History.  After a
    ///     Purge-Jobs operation has been performed, a Printer object MUST return
    ///     no jobs in subsequent Get-Job-Attributes and Get-Jobs responses
    ///     (until new jobs are submitted).
    ///     Whether the Purge-Jobs (and Get-Jobs) operation affects jobs that
    ///     were submitted to the device from other sources than the IPP Printer
    ///     object in the same way that the Purge-Jobs operation affects jobs
    ///     that were submitted to the IPP Printer object using IPP, depends on
    ///     implementation, i.e., on whether the IPP protocol is being used as a
    ///     universal management protocol or just to manage IPP jobs,
    ///     respectively.
    /// </summary>
    public class PurgeJobsRequest : IIppPrinterRequest
    {
        public IppVersion Version { get; set; } = IppVersion.V11;

        public int RequestId { get; set; } = 1;

        public Uri PrinterUri { get; set; } = null!;

        public string? RequestingUserName { get; set; }

        public IEnumerable<IppAttribute>? AdditionalOperationAttributes { get; set; }

        public IEnumerable<IppAttribute>? AdditionalJobAttributes { get; set; }
    }
}
