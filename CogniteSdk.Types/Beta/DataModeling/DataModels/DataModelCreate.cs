// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Create or update a data model.
    /// </summary>
    public class DataModelCreate
    {
        /// <summary>
        /// Id of the space that the data model belongs to.
        /// </summary>
        public string Space { get; set; }
        /// <summary>
        /// External ID that uniquely identifies this data model.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// Human readable name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Data model description.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Data model version.
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// List of views included in this data model,
        /// you can use a reference to an existing view, specify a new view,
        /// or update an existing view.
        /// </summary>
        public IEnumerable<IViewCreateOrReference> Views { get; set; }
    }

    /// <summary>
    /// Either a view reference or a view create
    /// </summary>
    public interface IViewCreateOrReference { }

    /// <summary>
    /// Json converter for ViewIdentifier, or ViewCreate.
    /// </summary>
    public class ViewCreateOrReferenceConverter : UntaggedUnionConverter<IViewCreateOrReference>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ViewCreateOrReferenceConverter() : base(new[]
        {
            typeof(ViewIdentifier), typeof(ViewCreate)
        })
        {
        }
    }
}
