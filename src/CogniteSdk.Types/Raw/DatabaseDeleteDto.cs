// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Raw
{
    /// <summary>
    /// Databases delete DTO.
    /// </summary>
    public class DatabaseDeleteDto : ItemsWithoutCursor<DatabaseDto>
    {
        /// <summary>
        /// When true, tables of this database are deleted with the database.
        /// </summary>
        public bool? Recursive { get; set; }
    }
}
