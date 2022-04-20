using System.Collections.Generic;

namespace CogniteSdk
{
    /// <summary>
    /// Query for fetching a specific config revision.
    /// </summary>
    public class GetConfigQuery : IQueryParams
    {
        /// <summary>
        /// Config revision to fetch.
        /// </summary>
        public int Revision { get; set; }

        /// <inheritdoc />
        public List<(string, string)> ToQueryParams()
        {
            var prs = new List<(string, string)>();
            if (Revision > 0) prs.Add(("revision", Revision.ToString()));
            return prs;
        }
    }
}
