// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Class for reading relationship.
    /// </summary>
    public class Relationship
    {
        /// <summary>
        /// External id of the relationship.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// The source of the relationship.
        /// </summary>
        public string SourceExternalId { get; set; }

        /// <summary>
        /// Resource type of the relationship source
        /// </summary>
        public RelationshipVertexType SourceType { get; set; }

        /// <summary>
        /// The target of the relationship.
        /// </summary>
        public string TargetExternalId { get; set; }

        /// <summary>
        /// Resource type of the relationship source
        /// </summary>
        public RelationshipVertexType TargetType { get; set; }

        /// <summary>
        /// Time when the relationship started.
        /// </summary>
        public long? StartTime { get; set; }

        /// <summary>
        /// Time when the relationship ended.
        /// </summary>
        public long? EndTime { get; set; }

        /// <summary>
        /// Confidence value of the existence of this relationship.
        /// Humans should enter 1.0 usually, generated relationships
        /// should provide a realistic score on the likelihood of the
        /// existence of the relationship. Generated relationships
        /// should never have the a confidence score of 1.0.
        /// </summary>
        public double? Confidence { get; set; }

        /// <summary>
        /// String describing the source system storing or generating the relationship.
        /// </summary>
        public Int64? DataSetId { get; set; }

        /// <summary>
        /// A list of labels associated with the relationships
        /// </summary>
        public IEnumerable<CogniteExternalId> Labels { get; set; }

        /// <summary>
        /// Time when this relationship was created.
        /// </summary>
        public long CreatedTime { get; set; }

        /// <summary>
        /// Time when this relationship was last updated.
        /// </summary>
        public long LastUpdatedTime { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
