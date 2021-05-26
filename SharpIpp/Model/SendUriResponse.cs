using System.Collections.Generic;

namespace SharpIpp.Model
{
    /// <summary>
    ///     https://tools.ietf.org/html/rfc2911#section-3.3.2
    /// </summary>
    public class SendUriResponse : IIppJobResponse
    {
        public IppVersion Version { get; set; } = IppVersion.V11;
        public IppStatusCode StatusCode { get; set; }
        public int RequestId { get; set; } = 1;
        public string JobUri { get; set; } = null!;
        public int JobId { get; set; }
        public JobState JobState { get; set; }
        public string[] JobStateReasons { get; set; } = null!;
        public string? JobStateMessage { get; set; }
        public int? NumberOfInterveningJobs { get; set; }
        public List<IppSection> Sections { get; } = new List<IppSection>();
    }
}