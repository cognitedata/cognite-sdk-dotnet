// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk
{
    /// <summary>
    /// A generic base class for API updates.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Update<T> {}

    /// <summary>
    /// The update class for setting values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SetUpdate<T> : Update<T> {
        public T Set { get; set; }
    }

    /// <summary>
    /// The update class for clearing values.
    /// </summary>
    public class SetNullUpdate<T> : Update<T> {
        public T SetNull { get; set; }
    }
}