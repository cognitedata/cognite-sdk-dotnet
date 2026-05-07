using System.Collections.Generic;
using CogniteSdk.DataModels;
using CogniteSdk.DataModels.Core;

namespace CogniteSdk.Resources.DataModels
{
    /// <summary>
    /// Base resource for asset in core data models.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CoreAssetResource<T> : BaseDataModelResource<T> where T : CogniteAssetBase
    {
        /// <summary>
        /// Default view 
        /// </summary>
        public static ViewIdentifier DefaultView = new ViewIdentifier("cdf_cdm", "CogniteAsset", "v1");
        /// <inheritdoc />
        public override ViewIdentifier View { get; }

        /// <inheritdoc />
        public CoreAssetResource(
            DataModelsResource resource,
            ViewIdentifier view,
            HashSet<ViewIdentifier> allowedViewIdentifiers = null) : base(resource, allowedViewIdentifiers)
        {
            View = view ?? DefaultView;
        }
    }
}
