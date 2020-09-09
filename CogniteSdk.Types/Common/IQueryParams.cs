// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk
{
    /// <summary>
    /// Interface for classes that can be converted to query parameters.
    /// </summary>
    public interface IQueryParams
    {
        /// <summary>
        /// Convert the query class object to a list of query parameters.
        /// </summary>
        /// <returns>Key/value tuple sequence of all properties set in the query object.</returns>
        List<(string, string)> ToQueryParams();
    }
}
