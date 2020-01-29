// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;
using System.Collections.Generic;

namespace CogniteSdk.Events
{
    /// <summary>
    /// The Event read DTO.
    /// </summary>
    public class EventReadDto : Stringable
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

        /// <summary>
        /// The Id of the asset.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Time when this asset was created in CDF in milliseconds since Jan 1, 1970.
        /// </summary>
        public long CreatedTime { get; set; }

        /// <summary>
        /// The last time this asset was updated in CDF, in milliseconds since Jan 1, 1970.
        /// </summary>
        /// <value></value>
        public long LastUpdatedTime { get; set; }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return obj is EventReadDto dto &&
                   Id == dto.Id;
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return 2108858624 + Id.GetHashCode();
        }
    }
}

