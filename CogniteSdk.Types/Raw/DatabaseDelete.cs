// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// Databases delete DTO.
    /// </summary>
    public class DatabaseDelete : ItemsWithoutCursor<RawDatabase>
    {
        /// <summary>
        /// When true, tables of this database are deleted with the database.
        /// </summary>
        public bool? Recursive { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
