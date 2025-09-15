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
        /// <inheritdoc />
        public override ViewIdentifier View { get; }

        /// <inheritdoc />
        public CoreFileResource(
            DataModelsResource resource,
            ViewIdentifier view) : base(resource)
        {
            View = view ?? new ViewIdentifier("cdf_cdm", "CogniteFile", "v1");
        }
    }
}
