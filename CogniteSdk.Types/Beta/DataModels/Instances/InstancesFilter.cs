// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CogniteSdk.Beta.DataModels
{
    /// <summary>
    /// Filter request for instances.
    /// </summary>
    public class InstancesFilter : CursorQueryBase
    {
        /// <summary>
        /// True to include property type information in the response.
        /// </summary>
        public bool IncludeTyping { get; set; }
        /// <summary>
        /// Retrieve properties from the listed - by reference - views.
        /// </summary>
        public IEnumerable<InstanceSource> Sources { get; set; }
        /// <summary>
        /// Type of instance to query for. If not specified, defaults to node.
        /// </summary>
        public InstanceType? InstanceType { get; set; }
        /// <summary>
        /// Sort the results
        /// </summary>
        public IEnumerable<InstanceSort> Sort { get; set; }
        /// <summary>
        /// Filter the results.
        /// </summary>
        public IDMSFilter Filter { get; set; }
    }

    /// <summary>
    /// Wrapper for a view identifier.
    /// </summary>
    public class InstanceSource
    {
        /// <summary>
        /// Source of instance
        /// </summary>
        public ViewIdentifier Source { get; set; }
    }

    /// <summary>
    /// Direction to sort
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SortDirection
    {
        /// <summary>
        /// Sort ascending
        /// </summary>
        ascending,
        /// <summary>
        /// Sort descending
        /// </summary>
        descending
    }

    /// <summary>
    /// Sort retrieved instances.
    /// </summary>
    public class InstanceSort
    {
        /// <summary>
        /// Property to sort.
        /// </summary>
        public IEnumerable<string> Property { get; set; }
        /// <summary>
        /// Direction to sort
        /// </summary>
        public SortDirection Direction { get; set; }
        /// <summary>
        /// Whether to put nulls first or last.
        /// </summary>
        public bool NullsFirst { get; set; }
    }

    /// <summary>
    /// Type information
    /// </summary>
    public class TypeInformation : Dictionary<string, Dictionary<string, Dictionary<string, ContainerPropertyDefinition>>>
    {
        // We could maybe implement IDictionary<(string, string, string), ContainerPropertyDefinition>, or something here?
        // This type is a bit clunky to work with.
    }

    /// <summary>
    /// Response when filtering instances.
    /// </summary>
    /// <typeparam name="T">Type of internal properties in retrieved instances</typeparam>
    public class InstancesFilterResponse<T> : ItemsWithCursor<BaseInstance<T>>
    {
        /// <summary>
        /// Optional type information.
        /// </summary>
        public TypeInformation Typing { get; set; }
    }
}
