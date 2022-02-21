// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk
{
    /// <summary>
    /// Query for requesting the schema of a transformation destination
    /// </summary>
    public class TransformationSchemaQuery : IQueryParams
    {
        /// <summary>
        /// Behavior when the data already exists.
        /// upsert - Create or Update,
        /// abort - Create and fail when already exists,
        /// update - update and fail if it does not exist,
        /// delete - delete the matched rows.
        /// </summary>
        public TransformationConflictMode? ConflictMode { get; set; }

        /// <inheritdoc />
        public List<(string, string)> ToQueryParams()
        {
            var list = new List<(string, string)>();
            if (ConflictMode.HasValue)
                list.Add(("conflictMode", ConflictMode.Value.ToString()));

            return list;
        }
    }
}
