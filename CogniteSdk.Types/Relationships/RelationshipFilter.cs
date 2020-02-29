// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk
{
    /// <summary>
    /// Dto for filtering relationships.
    /// </summary>
    public class RelationshipFilter
    {
        /// <summary>
        /// Sources to include.
        /// </summary>
        public IEnumerable<RelationshipResource> Sources { get; set; }

        /// <summary>
        /// Targets to include.
        /// </summary>
        public IEnumerable<RelationshipResource> Targets { get; set; }

        /// <summary>
        /// RelationshipTypes to include.
        /// </summary>
        public IEnumerable<string> RelationshipTypes { get; set; }

        /// <summary>
        /// DataSets to include.
        /// </summary>
        public IEnumerable<string> DataSets { get; set; }

        /// <summary>
        /// External id must be unique within the project.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Range for startTime.
        /// </summary>
        public TimeRange StartTime { get; set; }

        /// <summary>
        /// Range for EndTime.
        /// </summary>
        public TimeRange EndTime { get; set; }

        /// <summary>
        /// Range for Confidence.
        /// </summary>
        public RangeObject Confidence { get; set; }

        /// <summary>
        /// Range for CreatedTime.
        /// </summary>
        public TimeRange CreatedTime { get; set; }

        /// <summary>
        /// Range for LastUpdatedTime.
        /// </summary>
        public TimeRange LastUpdatedTime { get; set; }
    }
}