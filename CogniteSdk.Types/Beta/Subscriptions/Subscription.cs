// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Beta.DataModels;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// A timeseries subscription
    /// </summary>
    public class Subscription
    {
        /// <summary>
        /// Externally provided ID for the subscription.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// Human readable name of the subscription.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// A description of the subscription.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Maximum effective parallelism of this subscription
        /// (the number of clients that can read from it concurrently)
        /// will be limited to this number, but a higher partition count will cause
        /// a higher time overhead.
        /// </summary>
        public int PartitionCount { get; set; }
        /// <summary>
        /// Number of timeseries in the subscription.
        /// </summary>
        public int TimeSeriesCount { get; set; }
        /// <summary>
        /// If present, the subscription is defined by this filter.
        /// </summary>
        public IDMSFilter Filter { get; set; }
        /// <summary>
        /// Time when this subscription was created in CDF in milliseconds since Jan 1, 1970.
        /// </summary>
        public long CreatedTime { get; set; }

        /// <summary>
        /// The last time this subscription was updated in CDF, in milliseconds since Jan 1, 1970.
        /// </summary>
        public long LastUpdatedTime { get; set; }
    }
}
