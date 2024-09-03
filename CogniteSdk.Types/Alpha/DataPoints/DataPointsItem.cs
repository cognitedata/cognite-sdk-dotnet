// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Beta.DataModels;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// Data Points item class.
    /// </summary>
    public class DataPointsItem<T> : CogniteSdk.DataPointsItem<T>
    {
        /// <summary>
        /// The instance ID of the time series the data points belong to.
        /// </summary>
        public InstanceIdentifier InstanceId { get; set; }
    }
}