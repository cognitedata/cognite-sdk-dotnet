// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The data points query for each individual data point query within the top-level <see cref="DataPointsQuery">DataPointsQuery</see>.
    /// </summary>
    public class IdentityWithBefore : Identity
    {
        /// <summary>
        /// Get datapoints before this time. The format is N[timeunit]-ago where timeunit is w,d,h,m,s. Example: '2d-ago' gets data that is up to 2 days old. You can also specify time in milliseconds since epoch.
        /// </summary>
        public string Before { get; set; }

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