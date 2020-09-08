// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// Raw table class.
    /// </summary>
    public class RawTable
    {
        /// <summary>
        /// Name of the table. Unique in database.
        /// </summary>
        public string Name { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
