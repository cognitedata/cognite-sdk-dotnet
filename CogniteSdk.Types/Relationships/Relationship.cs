// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// Dto for reading relationship.
    /// </summary>
    public class Relationship
    {
        /// <summary>
        /// The source of the relationship.
        /// </summary>
        public RelationshipResource Source { get; set; }

        /// <summary>
        /// The target of the relationship.
        /// </summary>
        public RelationshipResource Target { get; set; }

        /// <summary>
        /// Time when the relationship started.
        /// </summary>
        public long StartTime { get; set; }

        /// <summary>
        /// Time when the relationship ended.
        /// </summary>
        public long EndTime { get; set; }

        /// <summary>
        /// Confidence value of the existence of this relationship.
        /// Humans should enter 1.0 usually, generated relationships
        /// should provide a realistic score on the likelihood of the
        /// existence of the relationship. Generated relationships
        /// should never have the a confidence score of 1.0.
        /// </summary>
        public float Confidence { get; set; }

        /// <summary>
        /// String describing the source system storing or generating the relationship.
        /// </summary>
        public string DataSet { get; set; }

        /// <summary>
        /// External id of the relationship.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Enum: "flowsTo" "belongsTo" "isParentOf" "implements"
        /// </summary>
        public string RelationshipType { get; set; }

        /// <summary>
        /// Time when this relationship was created.
        /// </summary>
        public long CreatedTime { get; set; }

        /// <summary>
        /// Time when this relationship was last updated.
        /// </summary>
        public long LastUpdatedTime { get; set; }
    }
}