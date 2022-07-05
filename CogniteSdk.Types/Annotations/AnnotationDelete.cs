// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0
using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// Delete a list of labels.
    /// </summary>
    public class AnnotationDelete : ItemsWithoutCursor<AnnotationId>
    {
        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
    /// <summary>
    /// Annotation Id.
    /// </summary>
    public class AnnotationId
    {
        /// <summary>
        /// Annotation Id.
        /// </summary>
        public long Id { get; set; }
    }
}
