// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;

namespace CogniteSdk
{
    /// <summary>
    /// Used for setting a new value for the update property. Or for removing the property.
    /// </summary>
    /// <typeparam name="T">The type of the property being updated.</typeparam>
    public class SetProperty<T>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SetProperty() { }

        /// <summary>
        /// Set a new value for the property.
        /// </summary>
        /// <param name="set">Value to set.</param>
        /// <returns>A new instance of the <see cref="Property{T}">SetUpdate{T}</see> class.</returns>
        public SetProperty(T set)
        {
            Set = set;
        }

        /// <summary>
        /// Contains the value to set.
        /// </summary>
        public T Set { get; set; }
    }

    /// <summary>
    /// Used for setting a new value for the update property. Or for removing the property.
    /// </summary>
    /// <typeparam name="T">The type of the property being updated.</typeparam>
    public class Property<T> : SetProperty<T>
    {
        /// <summary>
        /// True if the value should be cleared.
        /// </summary>
        public bool? SetNull { get; set; }

        /// <summary>
        /// Set a new value for the property.
        /// </summary>
        /// <param name="set">Value to set.</param>
        /// <returns>A new instance of the <see cref="Property{T}">SetUpdate{T}</see> class.</returns>
        public Property(T set) : base(set)
        {
        }

        /// <summary>
        /// Clear the property.
        /// </summary>
        /// <param name="clear">Set to true to clear the property.</param>
        public Property(bool clear)
        {
            SetNull = clear;
        }
    }

    /// <summary>
    /// Used for setting, updating and removing Metadata entries.
    /// </summary>
    public class CollectionProperty<TCollection, TRemove>
    {
        /// <summary>
        /// Set the key-value pairs. All existing key-value pairs will be removed.
        /// </summary>
        public TCollection Set { get; set; }

        /// <summary>
        /// Add the key-value pairs. Values for existing keys will be overwritten.
        /// </summary>
        public TCollection Add { get; set; }

        /// <summary>
        /// Remove the key-value pairs with the specified keys.
        /// </summary>
        public IEnumerable<TRemove> Remove { get; set; }

        /// <summary>
        /// Set the key-value pairs. All existing key-value pairs will be removed.
        /// </summary>
        /// <param name="set">Values to set (overwrite).</param>
        /// <returns>A new instance of the <see cref="CollectionProperty{TCollection, TRemove}">AddRemoveUpdate</see> class.</returns>
        public CollectionProperty(TCollection set)
        {
            Set = set;
        }

        /// <summary>
        /// Add the key-value pairs. Values for existing keys will be overwritten. Remove the key-value pairs with the specified keys.
        /// </summary>
        /// <param name="add">Values to update.</param>
        /// <param name="remove">Keys to remove.</param>
        /// <returns></returns>
        public CollectionProperty(TCollection add, IEnumerable<TRemove> remove)
        {
            Add = add;
            Remove = remove;
        }
    }

    /// <summary>
    /// The object property used i.e for Metadata properties.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    public class ObjProperty<T> : CollectionProperty<IDictionary<string, T>, T>
    {
        /// <summary>
        /// Initialize the object property and set a new value.
        /// </summary>
        /// <param name="set">Set the new value.</param>
        public ObjProperty(Dictionary<string, T> set) : base(set)
        {
        }

        /// <summary>
        /// Initialize the object property and remove values.
        /// </summary>
        /// <param name="remove">Remove the key-value pairs with the specified keys.</param>
        public ObjProperty(IEnumerable<T> remove) : base(null, remove)
        {
        }

        /// <summary>
        /// Initialize the object property and add and remove values.
        /// </summary>
        /// <param name="add">Add the key-value pairs. Values for existing keys will be overwritten.</param>
        /// <param name="remove">Remove the key-value pairs with the specified keys.</param>
        public ObjProperty(Dictionary<string, T> add, IEnumerable<T> remove=null) : base(add, remove)
        {
        }
    }

    /// <summary>
    /// A sequence property to use for array properties.
    /// </summary>
    /// <typeparam name="T">The type of the sequence items.</typeparam>
    public class SeqProperty<T> : CollectionProperty<IEnumerable<T>, T>
    {
        /// <summary>
        /// Replace sequence with given sequence.
        /// </summary>
        /// <param name="set">Values to set.</param>
        public SeqProperty(IEnumerable<T> set) : base(set) { }

        /// <summary>
        /// Initialize sequence property.
        /// </summary>
        /// <param name="add">Values to add to the sequence.</param>
        /// <param name="remove">Values to remove from the sequence.</param>
        public SeqProperty(IEnumerable<T> add, IEnumerable<T> remove) : base(add, remove) { }
    }
}