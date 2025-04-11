// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// Range between two timestamps.
    /// </summary>
    public class TimeRange
    {
        /// <summary>
        /// The number of milliseconds since 00:00:00 Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus
        /// leap seconds.
        /// </summary>

        public long? Max { get; set; }
        /// <summary>
        /// The number of milliseconds since 00:00:00 Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus
        /// leap seconds.
        /// </summary>
        public long? Min { get; set; }

        /// <summary>
        /// Set to true if you want to search for data with field value not set,
        /// false to search for cases where some value is present.
        /// </summary>
        public bool? IsNull { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
