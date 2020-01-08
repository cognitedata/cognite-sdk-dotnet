// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Types.Common
{
    /// <summary>
    /// Holds several resource items.
    /// </summary>
    /// <typeparam name="T">A resource type that is serializable.</typeparam>
    public class ResourceItems<T>
    {
        /// <summary>
        /// Resource items of type T.
        /// </summary>
        public IEnumerable<T> Items { get; set; }
    }
}
