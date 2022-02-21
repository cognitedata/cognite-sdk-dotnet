// Copyright 2021 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Token
{
    /// <summary>
    /// Token inspect read class.
    /// </summary>
    public class TokenInspect
    {
        /// <summary>
        /// Subject (sub claim) of JWT.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// List of projects associated with the token.
        /// </summary>
        public IEnumerable<TokenProject> Projects { get; set; }

        /// <summary>
        /// List of capabilities associated with the token. 
        /// </summary>
        public IEnumerable<BaseAcl> Capabilities { get; set; }
    }

}
