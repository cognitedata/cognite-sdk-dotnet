using System.Collections.Generic;
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
        /// <summary>
        /// Default view 
        /// </summary>
        public static ViewIdentifier DefaultView = new ViewIdentifier("cdf_cdm", "CogniteEquipment", "v1");
        /// <inheritdoc />
        public override ViewIdentifier View { get; }

        /// <inheritdoc />
        public CoreEquipmentResource(
            DataModelsResource resource,
            ViewIdentifier view,
            IEnumerable<ViewIdentifier> allowedViewIdentifiers = null) : base(resource, allowedViewIdentifiers)
        {
            View = view?.Clone() ?? DefaultView;
        }
    }
}
