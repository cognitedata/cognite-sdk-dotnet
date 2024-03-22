// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// Type of simulator value.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SimulatorValueType
    {
        /// String value
        [SuppressMessage("Naming", "CA1720: Identifiers should not contain type names", Justification = "By design.")]
        STRING,
        /// Double value
        [SuppressMessage("Naming", "CA1720: Identifiers should not contain type names", Justification = "By design.")]
        DOUBLE,
        /// Long value
        [SuppressMessage("Naming", "CA1720: Identifiers should not contain type names", Justification = "By design.")]
        LONG
    }
}