// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CogniteSdk.DataModels
{
    /// <summary>
    /// Query data models.
    /// </summary>
    public class DataModelQuery : CursorQueryBase
    {
        /// <summary>
        /// True to expand the referenced views inline in the returned result.
        /// </summary>
        public bool InlineViews { get; set; }
        /// <summary>
        /// The space to query.
        /// </summary>
        public string Space { get; set; }
        /// <summary>
        /// True if all versions of the entity should be returned.
        /// False returns the latest version, with the newest 'createdTime' field.
        /// </summary>
        public bool AllVersions { get; set; }

        /// <inheritdoc />
        public override List<(string, string)> ToQueryParams()
        {
            var q = base.ToQueryParams();
            if (InlineViews)
            {
                q.Add(("inlineViews", "true"));
            }
            if (!string.IsNullOrEmpty(Space))
            {
                q.Add(("space", Space));
            }
            if (AllVersions)
            {
                q.Add(("allVersions", "true"));
            }
            return q;
        }
    }

    /// <summary>
    /// Which data model property to sort on.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DataModelsSortProperty
    {
        /// <summary>
        /// Sort by space id
        /// </summary>
        space,
        /// <summary>
        /// Sort by externalId
        /// </summary>
        externalId,
        /// <summary>
        /// Sort by name
        /// </summary>
        name,
        /// <summary>
        /// Sort by description
        /// </summary>
        description,
        /// <summary>
        /// Sort by version.
        /// </summary>
        version,
        /// <summary>
        /// Sort by createdTime.
        /// </summary>
        createdTime,
        /// <summary>
        /// Sort by lastUpdatedTime.
        /// </summary>
        lastUpdatedTime
    }

    /// <summary>
    /// Which field to sort data models by.
    /// </summary>
    public class DataModelsSort
    {
        /// <summary>
        /// Property to base sorting on
        /// </summary>
        public DataModelsSortProperty Property { get; set; }
        /// <summary>
        /// Direction to sort.
        /// </summary>
        public SortDirection Direction { get; set; }
        /// <summary>
        /// True to list nulls first.
        /// </summary>
        public bool NullsFirst { get; set; }
    }

    /// <summary>
    /// Filter for data models.
    /// </summary>
    public class DataModelFilter : CursorQueryBase
    {
        /// <summary>
        /// List of spaces to limit the returned data models by.
        /// </summary>
        public IEnumerable<string> Spaces { get; set; }
        /// <summary>
        /// Optional complex filter.
        /// </summary>
        public IDMSFilter Filter { get; set; }
        /// <summary>
        /// True if all versions of the entity should be returned.
        /// False returns the latest version, with the newest 'createdTime' field.
        /// </summary>
        public bool AllVersions { get; set; }
        /// <summary>
        /// Specifies how to sort the retrieved data models.
        /// </summary>
        public IEnumerable<DataModelsSort> Sort { get; set; }
    }

    /// <summary>
    /// Query for operations passing inlineViews.
    /// </summary>
    public class DataModelInlineViewsQuery : IQueryParams
    {
        /// <summary>
        /// True to expand the referenced views inline in the returned result.
        /// </summary>
        public bool InlineViews { get; set; }

        /// <inheritdoc />
        public List<(string, string)> ToQueryParams()
        {
            var q = new List<(string, string)>();
            if (InlineViews)
            {
                q.Add(("inlineViews", "true"));
            }
            return q;
        }
    }
}
