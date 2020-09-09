// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The CogniteExternalId read class.
    /// </summary>
    public class CogniteExternalId
    {
        /// <summary>
        /// Placeholder ExternalId Class.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Empty constructor method for ExternalId type.
        /// </summary>
        public CogniteExternalId()
        {
        }

        /// <summary>
        /// Constructor method for ExternalId type.
        /// </summary>
        public CogniteExternalId(string externalID)
        {
            ExternalId = externalID;
        }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}

