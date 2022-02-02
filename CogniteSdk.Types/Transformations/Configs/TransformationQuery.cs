// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk
{
    /// <summary>
    /// Query for basic listing of transformations.
    /// </summary>
    public class TransformationQuery : CursorQueryBase
    {
        /// <summary>
        /// Whether public transformations should be included in the results. Default is true.
        /// </summary>
        public bool? IncludePublic { get; set; }

        /// <summary>
        /// Whether transformations should contain information about jobs. Default is true.
        /// </summary>
        public bool? WithJobDetails { get; set; }

        /// <inheritdoc />
        public override List<(string, string)> ToQueryParams()
        {
            var list = base.ToQueryParams();
            if (IncludePublic.HasValue)
                list.Add(("includePublic", IncludePublic.ToString()));
            if (WithJobDetails.HasValue)
                list.Add(("withJobDetails", WithJobDetails.ToString()));

            return list;
        }
    }
}
