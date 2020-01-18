// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk
{
    /// <summary>
    /// Base class for updating by Id or External Id.
    /// </summary>
    /// <typeparam name="TUpdate">Type of object to update.</typeparam>
    public abstract class UpdateItemType<TUpdate> {}

    /// <summary>
    /// Update using external Id.
    /// </summary>
    /// <typeparam name="TUpdate">Type of object to update.</typeparam>
    public class UpdateById<TUpdate> : UpdateItemType<TUpdate>
    {
        public TUpdate Update { get; set; }

        /// <summary>
        /// A server-generated ID for the object.
        /// </summary>
        public long Id { get; set; }
    }

    /// <summary>
    /// Update using internal Id
    /// </summary>
    /// <typeparam name="TUpdate">Type of object to update.</typeparam>
    public class UpdateByExternalId<TUpdate> : UpdateItemType<TUpdate> {
        public TUpdate Update { get; set; }

        /// <summary>
        /// The external ID provided by the client. Must be unique for the resource type.
        /// </summary>
        public string ExternalId { get; set; }
    }
}