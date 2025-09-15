﻿// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.DataModels;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Update a time series subscription.
    /// </summary>
    public class SubscriptionUpdate
    {
        /// <summary>
        /// Add or remove time series IDs for this subscription.
        /// 
        /// Not applicable to filter based subscriptions.
        /// </summary>
        public UpdateEnumerable<string> TimeSeriesIds { get; set; }
        /// <summary>
        /// Add or remove time series instance IDs for this subscription.
        /// 
        /// Not applicable to filter based subscriptions.
        /// </summary>
        public UpdateEnumerable<InstanceIdentifier> InstanceIds { get; set; }
        /// <summary>
        /// Update subscription name.
        /// </summary>
        public UpdateNullable<string> Name { get; set; }
        /// <summary>
        /// Update subscription description.
        /// </summary>
        public UpdateNullable<string> Description { get; set; }
        /// <summary>
        /// Set subscription filter.
        /// 
        /// Only applicable to filter based subscriptions.
        /// </summary>
        public Update<IDMSFilter> Filter { get; set; }
    }
}
