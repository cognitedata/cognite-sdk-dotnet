// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.DataModels
{
    /// <summary>
    /// Create or update a data model.
    /// </summary>
    public class DataModelCreate : BaseDataModel
    {
        /// <summary>
        /// List of views included in this data model,
        /// you can use a reference to an existing view, specify a new view,
        /// or update an existing view.
        /// Use <see cref="ViewIdentifier"/> for existing views, or
        /// <see cref="ViewCreate"/> for creating a new view.
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
            typeof(ViewCreate), typeof(ViewIdentifier)
        })
        {
        }
    }
}
