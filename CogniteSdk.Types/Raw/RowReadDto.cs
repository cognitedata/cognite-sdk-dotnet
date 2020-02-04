// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;
using System.Collections.Generic;
using System.Text.Json;

namespace CogniteSdk.Raw
{
    /// <summary>
    /// The row read DTO.
    /// </summary>
    public class RowReadDto
    {
        /// <summary>
        /// Row key. Unique in table.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Row data stored as a JSON object.
        /// </summary>
        public Dictionary<string, JsonElement> Columns { get; set; }

        /// <summary>
        /// Unix timestamp in milliseconds of when the row was last updated.
        /// </summary>
        public long LastUpdatedTime { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<RowReadDto>(this);
    }
}
