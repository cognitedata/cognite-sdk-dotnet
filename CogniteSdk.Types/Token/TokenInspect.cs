// Copyright 2021 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Text.Json.Serialization;

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
        public string Subject { get; set;}

        /// <summary>
        /// List of projects associated with the token.
        /// </summary>
        public IEnumerable<TokenProject> Projects { get; set; }

        // TODO: This can be ignored for now. If the Groups API is implemented,
        // then the same types can be reused here.
        /// <summary>
        /// List of capabilities associated with the token. 
        /// </summary>
        [JsonIgnore]
        public IEnumerable<object> Capabilities { get; set; }
    }

}
