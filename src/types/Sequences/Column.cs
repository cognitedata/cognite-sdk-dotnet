// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Types.Sequences
{
    public class Column
    {
        /// The name of the column.
        public string Name { get; set; }
        /// The externalId of the column. Must be unique within the project.
        public string ExternalId { get; set; }
        /// The description of the column.
        public string Description { get; set; }
        /// The valueType of the column. Enum STRING, DOUBLE, LONG
        //public ValueType : ValueType = valueType with get, set
        /// Custom, application specific metadata. String key -> String value
        public IDictionary<string, string> MetaData { get; set; }
        /// Time when this column was created in CDF in milliseconds since Jan 1, 1970.
        public long CreatedTime { get; set; }
        /// The last time this column was updated in CDF, in milliseconds since Jan 1, 1970.
        public long LastUpdatedTime { get; set; }
    }
}
