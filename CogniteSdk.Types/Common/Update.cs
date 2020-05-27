// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;
using System.Collections.Generic;

namespace CogniteSdk
{
    /// <summary>
    /// Used for setting a new value for the update property. Or for removing the property.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the property being updated. Must be a "nullable" type i.e reference or Nullable.
    /// </typeparam>
    public class Update<T>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public Update() { }

        /// <summary>
        /// Set a new value for the property.
        /// </summary>
        /// <param name="value">Value to set.</param>
        /// <returns>A new instance of the <see cref="Update{T}">SetProperty{T}</see> class.</returns>
        public Update(T value)
        {
            Set = value;
        }

        /// <summary>
        /// Contains the value to set.
        /// </summary>
        public T Set { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return Stringable.ToString(this);
        }
    }

    /// <summary>
    /// Used for setting a new value for the update property. Or for removing the property.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the property being updated. Must be a "nullable" type i.e reference or Nullable.
    /// </typeparam>
    public class UpdateNullable<T> : Update<T>
    {
        /// <summary>
        /// True if the value should be cleared.
        /// </summary>
        public bool? SetNull { get; set; }

        /// <summary>
        /// Set a new value for the property. A value of null will clear the value.
        /// </summary>
        /// <param name="value">Value to set.</param>
        /// <returns>A new instance of the <see cref="Update{T}">Property{T}</see> class.</returns>
        public UpdateNullable(T value) : base(value)
        {
            if (value is null)
            {
                SetNull = true;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Stringable.ToString(this);
        }
    }

    /// <summary>
    /// Used for setting, updating and removing Metadata entries.
    /// </summary>
    public abstract class UpdateCollection<TCollection, TRemove> : Update<TCollection>
    {
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
        /// <param name="value">Values to set (overwrite).</param>
        /// <returns>
        /// A new instance of the <see cref="UpdateCollection{TCollection, TRemove}">CollectionProperty{TCollection, TRemove}</see>
        /// class.
        /// </returns>
        public UpdateCollection(TCollection value)
        {
            Set = value;
        }

        /// <summary>
        /// Add the key-value pairs. Values for existing keys will be overwritten. Remove the key-value pairs with the
        /// specified keys.
        /// </summary>
        /// <param name="addKeyValues">Values to update.</param>
        /// <param name="removeKeys">Keys to remove.</param>
        /// <returns></returns>
        public UpdateCollection(TCollection addKeyValues, IEnumerable<TRemove> removeKeys)
        {
            Add = addKeyValues;
            Remove = removeKeys;
        }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }

    /// <summary>
    /// The dictionary update used i.e for Metadata properties. Inner type is Dictionary{string, T}.
    /// </summary>
    /// <typeparam name="T">The type of the dictionary value.</typeparam>
    public class UpdateDictionary<T> : UpdateCollection<Dictionary<string, T>, T>
    {
        /// <summary>
        /// Initialize the object property and set a new value.
        /// </summary>
        /// <param name="set">Set the new value.</param>
        public UpdateDictionary(Dictionary<string, T> set) : base(set) { }

        /// <summary>
        /// Initialize the object property and remove values.
        /// </summary>
        /// <param name="remove">Remove the key-value pairs with the specified keys.</param>
        public UpdateDictionary(IEnumerable<T> remove) : base(null, remove) { }

        /// <summary>
        /// Initialize the object property and add and remove values.
        /// </summary>
        /// <param name="add">Add the key-value pairs. Values for existing keys will be overwritten.</param>
        /// <param name="remove">Remove the key-value pairs with the specified keys.</param>
        public UpdateDictionary(Dictionary<string, T> add, IEnumerable<T> remove) : base(add, remove) { }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }

    /// <summary>
    /// A sequence property to use for array properties.
    /// </summary>
    /// <typeparam name="T">The type of the sequence items.</typeparam>
    public class UpdateEnumerable<T> : UpdateCollection<IEnumerable<T>, T>
    {
        /// <summary>
        /// Replace sequence with given sequence.
        /// </summary>
        /// <param name="set">Values to set.</param>
        public UpdateEnumerable(IEnumerable<T> set) : base(set) { }

        /// <summary>
        /// Initialize sequence property.
        /// </summary>
        /// <param name="add">Values to add to the sequence.</param>
        /// <param name="remove">Values to remove from the sequence.</param>
        public UpdateEnumerable(IEnumerable<T> add, IEnumerable<T> remove) : base(add, remove) { }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }

    /// <summary>
    /// Used for setting, updating and removing Labels, primarily for Assets.
    /// </summary>
    [System.Obsolete("The UpdateLabels class is in development, and currently only available for use in playground")]
    public class UpdateLabels<TCollection>
    {
        /// <summary>
        /// User to add new labels labels
        /// </summary>
        public TCollection Put { get; set; }

        /// <summary>
        /// Used to remove labels
        /// </summary>
        public TCollection Remove { get; set; }

        public UpdateLabels(TCollection putLabels)
        {
            Put = putLabels;
        }

        /// <summary>
        /// Add and remove labels
        /// specified keys.
        /// </summary>
        /// <param name="putLabels">Labels to put</param>
        /// <param name="removeLabels">Labels to remove.</param>
        /// <returns></returns>
        public UpdateLabels(TCollection putLabels, TCollection removeLabels)
        {
            Put = putLabels;
            Remove = removeLabels;
        }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}