// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CogniteSdk.DataModels
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
    public class ContainerConstraintConverter : IntTaggedUnionConverter<BaseConstraint, ConstraintType>
    {
        /// <inheritdoc />
        protected override string TypePropertyName => "constraintType";

        /// <inheritdoc />
        protected override BaseConstraint DeserializeFromEnum(JsonDocument document, JsonSerializerOptions options, ConstraintType type)
        {
            switch (type)
            {
                case ConstraintType.uniqueness:
                    return document.Deserialize<UniquenessConstraint>(options);
                default:
                    return document.Deserialize<RequiresConstraint>(options);
            }
        }
    }
}
