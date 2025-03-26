// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.DataModels;
using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The data points query for each individual data point query within the
    /// top-level <see cref="DataPointsLatestQuery">DataPointsLatestQuery</see>.
    /// </summary>
    public class IdentityWithBefore : Identity
    {
        /// <summary>
        /// Get datapoints before this time. The format is N[timeunit]-ago where timeunit is w,d,h,m,s. Example: '2d-ago' gets data that is up to 2 days old. You can also specify time in milliseconds since epoch.
        /// </summary>
        public string Before { get; set; }

        /// <summary>
        /// The unit externalId of the data points returned. If the time series does not have a
        /// unitExternalId that can be converted to the targetUnit, an error will be returned. Cannot be used with targetUnitSystem.
        /// </summary>
        public string TargetUnit { get; set; }

        /// <summary>
        /// The unit system of the data points returned. Cannot be used with targetUnit.
        /// </summary>
        public string TargetUnitSystem { get; set; }

        /// <summary>
        /// Show the status code for each data point in the response. Good (code = 0) status codes are always omitted.
        /// 
        /// Default false.
        /// </summary>
        public bool? IncludeStatus { get; set; }

        /// <summary>
        /// Treat data points with a Bad status code as if they do not exist. Set to false to include all data points.
        /// 
        /// Default true.
        /// </summary>
        public bool? IgnoreBadDataPoints { get; set; }

        /// <summary>
        /// Treat data points with Uncertain status codes as Bad. Set to false to to include uncertain data points.
        /// 
        /// Default true.
        /// </summary>
        public bool? TreatUncertainAsBad { get; set; }

        /// <summary>
        /// Creates an identity with a before parameter and externalId set
        /// </summary>
        /// <param name="externalId">The externalId to set</param>
        /// <param name="before">Get datapoints before this time. The format is N[timeunit]-ago where timeunit is w,d,h,m,s</param>
        public IdentityWithBefore(string externalId, string before) : base(externalId)
        {
            Before = before;
        }

        /// <summary>
        /// Creates an identity with a before parameter and internalId set
        /// </summary>
        /// <param name="internalId">The internalId to set</param>
        /// <param name="before">Get datapoints before this time. The format is N[timeunit]-ago where timeunit is w,d,h,m,s</param>
        public IdentityWithBefore(long internalId, string before) : base(internalId)
        {
            Before = before;
        }

        /// <summary>
        /// Creates an identity with a before parameter and instanceId set
        /// </summary>
        /// <param name="instanceId">The instanceId to set</param>
        /// <param name="before">Get datapoints before this time. The format is N[timeunit]-ago where timeunit is w,d,h,m,s</param>
        public IdentityWithBefore(InstanceIdentifier instanceId, string before) : base(instanceId)
        {
            Before = before;
        }

        /// <summary>
        /// Create new external Id and null before.
        /// </summary>
        /// <param name="externalId">External id value</param>
        /// <returns>New external Id.</returns>
        public new static IdentityWithBefore Create(string externalId)
        {
            return new IdentityWithBefore(externalId, null);
        }

        /// <summary>
        /// Create new internal Id and null before.
        /// </summary>
        /// <param name="internalId">Internal id value</param>
        /// <returns>New internal Id.</returns>
        public new static IdentityWithBefore Create(long internalId)
        {
            return new IdentityWithBefore(internalId, null);
        }

        /// <summary>
        /// Create new instance Id and null before.
        /// </summary>
        /// <param name="instanceId">Insatnce id value</param>
        /// <returns>New external Id.</returns>
        public new static IdentityWithBefore Create(InstanceIdentifier instanceId)
        {
            return new IdentityWithBefore(instanceId, null);
        }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<IdentityWithBefore>(this);
    }

    /// <summary>
    /// The top level data points query.
    /// </summary>
    public class DataPointsLatestQuery : ItemsWithoutCursor<IdentityWithBefore>
    {
        /// <summary>
        /// If true, then ignore IDs and external IDs that are not found.
        /// </summary>
        public bool? IgnoreUnknownIds { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<DataPointsLatestQuery>(this);
    }
}
