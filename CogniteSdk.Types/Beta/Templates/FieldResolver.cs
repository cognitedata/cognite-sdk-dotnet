// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Type of field resolver
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FieldResolverType
    {
        /// <summary>
        /// Constant field resolver
        /// </summary>
        constant,
        /// <summary>
        /// CDF Raw field resolver
        /// </summary>
        raw,
        /// <summary>
        /// Template field resolver
        /// </summary>
        template,
        /// <summary>
        /// Time series field resolver
        /// </summary>
        timeSeries,
        /// <summary>
        /// Synthetic time series field resolver
        /// </summary>
        syntheticTimeSeries,
        /// <summary>
        /// Search file resolver
        /// </summary>
        search,
        /// <summary>
        /// View field resolver
        /// </summary>
        view
    }

    /// <summary>
    /// Json converter for field resolvers
    /// </summary>
    public class FieldResolverConverter : JsonConverter<BaseFieldResolver>
    {
        /// <inheritdoc />
        public override BaseFieldResolver Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var document = JsonDocument.ParseValue(ref reader);

            var typeProp = document.RootElement.GetProperty("type").GetString();
            if (!Enum.TryParse<FieldResolverType>(typeProp, true, out var type))
            {
                return null;
            }

            switch (type)
            {
                case FieldResolverType.constant:
                    return document.Deserialize<ConstantFieldResolver>(options);
                case FieldResolverType.raw:
                    return document.Deserialize<RawFieldResolver>(options);
                case FieldResolverType.template:
                    return document.Deserialize<TemplateFieldResolver>(options);
                case FieldResolverType.timeSeries:
                    return document.Deserialize<TimeSeriesFieldResolver>(options);
                case FieldResolverType.syntheticTimeSeries:
                    return document.Deserialize<SyntheticTimeSeriesFieldResolver>(options);
                case FieldResolverType.search:
                    return document.Deserialize<SearchFieldResolver>(options);
                case FieldResolverType.view:
                    return document.Deserialize<ViewFieldResolver>(options);
            }

            return null;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, BaseFieldResolver value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.GetType(), options);
        }
    }


    /// <summary>
    /// Base class for field resolvers
    /// </summary>
    public abstract class BaseFieldResolver
    {
        /// <summary>
        /// Type of field resolver
        /// </summary>
        public abstract FieldResolverType Type { get; }
    }

    /// <summary>
    /// Field resolver with a JSON constant
    /// </summary>
    public class ConstantFieldResolver : BaseFieldResolver
    {
        /// <summary>
        /// Constant value
        /// </summary>
        public JsonElement Value { get; set; }

        /// <inheritdoc />
        public override FieldResolverType Type => FieldResolverType.constant;
    }

    /// <summary>
    /// Field resolver for a specific CDF Raw value
    /// </summary>
    public class RawFieldResolver : BaseFieldResolver
    {
        /// <summary>
        /// Name of database
        /// </summary>
        public string DbName { get; set; }

        /// <summary>
        /// Name of table
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Name of row
        /// </summary>
        public string RowKey { get; set; }

        /// <summary>
        /// Name of column
        /// </summary>
        public string ColumnName { get; set; }

        /// <inheritdoc />
        public override FieldResolverType Type => FieldResolverType.raw;
    }

    /// <summary>
    /// Field resolver for a template
    /// </summary>
    public class TemplateFieldResolver : BaseFieldResolver
    {
        /// <summary>
        /// Name of template
        /// </summary>
        public string TemplateName { get; set; }

        /// <inheritdoc />
        public override FieldResolverType Type => FieldResolverType.template;
    }

    /// <summary>
    /// Field resolver for a timeseries
    /// </summary>
    public class TimeSeriesFieldResolver : BaseFieldResolver
    {
        /// <summary>
        /// Time series id
        /// </summary>
        public string TimeSeriesId { get; set; }

        /// <inheritdoc />
        public override FieldResolverType Type => FieldResolverType.timeSeries;
    }

    /// <summary>
    /// Field resolver for a synthetic timeseries
    /// </summary>
    public class SyntheticTimeSeriesFieldResolver : BaseFieldResolver
    {
        /// <summary>
        /// Synthetic timeseries expression.
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        /// Synthetic timeseries name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Synthetic timeseries metadata.
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// Synthetic timeseries description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// True if synthetic timeseries uses discrete values.
        /// </summary>
        public bool IsStep { get; set; }

        /// <summary>
        /// True if synthetic timeseries is string.
        /// </summary>
        public bool IsString { get; set; }

        /// <summary>
        /// Synthetic timeseries unit.
        /// </summary>
        public string Unit { get; set; }

        /// <inheritdoc />
        public override FieldResolverType Type => FieldResolverType.syntheticTimeSeries;
    }

    /// <summary>
    /// Field resolver for template search.
    /// </summary>
    public class SearchFieldResolver : BaseFieldResolver
    {
        /// <summary>
        /// Template name.
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        /// Search parameters.
        /// </summary>
        public Dictionary<string, string> SearchParams { get; set; }

        /// <summary>
        /// Filter parameters.
        /// </summary>
        public Dictionary<string, JsonElement> FilterParams { get; set; }

        /// <inheritdoc />
        public override FieldResolverType Type => FieldResolverType.search;
    }

    /// <summary>
    /// Field resolver for a template view.
    /// </summary>
    public class ViewFieldResolver : BaseFieldResolver
    {
        /// <summary>
        /// View externalId.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// View input.
        /// </summary>
        public Dictionary<string, JsonElement> Input { get; set; }

        /// <inheritdoc />
        public override FieldResolverType Type => FieldResolverType.view;
    }
}
