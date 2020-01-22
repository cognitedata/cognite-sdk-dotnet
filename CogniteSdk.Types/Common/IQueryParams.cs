using System.Collections.Generic;

namespace CogniteSdk
{
    /// <summary>
    /// Interface for classes that can be converted to query parameters.
    /// </summary>
    public interface IQueryParams
    {
        /// <summary>
        /// Convert the object to a list of query parameters.
        /// </summary>
        /// <returns></returns>
        List<(string, string)> ToQueryParams();
    }
}