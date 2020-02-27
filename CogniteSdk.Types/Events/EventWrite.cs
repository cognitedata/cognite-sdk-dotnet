// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

using CogniteSdk.Types.Common;

namespace CogniteSdk.Events
{
    /// <summary>
    /// The event write DTO.
    /// </summary>
    public class EventWrite
    {
        /// <summary>
        /// External Id provided by client. Must be unique within the project.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Javascript friendly internal ID given to the object.
        /// </summary>
        public long? DataSetId { get; set; }

        /// <summary>
        /// Start time of event in unix timestamp milliseconds.
        /// </summary>
        public long? StartTime { get; set; }

        /// <summary>
        /// End time of event in unix timestamp milliseconds.
        /// </summary>
        public long? EndTime { get; set; }

        /// <summary>
        /// Type of the event, e.g 'failure'.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Subtype of the event, e.g 'electrical'.
        /// </summary>
        public string Subtype { get; set; }

        /// <summary>
        /// The description of the asset.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Custom, application specific metadata. String key -> String value
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "System.Text.Json ignores properties that don't have setters")]
        public Dictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// Asset IDs of related equipment that this event relates to.
        /// </summary>
        public IEnumerable<long> AssetIds { get; set; }

        /// <summary>
        /// The source of this event.
        /// </summary>
        public string Source { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
