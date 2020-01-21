// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk
{
    /// <summary>
    /// Update item.
    /// </summary>
    /// <typeparam name="TUpdate">Type of object to update.</typeparam>
    public class UpdateItem<TUpdate>
    {
        /// <summary>
        /// The update object.
        /// </summary>
        public TUpdate Update { get; set; }

        /// <summary>
        /// A server-generated ID for the object.
        /// </summary>
        public long? Id { get; set; }
    
        /// <summary>
        /// The external ID provided by the client. Must be unique for the resource type.
        /// </summary>
        public string ExternalId { get; set; }
    }
}