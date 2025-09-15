// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.DataModels
{
    /// <summary>
    /// Base properties of data model shared between create and retrieve types
    /// </summary>
    public class BaseDataModel
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
    }

    /// <summary>
    /// Definition of a data model.
    /// </summary>
    public class DataModel : BaseDataModel
    {
        /// <summary>
        /// Time when this data model was created in CDF in milliseconds since Jan 1, 1970.
        /// </summary>
        public long CreatedTime { get; set; }
        /// <summary>
        /// The last time this data model was updated in CDF, in milliseconds since Jan 1, 1970.
        /// </summary>
        /// <value></value>
        public long LastUpdatedTime { get; set; }
        /// <summary>
        /// List of views included in this data model,
        /// either flat view references, or full definitions if the
        /// InlineViews query parameter is set.
        /// Use <see cref="View"/> for full, and <see cref="ViewIdentifier"/>
        /// for references.
        /// </summary>
        public IEnumerable<IViewDefinitionOrReference> Views { get; set; }
    }

    /// <summary>
    /// Either a view definition, or a view reference.
    /// </summary>
    public interface IViewDefinitionOrReference { }

    /// <summary>
    /// Json converter for ViewIdentifier, or ViewCreate.
    /// </summary>
    public class ViewDefinitionOrReferenceConverter : UntaggedUnionConverter<IViewDefinitionOrReference>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ViewDefinitionOrReferenceConverter() : base(new[]
        {
            typeof(View), typeof(ViewIdentifier)
        })
        {
        }
    }
}
