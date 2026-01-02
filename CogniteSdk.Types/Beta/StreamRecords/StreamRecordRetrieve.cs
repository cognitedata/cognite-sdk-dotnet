// Copyright 2025 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Text.Json.Serialization;
using CogniteSdk.DataModels;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Sources to retrieve stream record data from.
    /// </summary>
    public class StreamRecordSource
    {
        /// <summary>
        /// Container reference.
        /// </summary>
        public ContainerIdentifier Source { get; set; }
        /// <summary>
        /// List of properties to retrieve.
        /// </summary>
        public IEnumerable<string> Properties { get; set; }
    }

    /// <summary>
    /// Optional attribute to extend the filter with full text search capabilities for
    /// a single query in the list of record properties with OR logic.
    /// </summary>
    public class StreamRecordSearch
    {
        /// <summary>
        /// Query string that will be parsed and used for search.
        /// </summary>
        public string Query { get; set; }
        /// <summary>
        /// Array of property identifiers to search through.
        /// </summary>
        public IEnumerable<IEnumerable<string>> Properties { get; set; }
    }

    /// <summary>
    /// Filter on records created within the provided range.
    /// </summary>
    public class LastUpdatedTimeFilter
    {
        /// <summary>
        /// Value must be greater than this
        /// </summary>
        [JsonPropertyName("gt")]
        public IDMSValue GreaterThan { get; set; }

        /// <summary>
        /// Value must be greater than or equal to this
        /// </summary>
        [JsonPropertyName("gte")]
        public IDMSValue GreaterThanEqual { get; set; }

        /// <summary>
        /// Value must be less than this
        /// </summary>
        [JsonPropertyName("lt")]
        public IDMSValue LessThan { get; set; }

        /// <summary>
        /// Value must be less than or equal to this
        /// </summary>
        [JsonPropertyName("lte")]
        public IDMSValue LessThanEqual { get; set; }
    }

    /// <summary>
    /// Specification for sorting retrieved records.
    /// </summary>
    public class StreamRecordsSort
    {
        /// <summary>
        /// Property you want to sort on.
        /// </summary>
        public IEnumerable<string> Property { get; set; }
        /// <summary>
        /// Sort direction.
        /// </summary>
        public SortDirection? Direction { get; set; }
    }

    /// <summary>
    /// Retrieve stream records.
    /// </summary>
    public class StreamRecordsRetrieve
    {
        /// <summary>
        /// Name of the stream where records are located, required.
        /// </summary>
        public string Stream { get; set; }
        /// <summary>
        /// List of containers and the properties that should be selected.
        /// 
        /// Optional, if this is left out all properties are returned.
        /// </summary>
        public IEnumerable<StreamRecordSource> Sources { get; set; }
        /// <summary>
        /// A filter Domain Specific Language (DSL) used to create advanced filter queries.
        /// 
        /// Note that some filter types are not supported, see API docs.
        /// </summary>
        public IDMSFilter Filter { get; set; }
        /// <summary>
        /// Matches records with last updated time time within the provided range.
        /// </summary>
        public LastUpdatedTimeFilter LastUpdatedTime { get; set; }
        /// <summary>
        /// Maximum number of results to return. Default 10, max 1000.
        /// </summary>
        public int? Limit { get; set; }
        /// <summary>
        /// Ordered list of sorting specifications.
        /// </summary>
        public IEnumerable<StreamRecordsSort> Sort { get; set; }
    }


    /// <summary>
    /// Request for syncing records.
    /// </summary>
    public class StreamRecordsSync
    {
        /// <summary>
        /// List of containers and the properties that should be selected.
        ///
        /// Optional, if this is left out all properties are returned.
        /// </summary>
        public IEnumerable<StreamRecordSource> Sources { get; set; }
        /// <summary>
        /// A filter Domain Specific Language (DSL) used to create advanced filter queries.
        ///
        /// Note that some filter types are not supported, see API docs.
        /// </summary>
        public IDMSFilter Filter { get; set; }
        /// <summary>
        /// A cursor returned from the previous sync request.
        /// </summary>
        public string Cursor { get; set; }
        /// <summary>
        /// Maximum number of results to return.
        /// </summary>
        public int? Limit { get; set; }
        /// <summary>
        /// Initialize cursor with a time offset. Required if `Cursor` is not set.
        /// </summary>
        public string InitializeCursor { get; set; }
    }

    /// <summary>
    /// Response from a sync request.
    /// </summary>
    /// <typeparam name="T">Type of properties in returned records.</typeparam>
    public class StreamRecordsSyncResponse<T> : ItemsWithCursor<StreamRecord<T>>
    {
        /// <summary>
        /// The attribute indiciates if there are more records to read in storage,
        /// or if the cursor points to the last item.
        /// </summary>
        public bool HasNext { get; set; }
    }
}
