// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.ComponentModel;

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

        /// <summary>
        /// Returns a user friendly string for printing.
        /// </summary>
        /// <returns>String representation of the items.</returns>
        public override string ToString()
        {
            var props = new List<string>();
            foreach (var item in Items)
            {
                props.Add(item.ToString());
            }

            return String.Join("/n", props);
        }
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

        /// <summary>
        /// Returns a user friendly string for printing.
        /// </summary>
        /// <returns>String representation of the items.</returns>
        public override string ToString()
        {
            var props = new List<string>();
            foreach (var item in Items)
            {
                props.Add(item.ToString());
            }

            return String.Join("/n", props);
        }
    }
}
