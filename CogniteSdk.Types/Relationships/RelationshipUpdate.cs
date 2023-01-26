// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk
{
    /// <summary>
    /// Relationship patch
    /// </summary>
    public class RelationshipUpdate
    {
        /// <summary>
        /// The resource type of the source
        /// </summary>
        public Update<RelationshipVertexType> SourceType { get; set; }

        /// <summary>
        /// The source of the relationship.
        /// </summary>
        public Update<string> SourceExternalId { get; set; }

        /// <summary>
        /// The resource type of the target.
        /// </summary>
        public Update<RelationshipVertexType> TargetType { get; set; }

        /// <summary>
        /// The target of the relationship.
        /// </summary>
        public Update<string> TargetExternalId { get; set; }

        /// <summary>
        /// Time when the relationship started.
        /// </summary>
        public UpdateNullable<long> StartTime { get; set; }

        /// <summary>
        /// Time when the relationship ended.
        /// </summary>
        public UpdateNullable<long> EndTime { get; set; }

        /// <summary>
        /// The id of the dataset this relationship belongs to.
        /// </summary>
        public UpdateNullable<long> DataSetId { get; set; }

        /// <summary>
        /// Confidence value of the existence of this relationship.
        /// Humans should enter 1.0 usually, generated relationships
        /// should provide a realistic score on the likelihood of the
        /// existence of the relationship. Generated relationships
        /// should never have the a confidence score of 1.0.
        /// </summary>
        public UpdateNullable<float> Confidence { get; set; }

        /// <summary>
        /// List of labels to associate with the relationship.
        /// </summary>
        public UpdateLabels<IEnumerable<CogniteExternalId>> Labels { get; set; }
    }
}
