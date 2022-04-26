// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk
{
    /// <summary>
    /// Query for fetching a list of config queries
    /// </summary>
    public class ListConfigQuery : CursorQueryBase
    {
        /// <summary>
        /// Extraction pipeline external id. Required
        /// </summary>
        public string ExtPipeId { get; set; }

        /// <inheritdoc />
        public override List<(string, string)> ToQueryParams()
        {
            var param = base.ToQueryParams();
            if (ExtPipeId != null)
            {
                param.Add(("externalId", ExtPipeId));
            }
            return param;
        }
    }
}
