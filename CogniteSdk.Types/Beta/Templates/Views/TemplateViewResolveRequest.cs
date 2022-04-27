// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Request to resolve a view.
    /// </summary>
    public class TemplateViewResolveRequest<T> : CursorQueryBase
    {
        /// <summary>
        /// View external id.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// View input.
        /// </summary>
        public Dictionary<string, T> Input { get; set; }
    }
}
