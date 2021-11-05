// Copyright 2021 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The row read class.
    /// </summary>
    public class RawRowJson<T>
    {
        /// <summary>
        /// Row key. Unique in table.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Row data stored as a JSON object.
        /// </summary>
        public T Columns { get; set; }

        /// <summary>
        /// Unix timestamp in milliseconds of when the row was last updated.
        /// </summary>
        public long LastUpdatedTime { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
