// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk
{
    /// <summary>
    /// Metric describing a transformation job.
    /// </summary>
    public class TransformationJobMetric
    {
        /// <summary>
        /// Time when this metric was recorded:
        /// The number of milliseconds since 00:00:00 Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus leap seconds
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// Name of the metric. Metrics like assets.read mean how many items were fetched from CDF; 
        /// "assets.create" and "assets.update" inform how many resources were created or updated
        /// (note that we count objects as updated even when no field is changed).
        /// "requests indicates how many HTTP request were made to CDF to complete the job.
        /// "requestWithoutRetries" does not count retried requests.
        /// Normally, these two metrics should be almost equal,
        /// if there is a big difference, it may indicate a problem with rate-limiting.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The value of this metric.
        /// </summary>
        public long Count { get; set; }
    }
}
