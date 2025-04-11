// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The Sequence row query class.
    /// </summary>
    public class SequenceRowQuery : CursorQueryBase
    {
        /// <summary>
        /// Lowest row number included.
        /// </summary>
        public long? Start { get; set; }

        /// <summary>
        /// Get rows up to, but excluding, this row number. Default - No limit.
        /// </summary>
        public long? End { get; set; }

        /// <summary>
        /// Columns to be included. Specified as list of column externalIds. In case this filter is not set, all
        /// available columns will be returned.
        /// </summary>
        public IEnumerable<string> Columns { get; set; }

        /// <summary>
        /// A server-generated ID for the object.
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// The external ID provided by the client. Must be unique for the resource type.
        /// </summary>
        public string ExternalId { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<SequenceRowQuery>(this);
    }
}
