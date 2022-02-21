// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The ThreeDRevision query class.
    /// </summary>
    public class ThreeDRevisionQuery : CursorQueryBase
    {
        /// <summary>
        /// Filter based on whether or not is has published revisions.
        /// </summary>
        public bool? Published { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<ThreeDRevisionQuery>(this);

        /// <inheritdoc/>
        public override List<(string, string)> ToQueryParams()
        {
            var list = new List<(string, string)>();
            if (Limit.HasValue)
                list.Add(("limit", Limit.ToString()));
            if (Cursor != null)
                list.Add(("cursor", Cursor));
            if (Published.HasValue)
                list.Add(("published", Published.Value.ToString()));

            return list;
        }
    }
}