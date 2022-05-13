using System;
using System.Collections.Generic;

using SharpIpp.Protocol.Models;

namespace SharpIpp.Models
{
    /// <summary>
    ///     <a href="https://tools.ietf.org/html/rfc2911#section-3.2.8">Resume-Printer Operation</a>
    ///     This operation allows a client to resume the Printer object
    ///     scheduling jobs on all its devices.  The Printer object MUST remove
    ///     the 'paused' and 'moving-to-paused' values from the Printer object's
    ///     "printer-state-reasons" attribute, if present.  If there are no other
    ///     reasons to keep a device paused (such as media-jam), the IPP Printer
    ///     is free to transition itself to the 'processing' or 'idle' states,
    ///     depending on whether there are jobs to be processed or not,
    ///     respectively, and the device(s) resume processing jobs.
    /// </summary>
    public class ResumePrinterRequest : IIppPrinterRequest
    {
        public IppVersion Version { get; set; } = IppVersion.V11;

        public int RequestId { get; set; } = 1;

        public Uri PrinterUri { get; set; } = null!;

        public string? RequestingUserName { get; set; }

        public IEnumerable<IppAttribute>? AdditionalOperationAttributes { get; set; }

        public IEnumerable<IppAttribute>? AdditionalJobAttributes { get; set; }
    }
}
