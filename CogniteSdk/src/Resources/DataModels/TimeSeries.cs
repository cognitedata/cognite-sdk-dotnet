using System.Collections.Generic;
using CogniteSdk.Beta.DataModels;
using CogniteSdk.Beta.DataModels.Core;
using CogniteSdk.Resources.Beta;

namespace CogniteSdk.Resources.DataModels
{
    /// <summary>
    /// Base resource for time series in core data models.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CoreTimeSeriesResource<T> : BaseDataModelResource<T> where T : CogniteTimeSeriesBase
    {
        /// <inheritdoc />
        public override ViewIdentifier View { get; }

        private readonly Alpha.DataPointsResource _dpResource;

        /// <inheritdoc />
        public CoreTimeSeriesResource(
            DataModelsResource resource,
            Alpha.DataPointsResource dpResource,
            ViewIdentifier view) : base(resource)
        {
            _dpResource = dpResource;
            View = view ?? new ViewIdentifier("cdf_cdm", "CogniteTimeSeries", "v1");
        }
    }
}
