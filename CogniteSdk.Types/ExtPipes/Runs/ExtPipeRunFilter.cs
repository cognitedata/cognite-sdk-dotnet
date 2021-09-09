using System;
using System.Collections.Generic;
using System.Text;

namespace CogniteSdk
{
    /// <summary>
    /// Filter for strings on extraction pipelines
    /// </summary>
    public class ExtPipeStringFilter
    {
        /// <summary>
        /// Matches strings that contain it, ignoring case.
        /// </summary>
        public string Substring { get; set; }
    }
    /// <summary>
    /// Filter for extraction pipeline runs.
    /// </summary>
    public class ExtPipeRunFilter
    {
        /// <summary>
        /// ExternalId of extraction pipeline
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// List of statuses to match.
        /// </summary>
        public IEnumerable<ExtPipeRunStatus> Statuses { get; set; }
        /// <summary>
        /// Created time range, in milliseconds since 00:00:00 Thursday 1 January 1970, minus leap seconds.
        /// </summary>
        public TimeRange CreatedTime { get; set; }
        /// <summary>
        /// Error message.
        /// </summary>
        public ExtPipeStringFilter Message { get; set; }
    }
}
