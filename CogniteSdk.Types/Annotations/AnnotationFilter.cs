// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk
{
    /// <summary>
    /// The Annotation filter class
    /// </summary>
    public class AnnotationFilter : AnnotationMetaData
    {
        /// <summary>
        /// A list of annotated resource ids
        /// </summary>
        public Identity[] AnnotatedResourceIds { get; set; }
    }
}
