using System;
using System.Collections.Generic;

namespace SharpIpp.Model
{
    /// <summary>
    /// Request to get a list of printers from a CUPS IPP server
    /// <seealso href="http://www.cups.org/doc/spec-ipp.html#CUPS_GET_PRINTERS"/>
    /// </summary>
    public class CUPSGetPrintersRequest : IIppPrinterRequest
    {
        ///<summary>
        /// The client OPTIONALLY supplies this attribute to select the 
        /// first printer that is returned. 
        /// </summary>
        public string? FirstPrinterName { get; set; }

        ///<summary>
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
        ///</summary>
        public int? Limit { get; set; }

        ///<summary>
        ///The client OPTIONALLY supplies this attribute to select which printer is returned.
        ///</summary>
        public int? PrinterID { get; set; }

        ///<summary>
        ///The client OPTIONALLY supplies this attribute to select which printers are returned.
        ///</summary>
        public string? PrinterLocation { get; set; }

        ///<summary>
        ///The client OPTIONALLY supplies a printer type enumeration to select which printers are returned.
        ///</summary>
        public PrinterType? PrinterType { get; set; }

        ///<summary>
        ///The client OPTIONALLY supplies a printer type mask enumeration to select which bits are used in the "printer-type" attribute.
        ///</summary>
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

    ///<summary>
    ///<para>Type flags for requesting specific types of printers.</para>
    ///<para><see href="https://www.cups.org/doc/spec-ipp.html#printer-type">See here</see> for docs</para>
    ///</summary>
    ///
    [Flags]
    public enum PrinterType
    {
        PrinterClass = 0x1,
        RemoteDestination = 0x2,
        PrintsBlack = 0x4,
        PrintsColor = 0x8,
        TwoSidedPrinting = 0x10,
        Stapler = 0x20,
        FastCopies = 0x40,
        FastCopyCollation = 0x80,
        HolePunch = 0x100,
        Cover = 0x200,
        Binding = 0x400,
        Sorting = 0x800,
        MediaToUSLegalA4 = 0x1000,
        MediaLegalToISOcA2 = 0x2000,
        MediaOverA2= 0x4000,
        MediaUserDefined = 0x8000,
        ImplicitClass = 0x10000,
        DefaultPrinter = 20000,
        FacsimileDevice = 0x40000,
        RejectingJobs = 0x80000,
        DeleteQueue = 0x100000,
        QueueNotShared = 0x200000,
        RequiresAuthentication = 0x400000,
        SupportsCUPSCommandFiles = 0x800000,
        AutomaticallyDescovered = 0x1000000,
        ScannerNoPrint = 0x2000000,
        ScannerPrints = 0x4000000,
        PrinterIs3D = 80000000
    }
}