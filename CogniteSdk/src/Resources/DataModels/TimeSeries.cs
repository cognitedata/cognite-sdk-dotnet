using System.Collections.Generic;
using CogniteSdk.DataModels;
using CogniteSdk.DataModels.Core;

namespace CogniteSdk.Resources.DataModels
{
    /// <summary>
    /// Base resource for time series in core data models.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CoreTimeSeriesResource<T> : BaseDataModelResource<T> where T : CogniteTimeSeriesBase
    {
        /// <summary>
        /// Default view 
        /// </summary>
        public static ViewIdentifier DefaultView = new ViewIdentifier("cdf_cdm", "CogniteTimeSeries", "v1");
        /// <inheritdoc />
        public override ViewIdentifier View { get; }

        private readonly DataPointsResource _dpResource;

        /// <inheritdoc />
        public CoreTimeSeriesResource(
            DataModelsResource resource,
            DataPointsResource dpResource,
            ViewIdentifier view,
            HashSet<ViewIdentifier> allowedViewIdentifiers = null) : base(resource, allowedViewIdentifiers)
        {
            _dpResource = dpResource;
            View = view ?? DefaultView;
        }
    }
}
