// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// Query containing filter and cursor for data sets.
    /// </summary>
    public class DataSetQuery : CursorQueryBase
    {
        /// <summary>
        /// Filter on data sets with strict matching.
        /// </summary>
        public DataSetFilter Filter { get; set; }
        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
