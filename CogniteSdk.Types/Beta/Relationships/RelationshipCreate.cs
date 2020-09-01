// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Class for writing relationship.
    /// </summary>
    public class RelationshipCreate
    {
        /// <summary>
        /// External id of the relationship.
        /// </summary>
        public string ExternalId { get; set; }


        /// <summary>
        /// The source externalId of the relationship.
        /// </summary>
        public string SourceExternalId { get; set; }

        /// <summary>
        /// The resource type of the source
        /// </summary>
        public string SourceType {get; set;}

        /// <summary>
        /// The target of the relationship.
        /// </summary>
        public string TargetExternalId { get; set; }

        /// <summary>
        /// The resource type of the target
        /// </summary>
        public string TargetType {get; set;}

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
        /// The id of the dataset this relationship belongs to.
        /// </summary>
        public Int64 DataSetId { get; set; }

        /// <summary>
        /// List of labels to associate with the relationship.
        /// </summary>
        public IEnumerable<CogniteExternalId> Labels { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
