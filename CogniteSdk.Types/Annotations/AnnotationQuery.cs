// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0
using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The Annotation query class
    /// </summary>
    public class AnnotationQuery : CursorQueryBase
    {
        /// <summary>
        /// A filter to apply on annotations
        /// </summary>
        public AnnotationFilter Filter {get; set;}

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
