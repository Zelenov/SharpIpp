namespace SharpIpp.Protocol.Models
{
    public class DocumentAttributes
    {
        /// <summary>
        ///     The client OPTIONALLY supplies this attribute.  The Printer
        ///     object MUST support this attribute.  The value 'true' indicates
        ///     that total fidelity to client supplied Job Template attributes
        ///     and values is required, else the Printer object MUST reject the
        ///     Print-Job request.  The value 'false' indicates that a
        ///     reasonable attempt to print the Job object is acceptable and
        ///     the Printer object MUST accept the Print-Job request. If not
        ///     supplied, the Printer object assumes the value is 'false'.  All
        ///     Printer objects MUST support both types of job processing.  See
        ///     section 15 for a full description of "ipp-attribute-fidelity"
        ///     and its relationship to other attributes, especially the
        ///     Printer object's "pdl-override-supported" attribute.
        /// </summary>
        public string? DocumentName { get; set; }

        /// <summary>
        ///     The client OPTIONALLY supplies this attribute.  The Printer
        ///     object MUST support this attribute.  The value of this
        ///     attribute identifies the format of the supplied document data.
        /// </summary>
        /// <example>application/octet-stream</example>
        public string? DocumentFormat { get; set; }

        /// <summary>
        ///     The client OPTIONALLY supplies this attribute.  The Printer
        ///     object OPTIONALLY supports this attribute. This attribute
        ///     specifies the natural language of the document for those
        ///     document-formats that require a specification of the natural
        ///     language in order to image the document unambiguously. There
        ///     are no particular values required for the Printer object to
        ///     support.
        /// </summary>
        public string? DocumentNaturalLanguage { get; set; }

        /// <summary>
        ///     The client OPTIONALLY supplies this attribute.  The Printer
        ///     object MUST support this attribute and the "compression-
        ///     supported" attribute (see section 4.4.32).  The client supplied
        ///     "compression" operation attribute identifies the compression
        ///     algorithm used on the document data. The following cases exist:
        ///     a) If the client omits this attribute, the Printer object MUST
        ///     assume that the data is not compressed   (i.e. the Printer
        ///     follows the rules below as if the client supplied the
        ///     "compression" attribute with a value of 'none').
        ///     b) If the client supplies this attribute, but the value is not
        ///     supported by the Printer object, i.e., the value is not one
        ///     of the values of the Printer object's "compression-
        ///     supported" attribute, the Printer object MUST reject the
        ///     request, and return the 'client-error-compression-not-
        ///     supported' status code. See section 3.1.7 for returning
        ///     unsupported attributes and values.
        ///     c) If the client supplies the attribute and the Printer object
        ///     supports the attribute value, the Printer object uses the
        ///     corresponding decompression algorithm on the document data.
        ///     d) If the decompression algorithm fails before the Printer
        ///     returns an operation response, the Printer object MUST
        ///     reject the request and return the 'client-error-
        ///     compression-error' status code.
        ///     e) If the decompression algorithm fails after the Printer
        ///     returns an operation response, the Printer object MUST abort
        ///     the job and add the 'compression-error' value to the job's
        ///     "job-state-reasons" attribute.
        ///     f) If the decompression algorithm succeeds, the document data
        ///     MUST then have the format specified by the job's "document-
        ///     format" attribute, if supplied (see "document-format"
        ///     operation attribute definition below).
        /// </summary>
        public Compression? Compression { get; set; }
    }
}
