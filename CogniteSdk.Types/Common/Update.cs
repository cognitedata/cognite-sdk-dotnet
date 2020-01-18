// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk
{
    /// <summary>
    /// A generic base class for API updates.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class UpdateType<T> {}

    /// <summary>
    /// The update class for setting values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SetUpdate<T> : UpdateType<T>
    {
        /// <summary>
        /// The property set in the update object.
        /// </summary>
        public T Set { get; set; }
    }

    /// <summary>
    /// The update class for clearing values.
    /// </summary>
    public class SetNullUpdate<T> : UpdateType<T>
    {
        /// <summary>
        /// The property set to null in the update object.
        /// </summary>
        public T SetNull { get; set; }
    }
}