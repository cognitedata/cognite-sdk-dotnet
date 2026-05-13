using System.Collections.Generic;
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
        /// <summary>
        /// Default view 
        /// </summary>
        public static ViewIdentifier DefaultView = new ViewIdentifier("cdf_cdm", "CogniteActivity", "v1");
        /// <inheritdoc />
        public override ViewIdentifier View { get; }

        /// <inheritdoc />
        public CoreActivityResource(
            DataModelsResource resource,
            ViewIdentifier view,
            IEnumerable<ViewIdentifier> allowedViewIdentifiers = null) : base(resource, allowedViewIdentifiers)
        {
            View = view?.Clone() ?? DefaultView;
        }
    }
}
