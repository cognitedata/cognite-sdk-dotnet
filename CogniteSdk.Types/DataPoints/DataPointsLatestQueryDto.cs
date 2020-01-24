// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.DataPoints
{
    /// <summary>
    /// The data points query for each individual data point query within the top-level <see cref="DataPointsQuery">DataPointsQuery</see>.
    /// </summary>
    public class IdentityWithBefore
    {
        /// <summary>
        /// Get datapoints before this time. The format is N[timeunit]-ago where timeunit is w,d,h,m,s. Example: '2d-ago' gets data that is up to 2 days old. You can also specify time in milliseconds since epoch.
        /// </summary>
        public string Before { get; set; }

        /// <summary>
        /// A server-generated ID for the object.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The external ID provided by the client. Must be unique for the resource type.
        /// </summary>
        public string ExternalId { get; set; }
    }

    /// <summary>
    /// The top level data points query.
    /// </summary>
    public class DataPointsLatestQueryDto : ItemsWithoutCursor<IdentityWithBefore>
    {
        /// <summary>
        /// If true, then ignore IDs and external IDs that are not found.
        /// </summary>
        public bool? IgnoreUnknownIds { get; set; }
    }
}