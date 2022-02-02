// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Types.Transformations
{
    /// <summary>
    /// Indicates why and when a transformation was blocked.
    /// </summary>
    public class TransformationBlockedInfo
    {
        /// <summary>
        /// Reason for why the transformation is blocked.
        /// </summary>
        public string Reason { get; set; }
        /// <summary>
        /// Indicates the time the transformation was blocked.
        /// The number of milliseconds since 00:00:00 Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus leap seconds.
        /// </summary>
        public long CreatedTime { get; set; }
    }
}
