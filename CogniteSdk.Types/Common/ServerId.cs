// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The CogniteServerId read class.
    /// </summary>
    public class CogniteServerId
    {
        /// <summary>
        /// A server-generated ID for the object.
        /// </summary>
        public long ServerId { get; set; }

        /// <summary>
        /// Empty constructor method for ServerId type
        /// </summary>
        public CogniteServerId()
        {
        }

        /// <summary>
        /// Constructor method for ServerId type
        /// </summary>
        public CogniteServerId(long serverId)
        {
            ServerId = serverId;
        }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}

