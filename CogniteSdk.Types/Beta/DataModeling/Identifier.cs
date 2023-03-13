// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Type of property reference.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PropertySourceType
    {
        /// View property
        view,
        /// Container property
        container,
    }
    /// <summary>
    /// Base identifier for flexible data models types
    /// </summary>
    public abstract class SourceIdentifier
    {
        /// <summary>
        /// Reference type
        /// </summary>
        public PropertySourceType Type { get; set; }
        /// <summary>
        /// Id of the space that the view or container belongs to
        /// </summary>
        public string Space { get; set; }
        /// <summary>
        /// External ID of the view or container
        /// </summary>
        public string ExternalId { get; set; }
    }
    /// <summary>
    /// Identifier for a flexible data models view.
    /// </summary>
    public class ViewIdentifier : SourceIdentifier, IViewCreateOrReference, IViewDefinitionOrReference
    {
        /// <summary>
        /// Version of the view.
        /// </summary>
        public string Version { get; set; }
    }

    /// <summary>
    /// Identifier for a container.
    /// </summary>
    public class ContainerIdentifier : SourceIdentifier { }

    /// <summary>
    /// JsonConverter for FDMIdentifier. Just deserializes as ViewIdentifier.
    /// </summary>
    public class SourceIdentifierConverter : JsonConverter<SourceIdentifier>
    {
        /// <inheritdoc />
        public override SourceIdentifier Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var document = JsonDocument.ParseValue(ref reader);

            var typeProp = document.RootElement.GetProperty("type").GetString();
            if (!Enum.TryParse<PropertySourceType>(typeProp, true, out var type))
            {
                return null;
            }
            switch (type)
            {
                case PropertySourceType.view:
                    return document.Deserialize<ViewIdentifier>(options);
                case PropertySourceType.container:
                    return document.Deserialize<ContainerIdentifier>(options);
            }
            return null;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, SourceIdentifier value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }

    /// <summary>
    /// Identifier for a direct relation
    /// </summary>
    public class DirectRelationIdentifier
    {
        /// <summary>
        /// Id of the space that the view or container belongs to
        /// </summary>
        public string Space { get; set; }
        /// <summary>
        /// External ID of the view or container
        /// </summary>
        public string ExternalId { get; set; }
    }

    /// <summary>
    /// Identifier for an instance, node or edge.
    /// </summary>
    public class InstanceIdentifier
    {
        /// <summary>
        /// Type of instance
        /// </summary>
        public InstanceType InstanceType { get; set; }
        /// <summary>
        /// ExternalId of instance.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// Space the instance belongs to.
        /// </summary>
        public string Space { get; set; }
    }
}
