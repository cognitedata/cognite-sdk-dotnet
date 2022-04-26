// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk
{
    /// <summary>
    /// Query for fetching a specific config revision.
    /// </summary>
    public class GetConfigQuery : IQueryParams
    {
        /// <summary>
        /// Config revision to fetch. Overrides other query parameters.
        /// </summary>
        public int Revision { get; set; }

        /// <summary>
        /// Filter by configs active at this time.
        /// </summary>
        public long ActiveAtTime { get; set; }

        /// <summary>
        /// Extraction pipeline external id. Required.
        /// </summary>
        public string ExtPipeId { get; set; }

        /// <inheritdoc />
        public List<(string, string)> ToQueryParams()
        {
            var prs = new List<(string, string)>();
            if (Revision > 0) prs.Add(("revision", Revision.ToString()));
            if (ActiveAtTime > 0) prs.Add(("activeAtTime", ActiveAtTime.ToString()));
            if (ExtPipeId != null) prs.Add(("externalId", ExtPipeId));
            return prs;
        }
    }
}
