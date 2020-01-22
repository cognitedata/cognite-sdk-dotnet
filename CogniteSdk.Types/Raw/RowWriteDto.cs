// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Text.Json;

namespace CogniteSdk.Raw
{
    /// <summary>
    /// Dto to write a row to a table in Raw.
    /// </summary>
    public class RowWriteDto
    {
        /// <summary>
        /// Row key. Unique in table.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Row data stored as a JSON object.
        /// </summary>
        public Dictionary<string, string> Columns { get; set; }
    }
}
