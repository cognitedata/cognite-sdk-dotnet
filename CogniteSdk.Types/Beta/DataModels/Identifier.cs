// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CogniteSdk.Beta.DataModels
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

        /// <summary>
        /// Base constructor
        /// </summary>
        public ViewIdentifier()
        {
            Type = PropertySourceType.view;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="externalId">View externalId</param>
        /// <param name="space">View space</param>
        /// <param name="version">View version</param>
        public ViewIdentifier(string space, string externalId, string version) : this()
        {
            ExternalId = externalId;
            Space = space;
            Version = version;
        }

        /// <summary>
        /// Get an FDMExternalId corresponding to this identifier.
        /// </summary>
        /// <returns></returns>
        public FDMExternalId FDMExternalId()
        {
            return new FDMExternalId(ExternalId, Space, Version);
        }
    }

    /// <summary>
    /// Identifier for a container.
    /// </summary>
    public class ContainerIdentifier : SourceIdentifier
    {
        /// <summary>
        /// Base constructor
        /// </summary>
        public ContainerIdentifier()
        {
            Type = PropertySourceType.container;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="externalId">Container externalId</param>
        /// <param name="space">Container space</param>
        public ContainerIdentifier(string space, string externalId) : this()
        {
            ExternalId = externalId;
            Space = space;
        }

        /// <summary>
        /// Get a ContainerId corresponding to this identifier
        /// </summary>
        /// <returns></returns>
        public ContainerId ContainerId()
        {
            return new ContainerId(ExternalId, Space);
        }
    }

    /// <summary>
    /// JsonConverter for FDMIdentifier. Just deserializes as ViewIdentifier.
    /// </summary>
    public class SourceIdentifierConverter : IntTaggedUnionConverter<SourceIdentifier, PropertySourceType>
    {
        /// <inheritdoc />
        protected override string TypePropertyName => "type";

        /// <inheritdoc />
        protected override SourceIdentifier DeserializeFromEnum(JsonDocument document, JsonSerializerOptions options, PropertySourceType type)
        {
            switch (type)
            {
                case PropertySourceType.view:
                    return document.Deserialize<ViewIdentifier>(options);
                default:
                    return document.Deserialize<ContainerIdentifier>(options);
            }
        }
    }

    /// <summary>
    /// Identifier for a direct relation
    /// </summary>
    public class DirectRelationIdentifier : IDMSValue
    {
        /// <summary>
        /// Id of the space that the node belongs to
        /// </summary>
        public string Space { get; set; }
        /// <summary>
        /// External ID of the node
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public DirectRelationIdentifier() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="space">Node space</param>
        /// <param name="externalId">Node external ID</param>
        public DirectRelationIdentifier(string space, string externalId)
        {
            Space = space;
            ExternalId = externalId;
        }
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

        /// <summary>
        /// Empty constructor
        /// </summary>
        public InstanceIdentifier() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Type of instance</param>
        /// <param name="externalId">ExternalId of instance</param>
        /// <param name="space">Space the instance belongs to</param>
        public InstanceIdentifier(InstanceType type, string space, string externalId)
        {
            InstanceType = type;
            ExternalId = externalId;
            Space = space;
        }
    }

    /// <summary>
    /// General identifier for FDM resources.
    /// </summary>
    public class FDMExternalId
    {
        /// <summary>
        /// Resource external ID
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// Resource space
        /// </summary>
        public string Space { get; set; }
        /// <summary>
        /// Resource version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public FDMExternalId()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="externalId">Resource external ID</param>
        /// <param name="space">Resource space</param>
        /// <param name="version">Resource version</param>
        public FDMExternalId(string externalId, string space, string version)
        {
            ExternalId = externalId;
            Space = space;
            Version = version;
        }
    }

    /// <summary>
    /// Identifier for a container.
    /// </summary>
    public class ContainerId
    {
        /// <summary>
        /// Resource external ID
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// Resource space
        /// </summary>
        public string Space { get; set; }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public ContainerId()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="externalId">Resource external ID</param>
        /// <param name="space">Resource space</param>
        public ContainerId(string externalId, string space)
        {
            ExternalId = externalId;
            Space = space;
        }
    }
}
