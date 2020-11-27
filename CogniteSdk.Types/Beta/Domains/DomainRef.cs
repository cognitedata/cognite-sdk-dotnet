// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Class for referencing a domain uniquely.
    /// </summary>
    public class DomainRef
    {
        /// <summary>
        /// External id of the domain.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Version of the domain.
        /// </summary>
        public int Version { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
