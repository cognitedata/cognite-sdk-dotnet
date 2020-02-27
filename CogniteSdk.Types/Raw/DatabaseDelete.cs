// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk.Raw
{
    /// <summary>
    /// Databases delete DTO.
    /// </summary>
    public class DatabaseDelete : ItemsWithoutCursor<Database>
    {
        /// <summary>
        /// When true, tables of this database are deleted with the database.
        /// </summary>
        public bool? Recursive { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
