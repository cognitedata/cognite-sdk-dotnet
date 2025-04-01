// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// Raw database class.
    /// </summary>
    public class RawDatabase
    {
        /// <summary>
        /// Unique name of a database.
        /// </summary>
        public string Name { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
