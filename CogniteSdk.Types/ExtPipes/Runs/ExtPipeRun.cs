using System;
using System.Collections.Generic;
using System.Text;

namespace CogniteSdk
{
    /// <summary>
    /// Extraction pipeline run.
    /// </summary>
    public class ExtPipeRun
    {
        /// <summary>
        /// Server generated ID for the extraction pipeline run.
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Extraction pipeline externalId.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// Extraction pipeline status.
        /// </summary>
        public ExtPipeRunStatus Status { get; set; }
        /// <summary>
        /// Error message.
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Created time, in milliseconds since 00:00:00 Thursday 1 January 1970, minus leap seconds.
        /// </summary>
        public long CreatedTime { get; set; }
    }
}
