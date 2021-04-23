// Copyright 2021 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Token
{
    /// <summary>
    /// Token project read class.
    /// </summary>
    public class TokenProject
    {
        /// <summary>
        /// Project url name.
        /// </summary>
        public string ProjectUrlName { get; set; }
        
        /// <summary>
        /// Group IDs.
        /// </summary>
        public IEnumerable<long> Groups { get; set; }
    }
}
