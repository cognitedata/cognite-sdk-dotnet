// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// Items with a next cursor.
    /// </summary>
    /// <typeparam name="T">Resource type.</typeparam>
    public interface IItemsWithCursor<out T>
    {
        /// <summary>
        /// Resource items of type T.
        /// </summary>
        IEnumerable<T> Items { get; }

        /// <summary>
        /// Cursor to next page of data items.
        /// </summary>
        public string NextCursor { get; }

        /// <inheritdoc />
        string ToString();
    }

    /// <summary>
    /// Holds several items. But don't support paging, i.e no cursor.
    /// </summary>
    /// <typeparam name="T">A resource type.</typeparam>
    public interface IItemsWithoutCursor<out T>
    {
        /// <summary>
        /// Resource items of type T.
        /// </summary>
        public IEnumerable<T> Items { get;  }

        /// <inheritdoc />
        public string ToString();
    }

    /// <summary>
    /// Holds several items. But don't support paging, i.e no cursor.
    /// </summary>
    /// <typeparam name="T">A resource type that is serializable.</typeparam>
    public class ItemsWithoutCursor<T> : IItemsWithoutCursor<T>
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
    public class ItemsWithCursor<T> : IItemsWithCursor<T>
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
        public override string ToString() => Stringable.ToString(this);
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
