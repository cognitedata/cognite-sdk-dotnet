using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Direction to sort by properties.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PropertySortDirection
    {
        /// <summary>
        /// Sort in ascending order.
        /// </summary>
        ascending = 0,
        /// <summary>
        /// Sort in descending order.
        /// </summary>
        descending = 1
    }

    /// <summary>
    /// Sort clause for a query on nodes.
    /// </summary>
    public class PropertySort
    {
        /// <summary>
        /// List of strings defining the property by spaceExternalId, modelExternalId and property name.
        /// </summary>
        public IEnumerable<string> Property { get; set; }
        /// <summary>
        /// Direction to sort.
        /// </summary>
        public PropertySortDirection Direction { get; set; } = PropertySortDirection.ascending;
        /// <summary>
        /// True to put nulls first.
        /// </summary>
        public bool NullsFirst { get; set; }
    }
}
