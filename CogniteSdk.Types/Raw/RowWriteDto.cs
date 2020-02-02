// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "System.Text.Json ignores properties that don't have setters")]
        public Dictionary<string, JsonElement> Columns { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<RowWriteDto>(this);
    }
}
