// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// Events delete class.
    /// </summary>
    public class EventDelete : ItemsWithoutCursor<Identity>
    {
        /// <summary>
        /// Ignore IDs and external IDs that are not found.
        /// </summary>
        public bool? IgnoreUnknownIds { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
