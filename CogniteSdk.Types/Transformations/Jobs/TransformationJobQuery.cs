// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk
{
    /// <summary>
    /// Query for listing jobs
    /// </summary>
    public class TransformationJobQuery : CursorQueryBase
    {
        /// <summary>
        /// List only jobs for the specified transformation by internal id.
        /// </summary>
        public long? TransformationId { get; set; }

        /// <summary>
        /// List only jobs for the specified transformation by external id.
        /// </summary>
        public string TransformationExternalId { get; set; }

        /// <inheritdoc />
        public override List<(string, string)> ToQueryParams()
        {
            var list = base.ToQueryParams();
            if (TransformationId.HasValue)
                list.Add(("transformationId", TransformationId.ToString()));
            if (TransformationExternalId != null)
                list.Add(("transformationExternalId", TransformationExternalId));

            return list;
        }
    }
}
