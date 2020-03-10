// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The Sequence column.
    /// </summary>
    public class SequenceColumn
    {
        /// <summary>
        /// The name of the column.
        /// </summary>
        public string Name { get; set; }

        /// The externalId of the column. Must be unique within the project.
        public string ExternalId { get; set; }

        /// <summary>
        /// The description of the column.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The valueType of the column. Enum STRING, DOUBLE, LONG
        /// </summary>
        public MultiValueType ValueType { get; set; }

        /// <summary>
        /// Custom, application specific metadata. String key -> String value
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "System.Text.Json ignores properties that don't have setters")]
        public Dictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// Time when this column was created in CDF in milliseconds since Jan 1, 1970.
        /// </summary>
        public long CreatedTime { get; set; }

        /// <summary>
        /// The last time this column was updated in CDF, in milliseconds since Jan 1, 1970.
        /// </summary>
        public long LastUpdatedTime { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
