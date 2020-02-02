// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;
using System.Collections.Generic;

namespace CogniteSdk.Sequences
{
    /// <summary>
    /// Sequence DTO for writing.
    /// </summary>
    public class SequenceWriteDto
    {
        /// <summary>
        /// The external ID provided by the client. Must be unique for the resource type.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// The name of the sequence.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of the sequence.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The valueType of the sequence. Enum STRING, DOUBLE, LONG
        /// </summary>
        public long? AssetId { get; set; }

        /// <summary>
        /// Custom, application specific metadata. String key -> String value
        /// </summary>
        public IDictionary<string, string> MetaData { get; set; }

        /// <summary>
        /// List of column definitions.
        /// </summary>
        public IEnumerable<SequenceColumnWriteDto> Columns { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<SequenceWriteDto>(this);
    }
}
