// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

using CogniteSdk.DataModels;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Create a timeseries subscription to a list of timeseries or a filter.
    /// </summary>
    public class SubscriptionCreate
    {
        /// <summary>
        /// Externally provided ID for the subscription. Must be unique.
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
        /// List of external IDs of timeseries that this subscription will listen to.
        /// Not compatible with filter.
        /// </summary>
        public IEnumerable<string> TimeSeriesIds { get; set; }
        /// <summary>
        /// List of instance ids of time series that this subscription will listen to.
        /// Not compatible with filter.
        /// </summary>
        public IEnumerable<InstanceIdentifier> InstanceIds { get; set; }
        /// <summary>
        /// A filter DSL (Domain Specific Language) to define advanced filter queries.
        /// 
        /// Timeseries that match this filter will be included in the subscription.
        /// </summary>
        public IDMSFilter Filter { get; set; }
    }
}
