// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace CogniteSdk.Beta.DataModels.Core
{
    /// <summary>
    /// Representation of a CDF timeseries in core data models.
    /// </summary>
    public class CogniteTimeSeriesBase : CogniteCoreInstanceBase
    {
        /// <summary>
        /// Defines whether the time series is a step series or not.
        /// </summary>
        public bool? IsStep { get; set; }
        /// <summary>
        /// Type of datapoints the time series contains.
        /// </summary>
        public TimeSeriesType? Type { get; set; }
        /// <summary>
        /// The physical unit of the time series as described in the source.
        /// </summary>
        public string SourceUnit { get; set; }
        /// <summary>
        /// Direct relation to unit in the `cdf_units` space.
        /// </summary>
        /// <value></value>
        public DirectRelationIdentifier Unit { get; set; }
        /// <summary>
        /// List of assets associated with this time series.
        /// </summary>
        public IEnumerable<DirectRelationIdentifier> Assets { get; set; }
        /// <summary>
        /// List of activities associated with this time series.
        /// </summary>
        public IEnumerable<DirectRelationIdentifier> Activities { get; set; }
        /// <summary>
        /// List of equipment associated with this time series.
        /// </summary>
        public IEnumerable<DirectRelationIdentifier> Equipment { get; set; }


        /// <summary>
        /// Empty constructor.
        /// </summary>
        public CogniteTimeSeriesBase()
        {
        }
    }

    /// <summary>
    /// Type of datapoints the time series contains.
    /// </summary>
    public enum TimeSeriesType
    {
        /// <summary>
        /// Time series containing string values.
        /// </summary>
        String,
        /// <summary>
        /// Time series containing numeric values.
        /// </summary>
        Numeric,
    }

    /// <summary>
    /// Converts string to TimeSeriesType
    /// </summary>
    public class ObjectToTimeSeriesTypeConverter : JsonConverter<TimeSeriesType?>
    {
        /// <summary>
        /// Reads string into an TimeSeriesType
        /// </summary>
        public override TimeSeriesType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException($"JsonTokenType was of type {reader.TokenType}, must be a string");
            }

            var typeVal = reader.GetString().ToLower();

            switch (typeVal)
            {
                case "numeric":
                    return TimeSeriesType.Numeric;
                case "string":
                    return TimeSeriesType.String;
                default:
                    throw new ArgumentOutOfRangeException(nameof(CogniteTimeSeriesBase.Type), "TimeSeries type can either be numeric or string");
            }
        }

        /// <summary>
        /// Writes a TimeSeriesType to string.
        /// </summary>
        public override void Write(Utf8JsonWriter writer, TimeSeriesType? value, JsonSerializerOptions options)
        {
            if (value == null)
                writer.WriteNullValue();
            else
                writer.WriteStringValue(Enum.GetName(typeof(TimeSeriesType), value).ToLower());
        }
    }
}