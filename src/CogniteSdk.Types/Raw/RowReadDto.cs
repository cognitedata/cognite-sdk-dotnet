// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Text.Json;

namespace CogniteSdk.Raw
{
    public class RowReadDto
    {
        /// <summary>
        /// Row key. Unique in table.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Row data stored as a JSON object.
        /// </summary>
        public IDictionary<string, JsonElement> Columns { get; set; }

        /// <summary>
        /// Unix timestamp in milliseconds of when the row was last updated.
        /// </summary>
        public long LastUpdatedTime { get; set; }
    }
}
