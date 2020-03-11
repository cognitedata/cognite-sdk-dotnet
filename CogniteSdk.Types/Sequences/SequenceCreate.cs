// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// Sequence class for writing.
    /// </summary>
    public class SequenceCreate
    {
        /// <summary>
        /// The external ID provided by the client. Must be unique for the resource type.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Javascript friendly internal ID given to the object.
        /// </summary>
        public long? DataSetId { get; set; }

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
        [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "System.Text.Json ignores properties that don't have setters")]
        public Dictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// List of column definitions.
        /// </summary>
        public IEnumerable<SequenceColumnWrite> Columns { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
