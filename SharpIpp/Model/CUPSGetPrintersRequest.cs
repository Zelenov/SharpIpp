using System;
using System.Collections.Generic;

namespace SharpIpp.Model
{
    public class CUPSGetPrintersRequest : IIppPrinterRequest
    {
        /// The client OPTIONALLY supplies this attribute to select the 
        /// first printer that is returned. 
        public string? FirstPrinterName { get; set; }

        /// The client OPTIONALLY supplies this attribute.  The Printer
        /// object MUST support this attribute. It is an integer value that
        /// determines the maximum number of jobs that a client will
        /// receive from the Printer even if "which-jobs" or "my-jobs"
        /// constrain which jobs are returned.  The limit is a "stateless
        /// limit" in that if the value supplied by the client is 'N', then
        /// only the first 'N' jobs are returned in the Get-Jobs Response.
        /// There is no mechanism to allow for the next 'M' jobs after the
        /// first 'N' jobs.  If the client does not supply this attribute,
        /// the Printer object responds with all applicable jobs.
        public int? Limit { get; set; }


        ///The client OPTIONALLY supplies this attribute to select which printer is returned.
        public int? PrinterID { get; set; }

        ///The client OPTIONALLY supplies this attribute to select which printers are returned.
        public string? PrinterLocation { get; set; }


        ///The client OPTIONALLY supplies a printer type enumeration to select which printers are returned.
        public PrinterType? PrinterType { get; set; }

        ///The client OPTIONALLY supplies a printer type mask enumeration to select which bits are used in the "printer-type" attribute.
        public PrinterType? PrinterTypeMask { get; set; }

        /// <summary>
        ///     The client OPTIONALLY supplies this attribute.  The Printer
        ///     object MUST support this attribute.  It is a set of Job
        ///     attribute names and/or attribute groups names in whose values
        ///     the requester is interested.  This set of attributes is
        ///     returned for each Job object that is returned.  The allowed
        ///     attribute group names are the same as those defined in the
        ///     Get-Job-Attributes operation in section 3.3.4.  If the client
        ///     does not supply this attribute, the Printer MUST respond as if
        ///     the client had supplied this attribute with two values: 'job-
        ///     uri' and 'job-id'.
        /// </summary>
        public string[]? RequestedAttributes { get; set; }

        public IppVersion Version { get; set; } = IppVersion.CUPS10;
        public int RequestId { get; set; } = 1;
        public Uri PrinterUri { get; set; } = null!;

        /// <summary>
        ///     The "requesting-user-name" (name(MAX)) attribute SHOULD be
        ///     supplied by the client
        /// </summary>
        public string? RequestingUserName { get; set; }

        public IEnumerable<IppAttribute>? AdditionalOperationAttributes { get; set; }
        public IEnumerable<IppAttribute>? AdditionalJobAttributes { get; set; }
    }

    public enum PrinterType
    {
        Printer
    }
}