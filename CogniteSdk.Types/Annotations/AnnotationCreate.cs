// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0
using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The Annotation create class
    /// </summary>
    public class AnnotationCreate : AnnotationCommon
    {
        /// <summary>
        /// The annotation information. The format of this object is decided by and validated against the 'AnnotationType' attribute.
        /// </summary>
        public BoundingVolume Data { get; set; }
        /// <summary>
        /// The status of the annotation.
        /// </summary>
        public string Status { get; set; }
        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
