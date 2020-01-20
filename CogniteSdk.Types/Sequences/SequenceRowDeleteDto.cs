// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Sequences
{
    /// <summary>
    /// The Sequence row delete DTO.
    /// </summary>
    public class SequenceRowDeleteDto
    {
        /// <summary>
        /// Rows to delete.
        /// </summary>
        /// <value></value>
        public IEnumerable<long> Rows { get; set; }

        /// <summary>
        /// A server-generated ID for the object.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The external ID provided by the client. Must be unique for the resource type.
        /// </summary>
        public string ExternalId { get; set; }
    }
}
