// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk
{
    /// <summary>
    /// The Annotation common attributes
    /// </summary>
    public abstract class AnnotationCommon: AnnotationMetaData
    {
        /// <summary>
        /// The internal ID of the annotated resource.
        /// </summary>
        public long AnnotatedResourceId {get; set;}

    }
    /// <summary>
    /// The Annotation metadata
    /// </summary>
    public abstract class AnnotationMetaData
    {
        /// <summary>
        /// The type of the annotation. This uniquely decides what the structure of the 'Data' block will be.
        /// </summary>
        public string AnnotationType {get; set;}
        /// <summary>
        /// Type name of the CDF resource that is annotated, e.g. "file".
        /// </summary> 
        public string AnnotatedResourceType {get; set;}
        /// <summary>
        /// The name of the app from which this annotation was created.
        /// </summary>
        public string CreatingApp {get; set;}
        /// <summary>
        /// The version of the app that created this annotation. Must be a valid semantic versioning (SemVer) string.
        /// </summary>
        public string CreatingAppVersion {get; set;}
        /// <summary>
        /// A username, or email, or name. This is not checked nor enforced. If the value is None, it means the annotation was created by a service.
        /// </summary>
        public string CreatingUser {get; set;}
    }
}
