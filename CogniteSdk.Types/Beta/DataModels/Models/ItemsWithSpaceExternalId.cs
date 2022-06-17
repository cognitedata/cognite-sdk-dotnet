// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Beta
{
    /// <summary>
    /// List of items with externalId of a data model space.
    /// </summary>
    /// <typeparam name="T">Type of item</typeparam>
    public class ItemsWithSpaceExternalId<T> : ItemsWithoutCursor<T>
    {
        /// <summary>
        /// Space external id.
        /// </summary>
        public string SpaceExternalId { get; set; }
    }
}
