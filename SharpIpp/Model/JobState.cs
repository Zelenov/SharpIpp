namespace SharpIpp.Model
{
    /// <summary>
    ///     https://tools.ietf.org/html/rfc2911#section-4.3.7
    /// </summary>
    public enum JobState
    {
        /// <summary>
        ///     pending
        /// </summary>
        Pending = 3,

        /// <summary>
        ///     pending-held
        /// </summary>
        PendingHeld = 4,

        /// <summary>
        ///     processing
        /// </summary>
        Processing = 5,

        /// <summary>
        ///     processing-stopped
        /// </summary>
        ProcessingStopped = 6,

        /// <summary>
        ///     canceled
        /// </summary>
        Canceled = 7,

        /// <summary>
        ///     aborted
        /// </summary>
        Aborted = 8,

        /// <summary>
        ///     completed
        /// </summary>
        Completed = 9
    }
}