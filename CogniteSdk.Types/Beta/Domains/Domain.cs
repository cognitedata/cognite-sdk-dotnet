// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Class for reading domains.
    /// </summary>
    public class Domain : DomainRef
    {
        /// <summary>
        /// The schema of the domain.
        /// </summary>
        public string Schema { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
