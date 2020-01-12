// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Types.Common
{
    public abstract class UpdateItem<TUpdate> {}
    public class UpdateById<TUpdate> : UpdateItem<TUpdate> {
        public TUpdate Update { get; set; }

        /// <summary>
        /// A server-generated ID for the object.
        /// </summary>
        public long Id { get; set; }
    }

    public class UpdateByExternalId<TUpdate> : UpdateItem<TUpdate> {
        public TUpdate Update { get; set; }

        /// <summary>
        /// The external ID provided by the client. Must be unique for the resource type.
        /// </summary>
        public string ExternalId { get; set; }
    }
}