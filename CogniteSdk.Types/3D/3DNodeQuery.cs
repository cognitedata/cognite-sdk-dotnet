// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Text.Json;

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The ThreeDModel query class.
    /// </summary>
    public class ThreeDNodeQuery : CursorQueryBase
    {
        /// <summary>
        /// Get sub nodes up to this many levels below the specified node. Depth 0 is the root node.
        /// </summary>
        public int? Depth { get; set; }
        /// <summary>
        /// ID of a node that are the root of the subtree you request (default is the root node).
        /// </summary>
        public long? NodeId { get; set; }
        /// <summary>
        /// Filter for node properties. Only nodes that match all the given properties exactly
        /// will be listed. The filter must be a JSON object with the same format as the properties field.
        /// /// </summary>
        public IDictionary<string, IDictionary<string, string>> Properties { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<ThreeDNodeQuery>(this);

        /// <inheritdoc/>
        public new List<(string, string)> ToQueryParams()
        {
            var list = new List<(string, string)>();
            if (Limit.HasValue)
                list.Add(("limit", Limit.ToString()));
            if (Cursor != null)
                list.Add(("cursor", Cursor));
            if (Depth.HasValue)
                list.Add(("depth", Depth.Value.ToString()));
            if (NodeId.HasValue)
                list.Add(("nodeId", NodeId.Value.ToString()));
            if (Properties != null)
                list.Add(("properties", JsonSerializer.Serialize(Properties)));

            return list;
        }
    }
}