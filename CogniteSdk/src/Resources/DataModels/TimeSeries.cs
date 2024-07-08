using System.Collections.Generic;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using CogniteSdk.Beta.DataModels;
using CogniteSdk.Beta.DataModels.Core;
using CogniteSdk.Resources.Beta;
using Com.Cognite.V1.Timeseries.Proto;

namespace CogniteSdk.Resources.DataModels
{
    /// <summary>
    /// Base resource for time series in core data models.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CoreTimeSeriesResource<T> : BaseDataModelResource<T> where T : TimeSeriesBase
    {
        /// <inheritdoc />
        public override ViewIdentifier View => new ViewIdentifier("cdf_cdm_experimental", "TimeSeriesBase", "v1");

        private readonly TimeSeriesResource _tsResource;
        private readonly DataPointsResource _dpResource;

        /// <inheritdoc />
        public CoreTimeSeriesResource(
            DataModelsResource resource,
            TimeSeriesResource tsResource,
            DataPointsResource dpResource) : base(resource)
        {
            _tsResource = tsResource;
            _dpResource = dpResource;
        }

        /// <summary>
        /// Asynchronously retrieve a list of time series matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of time series matching given filters and optional cursor</returns>
        public Task<IEnumerable<DataPointsSyntheticItem>> SyntheticQueryAsync(TimeSeriesSyntheticQuery query, CancellationToken token = default)
        {
            return _tsResource.SyntheticQueryAsync(query, token);
        }

        /// <summary>
        /// Retrieves list of time series matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of assets matching given filters and optional cursor</returns>
        public Task<DataPointListResponse> ListDataPointsAsync(DataPointsQuery query, CancellationToken token = default)
        {
            return _dpResource.ListAsync(query, token);
        }

        /// <summary>
        /// Create data points.
        /// </summary>
        /// <param name="points">Data Points to create.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Empty response.</returns>
        public Task<EmptyResponse> CreateDataPointsAsync(DataPointInsertionRequest points, CancellationToken token = default)
        {
            return _dpResource.CreateAsync(points, token);
        }

        /// <summary>
        /// Create data points, applying Gzip compression at level <paramref name="compression"/>.
        /// </summary>
        /// <param name="points">Data Points to create</param>
        /// <param name="compression">Compression level</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Empty response</returns>
        public Task<EmptyResponse> CreateDataPointsAsync(
            DataPointInsertionRequest points,
            CompressionLevel compression,
            CancellationToken token = default)
        {
            return _dpResource.CreateAsync(points, compression, token);
        }

        /// <summary>
        /// Delete multiple assets in the same project, along with all their descendants in the asset hierarchy if
        /// recursive is true.
        /// </summary>
        /// <param name="query">The list of assets to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Empty response.</returns>
        public Task<EmptyResponse> DeleteDataPointsAsync(DataPointsDelete query, CancellationToken token = default)
        {
            return _dpResource.DeleteAsync(query, token);
        }

        /// <summary>
        /// Retrieve the latest datapoint in the given time series
        /// </summary>
        /// <param name="query">The latest query.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of latest data points.</returns>
        public Task<IEnumerable<DataPointsItem<DataPoint>>> LatestDataPointsAsync(DataPointsLatestQuery query, CancellationToken token = default)
        {
            return _dpResource.LatestAsync(query, token);
        }
    }
}