using System.Collections.Generic;
using CogniteSdk.DataModels;
using CogniteSdk.DataModels.Core;

namespace CogniteSdk.Resources.DataModels
{
    /// <summary>
    /// Base resource for file in core data models.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CoreFileResource<T> : BaseDataModelResource<T> where T : CogniteFile
    {
        /// <summary>
        /// Default view 
        /// </summary>
        public static ViewIdentifier DefaultView = new ViewIdentifier("cdf_cdm", "CogniteFile", "v1");
        /// <inheritdoc />
        public override ViewIdentifier View { get; }

        /// <inheritdoc />
        public CoreFileResource(
            DataModelsResource resource,
            ViewIdentifier view,
            IEnumerable<ViewIdentifier> allowedViewIdentifiers = null) : base(resource, allowedViewIdentifiers)
        {
            View = view ?? DefaultView;
        }
    }
}
