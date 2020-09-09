// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Text.Json.Serialization;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Enumeration of the source- and targettypes of a relationship
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RelationshipVertexType
    {
        /// String value
        Asset,
        /// String value
        TimeSeries,
        /// String value
        File,
        /// String value
        Event,
        /// String value
        Sequence
    }
}
