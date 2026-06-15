using System.Collections.Generic;
using CogniteSdk.DataModels;
using CogniteSdk.DataModels.Core;

namespace CogniteSdk.Resources.DataModels
{
    /// <summary>
    /// Base resource for state sets in core data models.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CoreStateSetResource<T> : BaseDataModelResource<T> where T : CogniteStateSet
    {
        /// <summary>
        /// Default view
        /// </summary>
        public static ViewIdentifier DefaultView = new ViewIdentifier("cdf_cdm", "CogniteStateSet", "v1");
        /// <inheritdoc />
        public override ViewIdentifier View { get; }

        /// <inheritdoc />
        public CoreStateSetResource(
            DataModelsResource resource,
            ViewIdentifier view,
            IEnumerable<ViewIdentifier> allowedViewIdentifiers = null) : base(resource, allowedViewIdentifiers)
        {
            View = view?.Clone() ?? DefaultView;
        }
    }
}
