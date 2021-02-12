// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
namespace CogniteSdk.Beta
{
    /// <summary>
    /// Class for querying domains.
    /// </summary>
    public class DomainQuery : CursorQueryBase
    {
        /// <summary>
        /// Filter on owners
        ///
        public IEnumerable<string> Owners {get; set;}
    }
}
