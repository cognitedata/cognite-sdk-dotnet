// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Diagnostics.CodeAnalysis;

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// Class to write a row to a table in Raw.
    /// </summary>
    public class RawRowCreate<T>
    {
        /// <summary>
        /// Row key. Unique in table.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Row data stored as type <typeparamref name="T"/>. This gets converted to JSON.
        /// </summary>
        public T Columns { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
