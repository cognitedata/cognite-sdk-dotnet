// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.DataModels
{
    /// <summary>
    /// Class for retrieving a list of instances.
    /// </summary>
    public class InstancesRetrieve : ItemsWithoutCursor<InstanceIdentifierWithType>
    {
        /// <summary>
        /// Sources to retrieve from.
        /// </summary>
        public IEnumerable<InstanceSource> Sources { get; set; }
        /// <summary>
        /// True to retrieve additional type information.
        /// </summary>
        public bool IncludeTyping { get; set; }
    }

    /// <summary>
    /// Response when retrieving instances.
    /// </summary>
    /// <typeparam name="T">Type of internal properties in retrieved instances.</typeparam>
    public class InstancesRetrieveResponse<T> : ItemsWithoutCursor<BaseInstance<T>>
    {
        /// <summary>
        /// Optional type information
        /// </summary>
        public TypeInformation Typing { get; set; }
    }
}
