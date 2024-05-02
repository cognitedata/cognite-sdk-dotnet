// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace CogniteSdk
{
    /// <summary>
    /// Type of multi value.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MultiValueType
    {
        /// String value
        [SuppressMessage("Naming", "CA1720: Identifiers should not contain type names", Justification = "By design.")]
        STRING,
        /// Double value
        [SuppressMessage("Naming", "CA1720: Identifiers should not contain type names", Justification = "By design.")]
        DOUBLE,
        /// Long value
        [SuppressMessage("Naming", "CA1720: Identifiers should not contain type names", Justification = "By design.")]
        LONG,
        /// Null value
        [SuppressMessage("Naming", "CA1720: Identifiers should not contain type names", Justification = "By design.")]
        NULL
    }
}