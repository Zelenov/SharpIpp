using System;
using System.IO;

namespace SharpIpp.Model
{
    public class PrintJobRequest
    {
        public IppVersion IppVersion { get; set; } = IppVersion.V11;
        public int RequestId { get; set; } = 1;
        public Uri PrinterUri { get; set; } = null!;
        public Stream Document { get; set; } = null!;

        /// <summary>
        /// The client OPTIONALLY supplies this attribute.  The Printer
        /// object MUST support this attribute.  It contains the client
        /// supplied Job name.  If this attribute is supplied by the
        /// client, its value is used for the "job-name" attribute of the
        /// newly created Job object.  The client MAY automatically include
        /// any information that will help the end-user distinguish amongst
        /// his/her jobs, such as the name of the application program along
        /// with information from the document, such as the document name,
        /// document subject, or source file name.  If this attribute is
        /// not supplied by the client, the Printer generates a name to use
        /// in the "job-name" attribute of the newly created Job object
        /// </summary>
        public string? JobName { get; set; }
        /// <summary>
        /// The client OPTIONALLY supplies this attribute.  The Printer
        /// object MUST support this attribute.  The value 'true' indicates
        /// that total fidelity to client supplied Job Template attributes
        /// and values is required, else the Printer object MUST reject the
        /// Print-Job request.  The value 'false' indicates that a
        /// reasonable attempt to print the Job object is acceptable and
        /// the Printer object MUST accept the Print-Job request. If not
        /// supplied, the Printer object assumes the value is 'false'.  All
        /// Printer objects MUST support both types of job processing.  See
        /// section 15 for a full description of "ipp-attribute-fidelity"
        /// and its relationship to other attributes, especially the
        /// Printer object's "pdl-override-supported" attribute.
        /// </summary>
        public bool? IppAttributeFidelity { get; set; }
        /// <summary>
        /// The client OPTIONALLY supplies this attribute.  The Printer
        /// object MUST support this attribute.  The value 'true' indicates
        /// that total fidelity to client supplied Job Template attributes
        /// and values is required, else the Printer object MUST reject the
        /// Print-Job request.  The value 'false' indicates that a
        /// reasonable attempt to print the Job object is acceptable and
        /// the Printer object MUST accept the Print-Job request. If not
        /// supplied, the Printer object assumes the value is 'false'.  All
        /// Printer objects MUST support both types of job processing.  See
        /// section 15 for a full description of "ipp-attribute-fidelity"
        /// and its relationship to other attributes, especially the
        /// Printer object's "pdl-override-supported" attribute.
        /// </summary>
        public string? DocumentName { get; set; }
        /// <summary>
        /// The client OPTIONALLY supplies this attribute.  The Printer
        /// object MUST support this attribute.  The value of this
        /// attribute identifies the format of the supplied document data.
        /// </summary>
        public string? DocumentFormat { get; set; }
        /// <summary>
        /// The client OPTIONALLY supplies this attribute.  The Printer
        /// object OPTIONALLY supports this attribute. This attribute
        /// specifies the natural language of the document for those
        /// document-formats that require a specification of the natural
        /// language in order to image the document unambiguously. There
        /// are no particular values required for the Printer object to
        /// support.
        /// </summary>
        public string? DocumentNaturalLanguage { get; set; }
        /// <summary>
        /// This attribute specifies a priority for scheduling the Job. A higher
        /// value specifies a higher priority. The value 1 indicates the lowest
        /// possible priority. The value 100 indicates the highest possible
        /// priority.  Among those jobs that are ready to print, a Printer MUST
        /// print all jobs with a priority value of n before printing those with
        /// a priority value of n-1 for all n.
        /// 
        /// If the Printer object supports this attribute, it MUST always support
        /// the full range from 1 to 100.  No administrative restrictions are
        /// permitted.  This way an end-user can always make full use of the
        /// entire range with any Printer object.  If privileged jobs are
        /// implemented outside IPP/1.1, they MUST have priorities higher than
        /// 100, rather than restricting the range available to end-users.
        /// 
        /// If the client does not supply this attribute and this attribute is
        /// supported by the Printer object, the Printer object MUST use the
        /// value of the Printer object's "job-priority-default" at job
        /// submission time (unlike most Job Template attributes that are used if
        /// necessary at job processing time).
        /// </summary>
        public int? JobPriority { get; set; }
        /// <summary>
        /// This attribute specifies the named time period during which the Job
        /// MUST become a candidate for printing.
        /// </summary>
        public JobHoldUntil? JobHoldUntil { get; set; }
        public MultipleDocumentHandling? MultipleDocumentHandling { get; set; }
        /// <summary>
        /// This attribute specifies the number of copies to be printed.
        /// 
        /// On many devices the supported number of collated copies will be
        /// limited by the number of physical output bins on the device, and may
        /// be different from the number of uncollated copies which can be
        /// supported.
        /// </summary>
        public int? Copies { get; set; }

        /// <summary>
        /// This attribute identifies the finishing operations that the Printer
        /// uses for each copy of each printed document in the Job. For Jobs with
        /// multiple documents, the "multiple-document-handling" attribute
        /// determines what constitutes a "copy" for purposes of finishing.
        /// </summary>
        public Finishings? Finishings { get; set; }
        /// <summary>
        /// This attribute identifies the range(s) of print-stream pages that the
        /// Printer object uses for each copy of each document which are to be
        /// printed.  Nothing is printed for any pages identified that do not
        /// exist in the document(s).  Ranges MUST be in ascending order, for
        /// example: 1-3, 5-7, 15-19 and MUST NOT overlap, so that a non-spooling
        /// Printer object can process the job in a single pass.  If the ranges
        /// are not ascending or are overlapping, the IPP object MUST reject the
        /// request and return the 'client-error-bad-request' status code.  The
        /// attribute is associated with print-stream pages not application-
        /// numbered pages (for example, the page numbers found in the headers
        /// and or footers for certain word processing applications).
        /// 
        /// For Jobs with multiple documents, the "multiple-document-handling"
        /// attribute determines what constitutes a "copy" for purposes of the
        /// specified page range(s).  When "multiple-document-handling" is
        /// 'single-document', the Printer object MUST apply each supplied page
        /// range once to the concatenation of the print-stream pages.  For
        /// example, if there are 8 documents of 10 pages each, the page-range
        /// '41:60' prints the pages in the 5th and 6th documents as a single
        /// document and none of the pages of the other documents are printed.
        /// When "multiple-document- handling" is 'separate-documents-
        /// uncollated-copies' or 'separate-documents-collated-copies', the
        /// Printer object MUST apply each supplied page range repeatedly to each
        /// document copy.  For the same job, the page-range '1:3, 10:10' would
        /// print the first 3 pages and the 10th page of each of the 8 documents
        /// in the Job, as 8 separate documents.
        /// 
        /// In most cases, the exact pages to be printed will be generated by a
        /// device driver and this attribute would not be required.  However,
        /// when printing an archived document which has already been formatted,
        /// the end user may elect to print just a subset of the pages contained
        /// in the document.  In this case, if page-range = n.m is specified, the
        /// first page to be printed will be page n. All subsequent pages of the
        /// document will be printed through and including page m.
        /// 
        /// "page-ranges-supported" is a boolean value indicating whether or not
        /// the printer is capable of supporting the printing of page ranges.
        /// This capability may differ from one PDL to another. There is no
        /// "page-ranges-default" attribute.  If the "page-ranges" attribute is
        /// not supplied by the client, all pages of the document will be
        /// printed.
        /// </summary>
        public Range[]? PageRanges { get; set; }
        /// <summary>
        /// This attribute specifies how print-stream pages are to be imposed
        /// upon the sides of an instance of a selected medium, i.e., an
        /// impression.
        /// </summary>
        public Sides? Sides { get; set; }
        /// <summary>
        /// This attribute specifies the number of print-stream pages to impose
        /// upon a single side of an instance of a selected medium.  For example,
        /// if the value is:
        /// '1'    the Printer MUST place one print-stream page on a single side
        /// of an instance of the selected medium (MAY add some sort
        /// of translation, scaling, or rotation).
        /// '2'    the Printer MUST place two print-stream pages on a single side
        /// of an instance of the selected medium (MAY add some sort
        /// of translation, scaling, or rotation).
        /// '4'    the Printer MUST place four print-stream pages on a single
        /// side of an instance of the selected medium (MAY add some
        /// sort of translation, scaling, or rotation).
        /// 
        /// This attribute primarily controls the translation, scaling and
        /// rotation of print-stream pages.
        /// </summary>
        public int? NumberUp { get; set; }
        /// <summary>
        /// This attribute indicates the desired orientation for printed print-
        /// stream pages; it does not describe the orientation of the client-
        /// supplied print-stream pages.
        /// 
        /// For some document formats (such as 'application/postscript'), the
        /// desired orientation of the print-stream pages is specified within the
        /// document data.  This information is generated by a device driver
        /// prior to the submission of the print job.  Other document formats
        /// (such as 'text/plain') do not include the notion of desired
        /// orientation within the document data.  In the latter case it is
        /// possible for the Printer object to bind the desired orientation to
        /// the document data after it has been submitted.  It is expected that a
        /// Printer object would only support "orientations-requested" for some
        /// document formats (e.g., 'text/plain' or 'text/html') but not others
        /// (e.g., 'application/postscript').  This is no different than any
        /// other Job Template attribute since section 4.2, item 1, points out
        /// that a Printer object may support or not support any Job Template
        /// attribute based on the document format supplied by the client.
        /// However, a special mention is made here since it is very likely that
        /// a Printer object will support "orientation-requested" for only a
        /// subset of the supported document formats.
        /// </summary>
        public OrientationRequested? OrientationRequested { get; set; }

        /// <summary>
        /// 
        /// This attribute identifies the medium that the Printer uses for all
        /// impressions of the Job.
        /// 
        /// The values for "media" include medium-names, medium-sizes, input-
        /// trays and electronic forms so that one attribute specifies the media.
        /// If a Printer object supports a medium name as a value of this
        /// attribute, such a medium name implicitly selects an input-tray that
        /// contains the specified medium.  If a Printer object supports a medium
        /// size as a value of this attribute, such a medium size implicitly
        /// selects a medium name that in turn implicitly selects an input-tray
        /// that contains the medium with the specified size.  If a Printer
        /// object supports an input-tray as the value of this attribute, such an
        /// input-tray implicitly selects the medium that is in that input-tray
        /// at the time the job prints.  This case includes manual-feed input-
        /// trays.  If a Printer object supports an electronic form as the value
        /// of this attribute, such an electronic form implicitly selects a
        /// medium-name that in turn implicitly selects an input-tray that
        /// contains the medium specified by the electronic form.  The electronic
        /// form also implicitly selects an image that the Printer MUST merge
        /// with the document data as its prints each page.
        /// </summary>
        public string? Media { get; set; }
        /// <summary>
        /// This attribute identifies the resolution that Printer uses for the
        /// Job.
        /// </summary>
        public Resolution? PrinterResolution { get; set; }
        /// <summary>
        /// This attribute specifies the print quality that the Printer uses for
        /// the Job.
        /// </summary>
        public PrintQuality? PrintQuality { get; set; }
    }


    public enum PrintQuality
    {
        Unsupported,
        /// <summary>
        /// lowest quality available on the printer
        /// </summary>
        Draft = 3,
        /// <summary>
        /// normal or intermediate quality on the printer
        /// </summary>
        Normal = 3,
        /// <summary>
        /// highest quality available on the printer
        /// </summary>
        High = 3,
    }

    public enum OrientationRequested
    {
        Unsupported,
        /// The content will be imaged across the short edge
        /// of the medium.
        Portrait = 3,
        /// The content will be imaged across the long edge
        /// of the medium.  Landscape is defined to be a rotation of
        /// the print-stream page to be imaged by +90 degrees with
        /// respect to the medium (i.e. anti-clockwise) from the
        /// portrait orientation.  Note:  The +90 direction was
        /// chosen because simple finishing on the long edge is the
        /// same edge whether portrait or landscape
        Landscape = 4,
        /// The content will be imaged across the
        /// long edge of the medium.  Reverse-landscape is defined to
        /// be a rotation of the print-stream page to be imaged by -
        /// 90 degrees with respect to the medium (i.e. clockwise)
        /// from the portrait orientation.  Note: The 'reverse-
        /// landscape' value was added because some applications
        /// rotate landscape -90 degrees from portrait, rather than
        /// +90 degrees.
        ReverseLandscape = 5,
        /// The content will be imaged across the
        /// short edge of the medium.  Reverse-portrait is defined to
        /// be a rotatio of the print-stream page to be imaged by
        /// 180 degrees with respect to the medium from the portrait
        /// orientation.  Note: The 'reverse-portrait' value was
        /// added for use with the "finishings" attribute in cases
        /// where the opposite edge is desired for finishing a
        /// portrait document on simple finishing devices that have
        /// only one finishing position.  Thus a 'text'/plain'
        /// portrait document can be stapled "on the right" by a
        /// simple finishing device as is common use with some middle
        /// eastern languages such as Hebrew.
        ReversePortrait = 6
    }

    public enum Sides
    {
        Unsupported,
        /// 'one-sided': imposes each consecutive print-stream page upon the
        /// same side of consecutive media sheets.
        OneSided,
        /// 'two-sided-long-edge': imposes each consecutive pair of print-
        /// stream pages upon front and back sides of consecutive media
        /// sheets, such that the orientation of each pair of print-stream
        /// pages on the medium would be correct for the reader as if for
        /// binding on the long edge.  This imposition is sometimes called
        /// 'duplex' or 'head-to-head'.
        TwoSidedLongEdge,
        /// 'two-sided-short-edge': imposes each consecutive pair of print-
        /// stream pages upon front and back sides of consecutive media
        /// sheets, such that the orientation of each pair of print-stream
        /// pages on the medium would be correct for the reader as if for
        /// binding on the short edge. 
        TwoSidedShortEdge,
    }

    public enum Finishings
    {
        Unsupported,
        /// <summary>
        /// Perform no finishing
        /// </summary>
        None = 3,
        /// <summary>
        /// Bind the document(s) with one or more staples. The
        /// exact number and placement of the staples is site-
        /// defined.
        /// </summary>
        Staple = 4,
        /// <summary>
        /// This value indicates that holes are required in the
        /// finished document. The exact number and placement of the
        /// holes is site-defined  The punch specification MAY be
        /// satisfied (in a site- and implementation-specific manner)
        /// either by drilling/punching, or by substituting pre-
        /// drilled media.
        /// </summary>
        Punch = 5,
        /// <summary>
        /// This value is specified when it is desired to select
        /// a non-printed (or pre-printed) cover for the document.
        /// This does not supplant the specification of a printed
        /// cover (on cover stock medium) by the document itself.
        /// </summary>
        Cover = 6,
        /// <summary>
        /// This value indicates that a binding is to be applied
        /// to the document; the type and placement of the binding is
        /// site-defined.
        /// </summary>
        Bind = 7,
        /// <summary>
        /// Bind the document(s) with one or more
        /// staples (wire stitches) along the middle fold.  The exact
        /// number and placement of the staples and the middle fold
        /// is implementation and/or site-defined.
        /// </summary>
        SaddleStitch = 8,
        /// <summary>
        /// Bind the document(s) with one or more staples
        /// (wire stitches) along one edge.  The exact number and
        /// placement of the staples is implementation and/or site-
        /// defined.
        /// </summary>
        EdgeStitch = 9,
        /// <summary>
        /// Bind the document(s) with one or more
        /// staples in the top left corner.
        /// </summary>
        StapleTopLeft = 20,
        /// <summary>
        /// Bind the document(s) with one or more
        /// staples in the bottom left corner.
        /// </summary>
        StapleBottomLeft = 21,
        /// <summary>
        /// Bind the document(s) with one or more
        /// staples in the top right corner.
        /// </summary>
        StapleTopRight = 22,
        /// <summary>
        /// Bind the document(s) with one or more
        /// staples in the bottom right corner.
        /// </summary>
        StapleBottomRight = 23,
        /// <summary>
        /// Bind the document(s) with one or more
        /// staples (wire stitches) along the left edge.  The exact
        /// number and placement of the staples is implementation
        /// and/or site-defined.
        /// </summary>
        EdgeStitchLeft = 24,
        /// <summary>
        /// Bind the document(s) with one or more
        /// staples (wire stitches) along the top edge.  The exact
        /// number and placement of the staples is implementation
        /// and/or site-defined.
        /// </summary>
        EdgeStitchTop = 25,
        /// <summary>
        /// Bind the document(s) with one or more
        /// staples (wire stitches) along the right edge.  The exact
        /// number and placement of the staples is implementation
        /// and/or site-defined.
        /// </summary>
        EdgeStitchRight = 26,
        /// <summary>
        /// Bind the document(s) with one or more
        /// staples (wire stitches) along the bottom edge.  The exact
        /// number and placement of the staples is implementation
        /// and/or site-defined.
        /// </summary>
        EdgeStitchBottom = 27,
        /// <summary>
        /// Bind the document(s) with two staples
        /// (wire stitches) along the left edge assuming a portrait
        /// document (see above).
        /// </summary>
        StapleDualLeft = 28,
        /// <summary>
        /// Bind the document(s) with two staples
        /// (wire stitches) along the top edge assuming a portrait
        /// document (see above).
        /// </summary>
        StapleDualTop = 29,
        /// <summary>
        /// Bind the document(s) with two staples
        /// (wire stitches) along the right edge assuming a portrait
        /// document (see above).
        /// </summary>
        StapleDualRight = 30,
        /// <summary>
        /// Bind the document(s) with two staples
        /// (wire stitches) along the bottom edge assuming a portrait
        /// document (see above).
        /// </summary>
        StapleDualBottom = 31,
    }

    public enum MultipleDocumentHandling
    {
        Unsupported,
        /// 'single-document': If a Job object has multiple documents, say,
        /// the document data is called a and b, then the result of
        /// processing all the document data (a and then b) MUST be treated
        /// as a single sequence of media sheets for finishing operations;
        /// that is, finishing would be performed on the concatenation of
        /// the sequences a(*),b(*).  The Printer object MUST NOT force the
        /// data in each document instance to be formatted onto a new
        /// print-stream page, nor to start a new impression on a new media
        /// sheet. If more than one copy is made, the ordering of the sets
        /// of media sheets resulting from processing the document data
        /// MUST be a(*), b(*), a(*), b(*), start on a new media sheet.
        SingleDocument,
        /// 'separate-documents-uncollated-copies': If a Job object has
        /// multiple documents, say, the document data is called a and b,
        /// then the result of processing the data in each document
        /// instance MUST be treated as a single sequence of media sheets
        /// for finishing operations; that is, the sets a(*) and b(*) would
        /// each be finished separately. The Printer object MUST force each
        /// copy of the result of processing the data in a single document
        /// to start on a new media sheet. If more than one copy is made,
        /// the ordering of the sets of media sheets resulting from
        /// processing the document data MUST be a(*), a(*), ..., b(*),
        /// b(*) ... .
        SeparateDocumentsUncollatedCopies,
        /// 'separate-documents-collated-copies': If a Job object has multiple
        /// documents, say, the document data is called a and b, then the
        /// result of processing the data in each document instance MUST be
        /// treated as a single sequence of media sheets for finishing
        /// operations; that is, the sets a(*) and b(*) would each be
        /// finished separately. The Printer object MUST force each copy of
        /// the result of processing the data in a single document to start
        /// on a new media sheet.  If more than one copy is made, the
        /// ordering of the sets of media sheets resulting from processing
        /// the document data MUST be a(*), b(*), a(*), b(*), ... .
        SeparateDocumentsCollatedCopies,
        /// 'single-document-new-sheet':  Same as 'single-document', except
        /// that the Printer object MUST ensure that the first impression
        /// of each document instance in the job is placed on a new media
        /// sheet.  This value allows multiple documents to be stapled
        /// together with a single staple where each document starts on a
        /// new sheet.
        SingleDocumentNewSheet,
    }

    public enum JobHoldUntil
    {
        Unsupported,
        ///<summary>
        ///'no-hold': immediately, if there are not other reasons to hold the job
        ///</summary>
        NoHold,
        ///<summary>
        ///'indefinite':  - the job is held indefinitely, until a client performs a Release-Job (section 3.3.6)
        ///</summary>
        Indefinite,
        ///<summary>
        ///'day-time': during the day
        ///</summary>
        DayTime,
        ///<summary>
        ///'evening': evening
        ///</summary>
        Evening,
        ///<summary>
        ///'night': night
        ///</summary>
        Night,
        ///<summary>
        ///'weekend': weekend
        ///</summary>
        Weekend,
        ///<summary>
        ///'second-shift': second-shift (after close of business)
        ///</summary>
        SecondShift,
        ///<summary>
        ///'third-shift': third-shift (after midnight)
        ///</summary>
        ThirdShift
    }
}