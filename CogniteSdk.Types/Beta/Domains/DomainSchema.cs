// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0
using CogniteSdk.Types.Common;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Class for domain schema.
    /// </summary>
    public class DomainSchema
    {
        /// <summary>
        /// Complete schema for a domain.
        /// </summary>
        public string Schema { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
