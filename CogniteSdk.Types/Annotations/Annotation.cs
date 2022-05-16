// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0
using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The Annotation read class
    /// </summary>
    public class Annotation: AnnotationCommon
    {
        /// <summary>
        /// The Id of the annotation.
        /// </summary>
        public long Id {get; set;}
        /// <summary>
        /// The annotation information. The format of this object is decided by and validated against the 'AnnotationType' attribute.
        /// </summary>
        public BoundingVolume Data {get; set;}
        /// <summary>
        /// Time when this annotation was created in CDF. The time is measured in milliseconds since 00:00:00 Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus leap seconds. Read-only.
        /// </summary>
        public long CreatedTime { get; set; }
        /// <summary>
        /// Time when this annotation was last updated in CDF. The time is measured in milliseconds since 00:00:00 Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus leap seconds. Read-only.
        /// </summary>
        public long LastUpdatedTime { get; set; }
        /// <summary>
        /// The status of the annotation.
        /// </summary>
        public string Status {get; set;}
        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
