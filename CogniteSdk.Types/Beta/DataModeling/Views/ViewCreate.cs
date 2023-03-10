// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Create or update a flexible data models view.
    /// </summary>
    public class ViewCreate : IViewCreateOrReference
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
        /// Included properties and expected edges, indexed by a unique space-local identifier.
        /// </summary>
        public Dictionary<string, ViewPropertyCreate> Properties { get; set; }
    }

    /// <summary>
    /// Interface for possible view property create types.
    /// </summary>
    public interface ICreateViewProperty { }

    /// <summary>
    /// Create a view property.
    /// </summary>
    public class ViewPropertyCreate : ICreateViewProperty
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
        /// Reference to an existing container.
        /// </summary>
        public ContainerIdentifier Container { get; set; }
        /// <summary>
        /// The unique identifier, in the referenced container, for the property to map.
        /// </summary>
        public string ContainerPropertyIdentifier { get; set; }
        /// <summary>
        /// Indicates what type a referenced direct relation is expected to be.
        /// </summary>
        public ViewIdentifier Source { get; set; }
    }

    /// <summary>
    /// Json converter for create view property types.
    /// </summary>
    public class ViewPropertyCreateConverter : UntaggedUnionConverter<ICreateViewProperty>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ViewPropertyCreateConverter() : base(new[]
        {
            typeof(ViewPropertyCreate), typeof(ConnectionDefinition)
        })
        {
        }
    }
}
