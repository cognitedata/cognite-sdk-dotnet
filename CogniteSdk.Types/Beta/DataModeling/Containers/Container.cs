using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// A flexible data models container.
    /// </summary>
    public class Container
    {
        /// <summary>
        /// Space the container belongs to.
        /// </summary>
        public string Space { get; set; }
        /// <summary>
        /// ExternalId of the container.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// Human readable name of the container.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Description of the container.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Valid uses for the container.
        /// </summary>
        public FdmUsedFor UsedFor { get; set; }
        /// <summary>
        /// Properties indexed by a local unique identifier.
        /// </summary>
        public Dictionary<string, ContainerPropertyDefinition> Properties { get; set; }
        /// <summary>
        /// Set of indexes to apply to the container.
        /// </summary>
        public Dictionary<string, BaseIndex> Indexes { get; set; }
        /// <summary>
        /// Set of constraints to apply to the container.
        /// </summary>
        public Dictionary<string, BaseConstraint> Constraints { get; set; }
        /// <summary>
        /// Time when this container was created in CDF in milliseconds since Jan 1, 1970.
        /// </summary>
        public long CreatedTime { get; set; }
        /// <summary>
        /// The last time this container was updated in CDF, in milliseconds since Jan 1, 1970.
        /// </summary>
        /// <value></value>
        public long LastUpdatedTime { get; set; }

    }

    /// <summary>
    /// Definition of a container property.
    /// </summary>
    public class ContainerPropertyDefinition
    {
        /// <summary>
        /// Whether this property can be set to null.
        /// </summary>
        public bool Nullable { get; set; } = true;
        /// <summary>
        /// Whether to auto increment the property based on the highest current max value.
        /// Only applicable to properties of type int32 or int64.
        /// </summary>
        public bool AutoIncrement { get; set; }
        /// <summary>
        /// Optional default value for the property.
        /// </summary>
        public IDMSValue DefaultValue { get; set; }
        /// <summary>
        /// Description of the property.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Human readable property name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The data-type to use for the property.
        /// </summary>
        public BasePropertyType Type { get; set; }
    }
}
