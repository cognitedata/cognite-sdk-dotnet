using System.Collections.Generic;
using System.Text.Json.Serialization;
using CogniteSdk.Beta.DataModels;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// Sources to retrieve log data from.
    /// </summary>
    public class LogSource
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
    /// a single query in the list of log properties with OR logic.
    /// </summary>
    public class LogSearch
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
    /// Filter on logs created within the provided range.
    /// </summary>
    public class CreatedTimeFilter
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
    /// Specification for sorting retrieved ILA logs.
    /// </summary>
    public class LogSort
    {
        /// <summary>
        /// Property you want to sort on.
        /// </summary>
        public IEnumerable<string> Property { get; set; }
        /// <summary>
        /// Sort direction.
        /// </summary>
        public SortDirection Direction { get; set; }
    }

    /// <summary>
    /// Retrieve logs from ILA.
    /// </summary>
    public class LogRetrieve
    {
        /// <summary>
        /// Name of the stream where logs are located, required.
        /// </summary>
        public string Stream { get; set; }
        /// <summary>
        /// List of containers and the properties that should be selected.
        /// 
        /// Optional, if this is left out all properties are returned.
        /// </summary>
        public IEnumerable<LogSource> Sources { get; set; }
        /// <summary>
        /// Optional attribute to extend the filter with full text search capabilities
        /// for a single field in the list of log properties with OR logic.
        /// </summary>
        public LogSearch Search { get; set; }
        /// <summary>
        /// A filter Domain Specific Language (DSL) used to create advanced filter queries.
        /// 
        /// Note that some filter types are not supported with ILA, see API docs.
        /// </summary>
        public IDMSFilter Filter { get; set; }
        /// <summary>
        /// Matches logs with created time within the provided range.
        /// </summary>
        public CreatedTimeFilter CreatedTime { get; set; }
        /// <summary>
        /// Maximum number of results to return. Default 10, max 10000.
        /// </summary>
        public int Limit { get; set; }
        /// <summary>
        /// Ordered list of sorting specifications.
        /// </summary>
        public IEnumerable<LogSort> Sort { get; set; }
    }


    /// <summary>
    /// Request for syncing logs.
    /// </summary>
    public class LogSync
    {
        /// <summary>
        /// Name of the stream where logs are located, required.
        /// </summary>
        public string Stream { get; set; }
        /// <summary>
        /// List of containers and the properties that should be selected.
        /// 
        /// Optional, if this is left out all properties are returned.
        /// </summary>
        public IEnumerable<LogSource> Sources { get; set; }
        /// <summary>
        /// A filter Domain Specific Language (DSL) used to create advanced filter queries.
        /// 
        /// Note that some filter types are not supported with ILA, see API docs.
        /// </summary>
        public IDMSFilter Filter { get; set; }
        /// <summary>
        /// A cursor returned from the previous sync request.
        /// </summary>
        public string Cursors { get; set; }
        /// <summary>
        /// Maximum number of results to return.
        /// </summary>
        public int Limit { get; set; }
    }

    /// <summary>
    /// Response from an ILA sync request.
    /// </summary>
    /// <typeparam name="T">Type of properties in returned logs.</typeparam>
    public class LogSyncResponse<T> : ItemsWithCursor<Log<T>>
    {
        /// <summary>
        /// The attribute indiciates if there are more logs to read in storage,
        /// or if the cursor points to the last item.
        /// </summary>
        public bool HasNext { get; set; }
    }
}