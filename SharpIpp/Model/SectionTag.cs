namespace SharpIpp.Model
{
    /// <summary>
    ///     https://tools.ietf.org/html/rfc8010#section-3.5.1
    /// </summary>
    public enum SectionTag : byte
    {
        /// <summary>
        ///     Reserved
        /// </summary>
        Reserved = 0x00,

        /// <summary>
        ///     operation-attributes-tag
        /// </summary>
        OperationAttributesTag = 0x01,

        /// <summary>
        ///     job-attributes-tag
        /// </summary>
        JobAttributesTag = 0x02,

        /// <summary>
        ///     end-of-attributes-tag
        /// </summary>
        EndOfAttributesTag = 0x03,

        /// <summary>
        ///     printer-attributes-tag
        /// </summary>
        PrinterAttributesTag = 0x04,

        /// <summary>
        ///     unsupported-attributes-tag
        /// </summary>
        UnsupportedAttributesTag = 0x05
    }
}