// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk.Sequences
{
    /// <summary>
    /// The Sequence row DTO. Contains row number and values in the order defined by the columns.
    /// </summary>
    public class SequenceRow
    {
        /// <summary>
        /// The row number for this row.
        /// </summary>
        public long RowNumber { get; set; }

        /// <summary>
        /// List of values in order defined in the columns field (Number of items must match. Null is accepted for
        /// missing values. String values must be no longer than 256 characters).
        /// </summary>
        public IEnumerable<MultiValue> Values { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<SequenceRow>(this);
    }
}
