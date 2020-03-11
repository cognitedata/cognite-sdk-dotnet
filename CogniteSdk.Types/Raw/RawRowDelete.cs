// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// Class with keys of rows to delete in table.
    /// </summary>
    public class RawRowDelete
    {
        /// <summary>
        /// Row key. Unique in table.
        /// </summary>
        public string Key { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
