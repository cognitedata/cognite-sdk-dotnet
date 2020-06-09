// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// Holds several items. But don't support paging, i.e no cursor.
    /// </summary>
    /// <typeparam name="T">A resource type that is serializable.</typeparam>
    public class ItemsWithoutCursor<T>
    {
        /// <summary>
        /// Resource items of type T.
        /// </summary>
        public IEnumerable<T> Items { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }

    /// <summary>
    /// Items with a next cursor.
    /// </summary>
    /// <typeparam name="T">Resource type that is serializable.</typeparam>
    public class ItemsWithCursor<T>
    {
        /// <summary>
        /// Resource items of type T.
        /// </summary>
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// Cursor to next page of data items.
        /// </summary>
        public string NextCursor { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<ItemsWithCursor<T>>(this);
    }

    /// <summary>
    /// Holds several items, and can ignore unknown ids. But don't support paging, i.e no cursor.
    /// </summary>
    /// <typeparam name="T">A resource type that is serializable.</typeparam>
    public class ItemsWithIgnoreUnknownIds<T>
    {
        /// <summary>
        /// Resource items of type T.
        /// </summary>
        public IEnumerable<T> Items { get; set; }
        /// <summary>
        /// Default: false
        /// Ignore IDs and external IDs that are not found
        /// </summary>
        public bool IgnoreUnknownIds { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
