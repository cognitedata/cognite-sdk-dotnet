using CogniteSdk.DataModels;
using CogniteSdk.DataModels.Core;

namespace CogniteSdk.Resources.DataModels
{
    /// <summary>
    /// Base resource for activity in core data models.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CoreActivityResource<T> : BaseDataModelResource<T> where T : CogniteActivity
    {
        /// <inheritdoc />
        public override ViewIdentifier View { get; }

        /// <inheritdoc />
        public CoreActivityResource(
            DataModelsResource resource,
            ViewIdentifier view) : base(resource)
        {
            View = view ?? new ViewIdentifier("cdf_cdm", "CogniteActivity", "v1");
        }
    }
}
