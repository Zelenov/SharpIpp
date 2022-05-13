using System;
using System.Collections.Generic;

using SharpIpp.Protocol.Models;

namespace SharpIpp.Models
{
    /// <summary>
    ///     <a href="https://datatracker.ietf.org/doc/html/rfc2911#section-3.3.4">Get-Job-Attributes Operation</a>
    ///     This REQUIRED operation allows a client to request the values of
    ///     attributes of a Job object and it is almost identical to the Get-
    ///     Printer-Attributes operation.  The only
    ///     differences are that the operation is directed at a Job object rather
    ///     than a Printer object, there is no "document-format" operation
    ///     attribute used when querying a Job object, and the returned attribute
    ///     group is a set of Job object attributes rather than a set of Printer
    ///     object attributes.
    /// </summary>
    public class GetJobAttributesRequest : IIppJobRequest
    {
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

        public IppVersion Version { get; set; } = IppVersion.V11;

        public Uri PrinterUri { get; set; } = null!;

        public int RequestId { get; set; } = 1;

        public Uri? JobUrl { get; set; }

        public int? JobId { get; set; }

        /// <summary>
        ///     The "requesting-user-name" (name(MAX)) attribute SHOULD be
        ///     supplied by the client
        /// </summary>
        public string? RequestingUserName { get; set; }

        public IEnumerable<IppAttribute>? AdditionalOperationAttributes { get; set; }

        public IEnumerable<IppAttribute>? AdditionalJobAttributes { get; set; }
    }
}
