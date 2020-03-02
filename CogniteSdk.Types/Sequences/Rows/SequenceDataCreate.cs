// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The Sequence Data create class
    /// </summary>
    public class SequenceDataCreate
    {
        /// <summary>
        /// Column external ids in the same order as the values for each row.
        /// </summary>
        public IEnumerable<string> Columns { get; set; }

        /// <summary>
        /// List of row information.
        /// </summary>
        public IEnumerable<SequenceRow> Rows { get; set; }

        /// <summary>
        /// A server-generated ID for the object.
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// The external ID provided by the client. Must be unique for the resource type.
        /// </summary>
        public string ExternalId { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
