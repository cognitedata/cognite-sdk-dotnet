// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Enumeration of the possible uses for a flexible data models view.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ViewUse
    {
        /// <summary>
        /// View applies to nodes only
        /// </summary>
        node,
        /// <summary>
        /// View applies to edges only
        /// </summary>
        edge,
        /// <summary>
        /// View applies to both nodes and edges.
        /// </summary>
        all
    }
    /// <summary>
    /// Possible directions of a connection
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ConnectionDirection
    {
        /// <summary>
        /// Connections pointing outwards.
        /// </summary>
        outwards,
        /// <summary>
        /// Connections pointing inwards.
        /// </summary>
        inwards,
    }
    /// <summary>
    /// A flexible data models view.
    /// </summary>
    public class View
    {
        /// <summary>
        /// External ID uniquely identifying this view.
        /// The values Query, Mutation, Subscription, String,
        /// Int32, Int64, Int, Float32, Float64, Float, Timestamp, JSONObject,
        /// Date, Numeric, Boolean, and PageInfo are reserved.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// Id of the space that the view belongs to.
        /// </summary>
        public string Space { get; set; }
        /// <summary>
        /// Human readable name for the view.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Description of the view.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// A complex filter the contents of the view must match.
        /// </summary>
        public IDMSFilter Filter { get; set; }
        /// <summary>
        /// References to views which this view will inherit from.
        /// 
        /// Note: The order is significant. It is used to deduce the priority when
        /// duplicate property references are encountered.
        /// 
        /// If you do not specify a view version, the most recent version available will be used.
        /// </summary>
        public IEnumerable<ViewIdentifier> Implements { get; set; }
        /// <summary>
        /// Version of the view. Must match the regular expression
        /// ^[a-zA-Z0-9]([a-zA-Z0-9_-]{0,41}[a-zA-Z0-9])?$
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// Time when this view was created in CDF in milliseconds since Jan 1, 1970.
        /// </summary>
        public long CreatedTime { get; set; }
        /// <summary>
        /// The last time this view was updated in CDF, in milliseconds since Jan 1, 1970.
        /// </summary>
        /// <value></value>
        public long LastUpdatedTime { get; set; }
        /// <summary>
        /// Does the view support write operations?
        /// You can write to a view if the view maps all non-nullable properties, and the view has no
        /// relations (filters).
        /// </summary>
        public bool Writable { get; set; }
        /// <summary>
        /// Should this view apply to nodes, edges, or both.
        /// </summary>
        public ViewUse UsedFor { get; set; }
        /// <summary>
        /// List of properties and relations included in this view.
        /// </summary>
        public Dictionary<string, IViewProperty> Properties { get; set; }
    }

    /// <summary>
    /// Interface for possible view property types.
    /// </summary>
    public interface IViewProperty { }

    /// <summary>
    /// Description of a view property.
    /// </summary>
    public class ViewPropertyDefinition : IViewProperty
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
        public IDMSFilterValue DefaultValue { get; set; }
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
        /// <summary>
        /// Reference to an existing container.
        /// </summary>
        public ContainerIdentifier Container { get; set; }
        /// <summary>
        /// The unique identifier, in the referenced container, for the property to map.
        /// </summary>
        public string ContainerPropertyIdentifier { get; set; }
    }

    /// <summary>
    /// Description of a view connection.
    /// </summary>
    public class ConnectionDefinition : IViewProperty
    {
        /// <summary>
        /// Description of the property.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Human readable property name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Reference to the direct relation.
        /// </summary>
        public DirectRelationIdentifier Type { get; set; }
        /// <summary>
        /// Reference to a view.
        /// </summary>
        public ViewIdentifier Source { get; set; }
        /// <summary>
        /// Direction of the connection.
        /// </summary>
        public ConnectionDirection Direction { get; set; }
    }

    /// <summary>
    /// Json converter for view property types.
    /// </summary>
    public class ViewPropertyConverter : UntaggedUnionConverter<IViewProperty>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ViewPropertyConverter() : base(new[]
        {
            typeof(ViewPropertyDefinition), typeof(ConnectionDefinition)
        })
        {
        }
    }
}
