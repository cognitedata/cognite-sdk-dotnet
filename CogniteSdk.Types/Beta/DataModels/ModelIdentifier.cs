using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Identifier for an FDM model, serializes to an array of one or two externalIds
    /// </summary>
    public class ModelIdentifier
    {
        /// <summary>
        /// Space externalId. Null for "edge" or "node" models.
        /// </summary>
        public string Space { get; }

        /// <summary>
        /// Model externalId.
        /// </summary>
        public string Model { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="space">Space externalId. Null for "edge" or "node" models.</param>
        /// <param name="model">Model externalId.</param>
        /// <exception cref="ArgumentNullException">If model is null</exception>
        public ModelIdentifier(string space, string model)
        {
            Space = space;
            if (model == null) throw new ArgumentNullException(nameof(model));
            Model = model;
        }

        /// <summary>
        /// Identifier for the edge model
        /// </summary>
        public static ModelIdentifier Edge { get; } = new ModelIdentifier(null, "edge");

        /// <summary>
        /// Identifier for the node model
        /// </summary>
        public static ModelIdentifier Node { get; } = new ModelIdentifier(null, "node");

        /// <summary>
        /// Create a property identifier for a property in this model
        /// </summary>
        /// <param name="property">Property externalId</param>
        /// <returns>PropertyIdentifier for property in this model with externalId <paramref name="property"/></returns>
        public PropertyIdentifier Property(string property)
        {
            return new PropertyIdentifier(this, property);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is ModelIdentifier other && other.Model == Model && other.Space == Space;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (Space, Model).GetHashCode();
        }
    }

    /// <summary>
    /// JsonConverter for ModelIdentifier
    /// </summary>
    public class ModelIdentifierConverter : JsonConverter<ModelIdentifier>
    {
        /// <inheritdoc />
        public override ModelIdentifier Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException($"JsonTokenType was of type {reader.TokenType}, must be StartArray");
            }

            var arr = JsonSerializer.Deserialize<string[]>(ref reader, options);
            if (arr.Length == 1)
            {
                return new ModelIdentifier(null, arr[0]);
            }
            else if (arr.Length == 2)
            {
                return new ModelIdentifier(arr[0], arr[1]);
            }
            else
            {
                throw new JsonException($"Expected array of length 1 or 2, got {arr.Length}");
            }
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, ModelIdentifier value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            if (value.Space != null) writer.WriteStringValue(value.Space);
            writer.WriteStringValue(value.Model);
            writer.WriteEndArray();
        }
    }

    /// <summary>
    /// Identifier for an FDM property, serializes to an array with two or three externalIds.
    /// </summary>
    public class PropertyIdentifier : ModelIdentifier
    {
        /// <summary>
        /// Property externalId.
        /// </summary>
        public string PropertyId { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="space">Space externalId. Null for "edge" or "node" models.</param>
        /// <param name="model">Model externalId.</param>
        /// <param name="property">Property externalId.</param>
        /// <exception cref="ArgumentNullException">If property or model is null</exception>
        public PropertyIdentifier(string space, string model, string property) : base(space, model)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));
            PropertyId = property;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="model">Model identifier.</param>
        /// <param name="property">Property externalId.</param>
        /// <exception cref="ArgumentNullException">If property or model is null</exception>
        public PropertyIdentifier(ModelIdentifier model, string property) : this(model.Space, model.Model, property)
        {
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is PropertyIdentifier other && other.Model == Model && other.Space == Space && other.PropertyId == PropertyId;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (Space, Model, PropertyId).GetHashCode();
        }
    }

    /// <summary>
    /// JsonConverter for PropertyIdentifier
    /// </summary>
    public class PropertyIdentifierConverter : JsonConverter<PropertyIdentifier>
    {
        /// <inheritdoc />
        public override PropertyIdentifier Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException($"JsonTokenType was of type {reader.TokenType}, must be StartArray");
            }

            var arr = JsonSerializer.Deserialize<string[]>(ref reader, options);
            if (arr.Length == 2)
            {
                return new PropertyIdentifier(null, arr[0], arr[1]);
            }
            else if (arr.Length == 3)
            {
                return new PropertyIdentifier(arr[0], arr[1], arr[2]);
            }
            else
            {
                throw new JsonException($"Expected array of length 2 or 3, got {arr.Length}");
            }
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, PropertyIdentifier value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            if (value.Space != null) writer.WriteStringValue(value.Space);
            writer.WriteStringValue(value.Model);
            writer.WriteEndArray();
        }
    }
}
