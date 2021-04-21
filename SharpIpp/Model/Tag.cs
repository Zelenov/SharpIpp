namespace SharpIpp.Model
{
    /// <summary>
    ///     https://tools.ietf.org/html/rfc8010#section-3.5.2
    ///     https://tools.ietf.org/html/rfc8010#section-3.5.1
    /// </summary>
    public enum Tag : byte
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
        UnsupportedAttributesTag = 0x05,

        /// <summary>
        ///     unsupported
        /// </summary>
        Unsupported = 0x10,

        /// <summary>
        ///     unknown
        /// </summary>
        Unknown = 0x12,

        /// <summary>
        ///     no-value
        /// </summary>
        NoValue = 0x13,
        IntegerUnassigned1 = 0x20,

        /// <summary>
        ///     integer
        /// </summary>
        Integer = 0x21,

        /// <summary>
        ///     boolean
        /// </summary>
        Boolean = 0x22,

        /// <summary>
        ///     enum
        /// </summary>
        Enum = 0x23,
        IntegerUnassigned2 = 0x24,
        IntegerUnassigned3 = 0x25,
        IntegerUnassigned4 = 0x26,
        IntegerUnassigned5 = 0x27,
        IntegerUnassigned6 = 0x28,
        IntegerUnassigned7 = 0x29,
        IntegerUnassigned8 = 0x2A,
        IntegerUnassigned9 = 0x2B,
        IntegerUnassigned10 = 0x2C,
        IntegerUnassigned11 = 0x2D,
        IntegerUnassigned12 = 0x2E,
        IntegerUnassigned13 = 0x2F,

        /// <summary>
        ///     octetString with an unspecified format
        /// </summary>
        OctetStringWithAnUnspecifiedFormat = 0x30,

        /// <summary>
        ///     dateTime
        /// </summary>
        DateTime = 0x31,

        /// <summary>
        ///     resolution
        /// </summary>
        Resolution = 0x32,

        /// <summary>
        ///     rangeOfInteger
        /// </summary>
        RangeOfInteger = 0x33,

        /// <summary>
        ///     begCollection
        /// </summary>
        BegCollection = 0x34,

        /// <summary>
        ///     textWithLanguage
        /// </summary>
        TextWithLanguage = 0x35,

        /// <summary>
        ///     nameWithLanguage
        /// </summary>
        NameWithLanguage = 0x36,

        /// <summary>
        ///     endCollection
        /// </summary>
        EndCollection = 0x37,
        OctetStringUnassigned1 = 0x38,
        OctetStringUnassigned2 = 0x39,
        OctetStringUnassigned3 = 0x3a,
        OctetStringUnassigned4 = 0x3b,
        OctetStringUnassigned5 = 0x3c,
        OctetStringUnassigned6 = 0x3d,
        OctetStringUnassigned7 = 0x3e,
        OctetStringUnassigned8 = 0x3f,

        StringUnassigned1 = 0x40,

        /// <summary>
        ///     textWithoutLanguage
        /// </summary>
        TextWithoutLanguage = 0x41,

        /// <summary>
        ///     nameWithoutLanguage
        /// </summary>
        NameWithoutLanguage = 0x42,

        StringUnassigned2 = 0x43,

        /// <summary>
        ///     keyword
        /// </summary>
        Keyword = 0x44,

        /// <summary>
        ///     uri
        /// </summary>
        Uri = 0x45,

        /// <summary>
        ///     uriScheme
        /// </summary>
        UriScheme = 0x46,

        /// <summary>
        ///     charset
        /// </summary>
        Charset = 0x47,

        /// <summary>
        ///     naturalLanguage
        /// </summary>
        NaturalLanguage = 0x48,

        /// <summary>
        ///     mimeMediaType
        /// </summary>
        MimeMediaType = 0x49,

        /// <summary>
        ///     memberAttrName
        /// </summary>
        MemberAttrName = 0x4a,
        StringUnassigned3 = 0x4b,
        StringUnassigned4 = 0x4c,
        StringUnassigned5 = 0x4d,
        StringUnassigned6 = 0x4e,
        StringUnassigned7 = 0x4f,
        StringUnassigned8 = 0x50,
        StringUnassigned9 = 0x51,
        StringUnassigned10 = 0x52,
        StringUnassigned11 = 0x53,
        StringUnassigned12 = 0x54,
        StringUnassigned13 = 0x55,
        StringUnassigned14 = 0x56,
        StringUnassigned15 = 0x57,
        StringUnassigned16 = 0x58,
        StringUnassigned17 = 0x59,
        StringUnassigned18 = 0x5a,
        StringUnassigned19 = 0x5b,
        StringUnassigned20 = 0x5c,
        StringUnassigned21 = 0x5d,
        StringUnassigned22 = 0x5e,
        StringUnassigned23 = 0x5f
    }
}