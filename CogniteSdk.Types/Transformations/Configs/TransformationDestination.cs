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
        assets,
        /// <summary>
        /// Timeseries
        /// </summary>
        timeseries,
        /// <summary>
        /// Construct an asset hierarchy from the result.
        /// </summary>
        asset_hierarchy,
        /// <summary>
        /// Events
        /// </summary>
        events,
        /// <summary>
        /// Numerical datapoints.
        /// </summary>
        datapoints,
        /// <summary>
        /// String datapoints.
        /// </summary>
        string_datapoints,
        /// <summary>
        /// Sequences
        /// </summary>
        sequences,
        /// <summary>
        /// Files
        /// </summary>
        files,
        /// <summary>
        /// Labels
        /// </summary>
        labels,
        /// <summary>
        /// Relationships
        /// </summary>
        relationships,
        /// <summary>
        /// Raw, requires specifying table and database.
        /// </summary>
        raw,
        /// <summary>
        /// Data sets.
        /// </summary>
        data_sets
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
