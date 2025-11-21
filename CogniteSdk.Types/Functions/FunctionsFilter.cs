using System.Collections.Generic;

namespace CogniteSdk
{
    /// <summary>
    /// Filter for functions.
    /// </summary>
    public class FunctionsFilter
    {
        /// <summary>
        /// Filter by function name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Filter by function owner.
        /// </summary>
        public string Owner { get; set; }
        /// <summary>
        /// Filter by ID of the file containing the function code.
        /// </summary>
        public int FileId { get; set; }
        /// <summary>
        /// Filter by the status of the function.
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// Filter by a prefix on function external ID.
        /// </summary>
        public string ExternalIdPrefix { get; set; }
        /// <summary>
        /// Filter by a range of timestamps for the
        /// creation time of the function.
        /// </summary>
        public TimeRange CreatedTime { get; set; }
        /// <summary>
        /// Filter by metadata fields.
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; }
    }

    /// <summary>
    /// Request for retrieving functions with optional filter.
    /// </summary>
    public class FunctionsFilterRequest
    {
        /// <summary>
        /// Optional filter on functions to retrieve.
        /// </summary>
        public FunctionsFilter Filter { get; set; }
        /// <summary>
        /// /// Maximum number of functions to return. Defaults to 100.
        /// </summary>
        /// <value></value>
        public int? Limit { get; set; }
    }
}