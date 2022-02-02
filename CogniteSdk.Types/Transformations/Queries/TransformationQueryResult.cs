// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk
{
    /// <summary>
    /// Result of a transformations preview.
    /// </summary>
    public class TransformationQueryResult
    {
        /// <summary>
        /// Schema of results.
        /// </summary>
        public ItemsWithoutCursor<TransformationColumnType> Schema { get; set; }

        /// <summary>
        /// List of results.
        /// </summary>
        public ItemsWithoutCursor<Dictionary<string, string>> Results { get; set; }
    }
}
