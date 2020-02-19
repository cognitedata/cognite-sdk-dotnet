// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

using CogniteSdk.Types.Common;

namespace CogniteSdk.Sequences
{
    /// <summary>
    /// The Sequence Data Read DTO.
    /// </summary>
    public class SequenceDataReadDto
    {
        /// <summary>
        /// A server-generated ID for the object.
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// The external ID provided by the client. Must be unique for the resource type.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Column information in order given by data.
        /// </summary>
        public IEnumerable<SequenceColumnInfoDto> Columns { get; set; }

        /// <summary>
        /// List of row information.
        /// </summary>
        public IEnumerable<SequenceRowDto> Rows { get; set; }

        /// <summary>
        /// Cursor to get the next page of results (if available).
        /// </summary>
        public string NextCursor { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<SequenceDataReadDto>(this);
    }
}
