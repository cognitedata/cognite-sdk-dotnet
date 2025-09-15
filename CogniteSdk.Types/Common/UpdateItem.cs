// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// Update class for a given property.
    /// </summary>
    /// <typeparam name="TUpdate">Type of object to update.</typeparam>
    public class UpdateItem<TUpdate> : Identity
    {
        /// <summary>
        /// Initialize the update item with an external Id.
        /// </summary>
        /// <param name="externalId">External Id to set.</param>
        public UpdateItem(string externalId) : base(externalId)
        {
        }

        /// <summary>
        /// Initialize the update item with an internal Id.
        /// </summary>
        /// <param name="id">Internal Id to set.</param>
        public UpdateItem(long id) : base(id)
        {
        }

        /// <summary>
        /// The update object.
        /// </summary>
        public TUpdate Update { get; set; }


        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
