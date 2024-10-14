// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Resources.DataModels;

namespace CogniteSdk.Beta.DataModels.Core
{
    /// <summary>
    /// Resource for CDF core data models.
    /// </summary>
    public class CoreResource
    {
        private readonly Client _client;

        /// <summary>
        /// Resource for core data model time series.
        /// </summary>
        /// <param name="view">ID of the view to write to. Defaults to the TimeSeriesBase view in
        /// the core data model.</param>
        /// <typeparam name="T">Time series type.</typeparam>
        /// <returns>Core data model time series resource.</returns>
        public CoreTimeSeriesResource<T> TimeSeries<T>(ViewIdentifier view = null) where T : CogniteTimeSeriesBase
        {
            return new CoreTimeSeriesResource<T>(_client.Beta.DataModels, _client.DataPoints, view);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="client">CDF Client</param>
        public CoreResource(Client client)
        {
            _client = client;
        }
    }
}
