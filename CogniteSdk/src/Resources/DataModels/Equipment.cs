using CogniteSdk.DataModels;
using CogniteSdk.DataModels.Core;

namespace CogniteSdk.Resources.DataModels
{
    /// <summary>
    /// Base resource for equipment in core data models.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CoreEquipmentResource<T> : BaseDataModelResource<T> where T : CogniteEquipment
    {
        /// <inheritdoc />
        public override ViewIdentifier View { get; }

        /// <inheritdoc />
        public CoreEquipmentResource(
            DataModelsResource resource,
            ViewIdentifier view) : base(resource)
        {
            View = view ?? new ViewIdentifier("cdf_cdm", "CogniteEquipment", "v1");
        }
    }
}
