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
        /// <inheritdoc />
        public override ViewIdentifier View { get; }

        /// <inheritdoc />
        public CoreAssetResource(
            DataModelsResource resource,
            ViewIdentifier view) : base(resource)
        {
            View = view ?? new ViewIdentifier("cdf_cdm", "CogniteAsset", "v1");
        }
    }
}
