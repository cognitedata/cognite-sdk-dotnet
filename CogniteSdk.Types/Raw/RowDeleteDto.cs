// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Text.Json;

namespace CogniteSdk.Raw
{
    /// <summary>
    /// Dto with keys of rows to delete in table.
    /// </summary>
    public class RowDeleteDto
    {
        /// <summary>
        /// Row key. Unique in table.
        /// </summary>
        public string Key { get; set; }
    }
}
