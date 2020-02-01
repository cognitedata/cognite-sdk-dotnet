// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace CogniteSdk.Sequences
{
    /// <summary>
    /// Sequence DTO for reading sequences.
    /// </summary>
    public class SequenceReadDto
    {
        /// <summary>
        /// The Id of the sequence
        /// </summary>
        public long Id { get; set; }

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
        /// The externalId of the sequence. Must be unique within the project.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Custom, application specific metadata. String key -> String value
        /// </summary>
        [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "System.Text.Json ignores properties that don't have setters")]
        public Dictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// List of column definitions.
        /// </summary>
        public IEnumerable<SequenceColumnDto> Columns { get; set; }

        /// <summary>
        /// Time when this sequence was created in CDF in milliseconds since Jan 1, 1970.
        /// </summary>
        public long CreatedTime { get; set; }

        /// <summary>
        /// The last time this sequence was updated in CDF, in milliseconds since Jan 1, 1970.
        /// </summary>
        public long LastUpdatedTime { get; set; }
    }
}
