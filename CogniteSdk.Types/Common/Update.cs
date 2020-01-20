// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;

namespace CogniteSdk
{
    /// <summary>
    /// A generic base class for API updates.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Update<T> {
        private T _set;
        private T _setNull;

        /// <summary>
        /// The property set in the update object.
        /// </summary>
        public T Set
        {
            get => _set;
            set
            {
                if (_setNull == null)
                {
                    _set = value;
                }
                else
                {
                    throw new ArgumentException("Cannot both set and remove in the same update");
                }
            }
        }

        /// <summary>
        /// The property set to null in the update object.
        /// </summary>
        public T SetNull
        {
            get => _setNull;
            set
            {
                if (_set == null)
                {
                    _setNull = value;
                }
                else
                {
                    throw new ArgumentException("Cannot both set and remove in the same update");
                }
            }
        }

        public T Add { get; set; }
        public T Remove { get; set; }
    }
}