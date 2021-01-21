// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Text.Json.Serialization;

namespace CogniteSdk
{
    /// <summary>
    /// Enumeration of the source- and targettypes of a relationship
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RelationshipVertexType
    {
        /// Asset type
        Asset,
        /// TimeSeries type
        TimeSeries,
        /// File type
        File,
        /// Event type
        Event,
        /// Sequence type
        Sequence
    }
}
