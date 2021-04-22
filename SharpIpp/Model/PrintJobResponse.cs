using System.Collections.Generic;

namespace SharpIpp.Model
{
    public class PrintJobResponse
    {
        public IppVersion IppVersion { get; set; } = IppVersion.V11;
        public int RequestId { get; set; } = 1;

        /// <summary>
        ///     job-uri
        /// </summary>
        public string JobUri { get; set; } = null!;

        /// <summary>
        ///     job-id
        /// </summary>
        public int JobId { get; set; }

        /// <summary>
        ///     job-state
        /// </summary>
        public JobState JobState { get; set; }

        /// <summary>
        ///     job-state-reasons
        /// </summary>
        public string[] JobStateReasons { get; set; } = null!;

        /// <summary>
        ///     job-state-message
        /// </summary>
        public string? JobStateMessage { get; set; }

        /// <summary>
        ///     number-of-intervening-jobs
        /// </summary>
        public int? NumberOfInterveningJobs { get; set; }

        public IDictionary<string, IppAttribute[]> AllAttributes { get; set; } = null!;
    }
}