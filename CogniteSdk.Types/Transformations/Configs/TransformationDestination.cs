// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Text.Json.Serialization;

namespace CogniteSdk
{
    /// <summary>
    /// Where result of a transformation is written to.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TransformationDestinationType
    {
        /// <summary>
        /// Assets with no extra logic.
        /// </summary>
        assets = 0,
        /// <summary>
        /// Timeseries
        /// </summary>
        timeseries = 1,
        /// <summary>
        /// Construct an asset hierarchy from the result.
        /// </summary>
        asset_hierarchy = 2,
        /// <summary>
        /// Events
        /// </summary>
        events = 3,
        /// <summary>
        /// Numerical datapoints.
        /// </summary>
        datapoints = 4,
        /// <summary>
        /// String datapoints.
        /// </summary>
        string_datapoints = 5,
        /// <summary>
        /// Sequences
        /// </summary>
        sequences = 6,
        /// <summary>
        /// Files
        /// </summary>
        files = 7,
        /// <summary>
        /// Labels
        /// </summary>
        labels = 8,
        /// <summary>
        /// Relationships
        /// </summary>
        relationships = 9,
        /// <summary>
        /// Raw, requires specifying table and database.
        /// </summary>
        raw = 10,
        /// <summary>
        /// Data sets.
        /// </summary>
        data_sets = 11
    }


    /// <summary>
    /// Object describing the destination of a transformation.
    /// </summary>
    public class TransformationDestination
    {
        /// <summary>
        /// Type of destination.
        /// </summary>
        public TransformationDestinationType Type { get; set; }

        /// <summary>
        /// Raw database. Only valid if Type is "raw".
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// Raw table. Only valid if Type is "raw".
        /// </summary>
        public string Table { get; set; }
    }
}
