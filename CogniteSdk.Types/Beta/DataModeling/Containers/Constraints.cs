// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Type of container constraint.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ConstraintType
    {
        /// <summary>
        /// Another container is required.
        /// </summary>
        requires,
        /// <summary>
        /// A list of properties that combined must be unique.
        /// </summary>
        uniqueness
    }
    /// <summary>
    /// Base class for constraint types.
    /// </summary>
    public abstract class BaseConstraint
    {
        /// <summary>
        /// Type of constraint.
        /// </summary>
        public ConstraintType ConstraintType { get; set; }
    }

    /// <summary>
    /// A constraint that indicates that nodes in a container must also implement
    /// another container.
    /// </summary>
    public class RequiresConstraint : BaseConstraint
    {
        /// <summary>
        /// Reference to an existing container.
        /// </summary>
        public ContainerIdentifier Require { get; set; }
    }

    /// <summary>
    /// A constraint that requires each combination of a list of properties to be unique.
    /// </summary>
    public class UniquenessConstraint : BaseConstraint
    {
        /// <summary>
        /// List of properties included in the uniqueness constraint. The order is significant.
        /// </summary>
        public IEnumerable<string> Properties { get; set; }
    }

    /// <summary>
    /// JsonConverter for container constraint variants.
    /// </summary>
    public class ContainerConstraintConverter : JsonConverter<BaseConstraint>
    {
        /// <inheritdoc />
        public override BaseConstraint Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var document = JsonDocument.ParseValue(ref reader);

            var typeProp = document.RootElement.GetProperty("type").GetString();
            if (!Enum.TryParse<ConstraintType>(typeProp, true, out var type))
            {
                return null;
            }
            switch (type)
            {
                case ConstraintType.uniqueness:
                    return document.Deserialize<UniquenessConstraint>(options);
                case ConstraintType.requires:
                    return document.Deserialize<RequiresConstraint>(options);
            }
            return null;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, BaseConstraint value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}
