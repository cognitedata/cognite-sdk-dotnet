// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0
using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The Annotation update class
    /// </summary>
    public class AnnotationUpdate
    {
        /// <summary>
        /// Change the annotation type of the object.
        /// </summary>
        public Update<string> AnnotationType {get; set;}
        /// <summary>
        /// Change the annotation payload of the object.
        /// </summary>
        public Update<BoundingVolume> Data {get; set;}
        /// <summary>
        /// Change the status of the object.
        /// </summary>
        public Update<string> Status {get; set;}
        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }

    /// <summary>
    /// The annotation update item type.
    /// </summary>
    public class AnnotationUpdateItem : UpdateItem<AnnotationUpdate>
    {
        /// <summary>
        /// Initialize the annotation update item with an internal Id.
        /// </summary>
        /// <param name="id">Internal Id to set.</param>
        public AnnotationUpdateItem(long id) : base(id)
        {
        }
    }
}
