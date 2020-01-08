// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Types.Events
{
    public class EventReadDto
    {
        /// <summary>
        /// External Id provided by client. Must be unique within the project.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Start time of event in unix timestamp.
        /// </summary>
        public long? startTime { get; set; }

        /// <summary>
        /// End time of event in unix timestamp.
        /// </summary>
        public long? EndTime { get; set; }

        /// <summary>
        /// Type of the event, e.g 'failure'.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Subtype of the event, e.g 'electrical'.
        /// </summary>
        public string SubType { get; set; }

        /// <summary>
        /// The description of the asset.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Custom, application specific metadata. String key -> String value
        /// </summary>
        public IDictionary<string, string> MetaData { get; set; }

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
    }
}

