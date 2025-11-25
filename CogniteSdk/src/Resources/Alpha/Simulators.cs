// Copyright Cognite AS
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CogniteSdk.Alpha;
using Microsoft.FSharp.Core;
using Oryx;
using Oryx.Cognite.Alpha;

namespace CogniteSdk.Resources.Alpha
{
    /// <summary>
    /// Resource for simulators
    /// </summary>
    public class SimulatorsResource : Resource
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public SimulatorsResource(Func<CancellationToken, Task<string>> authHandler, FSharpFunc<IHttpNext<Unit>, Task<Unit>> ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Create simulation runs
        /// </summary>
        /// <param name="items">Simulation run items to create</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>A list of created simulation runs</returns>
        public async Task<IEnumerable<SimulationRun>> CreateSimulationRunsAsync(IEnumerable<SimulationRunCreate> items, CancellationToken token = default)
        {
            var req = Simulators.createSimulationRuns(items, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// List all simulation runs
        /// </summary>
        /// <param name="query">Simulation run filter query</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>All simulation runs in project</returns>
        public async Task<IItemsWithCursor<SimulationRun>> ListSimulationRunsAsync(SimulationRunQuery query, CancellationToken token = default)
        {
            var req = Simulators.listSimulationRuns(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Change the status of the simulation run (used by the simulator connector to report progress)
        /// </summary>
        /// <param name="query">Simulation run callback query</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>The updated simulation run</returns>
        public async Task<IItemsWithoutCursor<SimulationRun>> SimulationRunCallbackAsync(SimulationRunCallbackItem query, CancellationToken token = default)
        {
            var req = Simulators.simulationRunCallback(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieves information about multiple simulation runs. A maximum of 1000
        /// </summary>
        /// <param name="internalIds">The simulation run internal ids to retrieve.</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>The requested simulation runs</returns>
        public async Task<IEnumerable<SimulationRun>> RetrieveSimulationRunsAsync(IEnumerable<long> internalIds, CancellationToken token = default)
        {
            if (internalIds is null)
            {
                throw new ArgumentNullException(nameof(internalIds));
            }

            var ids = internalIds.Select(Identity.Create);
            var req = Simulators.retrieveSimulationRuns<SimulationRun>(ids, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieves inputs and outputs items for multiple simulation runs.
        /// </summary>
        /// <param name="runIds">The simulation run ids to retrieve data for.</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>The requested simulation runs data</returns>
        public async Task<IEnumerable<SimulationRunData>> ListSimulationRunsDataAsync(IEnumerable<long> runIds, CancellationToken token = default)
        {
            if (runIds is null)
            {
                throw new ArgumentNullException(nameof(runIds));
            }

            var ids = runIds.Select(SimulationRunId.Create);
            var req = Simulators.listSimulationRunsData(ids, GetContext(token));
            var res = await RunAsync(req).ConfigureAwait(false);
            return res.Items;
        }

        /// <summary>
        /// Asynchronously retrieves information about multiple simulator integrations. A maximum of 1000
        /// </summary>
        /// <param name="query">The simulator integration query to retrieve.</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task<IItemsWithoutCursor<SimulatorIntegration>> ListSimulatorIntegrationsAsync(SimulatorIntegrationQuery query, CancellationToken token = default)
        {
            var req = Simulators.listSimulatorIntegrations(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously updates multiple simulator integrations. Only one at a time is supported as of now.
        /// </summary>
        /// <param name="items">The simulator integration items to update.</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task<IEnumerable<SimulatorIntegration>> UpdateSimulatorIntegrationAsync(IEnumerable<UpdateItem<SimulatorIntegrationUpdate>> items, CancellationToken token = default)
        {
            if (items is null) throw new ArgumentNullException(nameof(items));
            var req = Simulators.updateSimulatorIntegrations(items, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously creates multiple simulator integrations. Only one at a time is supported as of now.
        /// </summary>
        /// <param name="items">The simulator integration items to create.</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task<IEnumerable<SimulatorIntegration>> CreateSimulatorIntegrationAsync(IEnumerable<SimulatorIntegrationCreate> items, CancellationToken token = default)
        {
            var req = Simulators.createSimulatorIntegrations(items, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Asyncronously creates a simulation model. Only one at a time is supported as of now.
        /// </summary>
        /// <param name="items">The simulator model items to create.</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task<IEnumerable<SimulatorModel>> CreateSimulatorModelsAsync(IEnumerable<SimulatorModelCreate> items, CancellationToken token = default)
        {
            var req = Simulators.createSimulatorModels(items, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Asyncronously lists information about multiple simulation models. A maximum of 1000
        /// </summary>
        /// <param name="query">The simulator model query to filter.</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task<IItemsWithCursor<SimulatorModel>> ListSimulatorModelsAsync(SimulatorModelQuery query, CancellationToken token = default)
        {
            var req = Simulators.listSimulatorModels(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Asyncronously retrieves information about multiple simulation models.
        /// </summary>
        /// <param name="ids">The simulator model ids to retrieve.</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>The requested simulation models</returns>
        public async Task<IEnumerable<SimulatorModel>> RetrieveSimulatorModelsAsync(IEnumerable<Identity> ids, CancellationToken token = default)
        {
            if (ids is null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var req = Simulators.retrieveSimulatorModels<SimulatorModel>(ids, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Asyncronously updates simulation models. Only one at a time is supported as of now.
        /// </summary>
        /// <param name="items">The simulator model items to update.</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task<IEnumerable<SimulatorModel>> UpdateSimulatorModelsAsync(IEnumerable<UpdateItem<SimulatorModelUpdate>> items, CancellationToken token = default)
        {
            var req = Simulators.updateSimulatorModels(items, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Asyncronously deletes simulation models. Only one at a time is supported as of now.
        /// </summary>
        /// <param name="items">The simulator model items to delete.</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task<EmptyResponse> DeleteSimulatorModelsAsync(IEnumerable<Identity> items, CancellationToken token = default)
        {
            var query = new SimulatorModelDelete { Items = items };
            var req = Simulators.deleteSimulatorModels(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Asyncronously creates a simulator model revision. Only one at a time is supported as of now.
        /// </summary>
        /// <param name="items">The simulator model revision items to create.</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task<IEnumerable<SimulatorModelRevision>> CreateSimulatorModelRevisionsAsync(IEnumerable<SimulatorModelRevisionCreate> items, CancellationToken token = default)
        {
            var req = Simulators.createSimulatorModelRevisions(items, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Asyncronously lists information about multiple simulator model revisions. A maximum of 1000
        /// </summary>
        /// <param name="query">The simulator model revision query to filter.</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task<IItemsWithCursor<SimulatorModelRevision>> ListSimulatorModelRevisionsAsync(SimulatorModelRevisionQuery query, CancellationToken token = default)
        {
            var req = Simulators.listSimulatorModelRevisions(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Asyncronously updates simulator model revisions. Only one at a time is supported as of now.
        /// </summary>
        /// <param name="items">The simulator model revision items to update.</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task<IEnumerable<SimulatorModelRevision>> UpdateSimulatorModelRevisionsAsync(IEnumerable<UpdateItem<SimulatorModelRevisionUpdate>> items, CancellationToken token = default)
        {
            var req = Simulators.updateSimulatorModelRevisions(items, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Asyncronously retrieves information about multiple simulator model revisions.
        /// </summary>
        /// <param name="ids">The simulator model revision ids to retrieve.</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>The requested simulator model revisions</returns>
        public async Task<IEnumerable<SimulatorModelRevision>> RetrieveSimulatorModelRevisionsAsync(IEnumerable<Identity> ids, CancellationToken token = default)
        {
            if (ids is null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var req = Simulators.retrieveSimulatorModelRevisions<SimulatorModelRevision>(ids, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }



        /// <summary>
        /// Asyncronously list simulator routines.
        /// </summary>
        /// <param name="query">The simulator routine query to retrieve.</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task<IItemsWithCursor<SimulatorRoutine>> ListSimulatorRoutinesAsync(SimulatorRoutineQuery query, CancellationToken token = default)
        {
            var req = Simulators.listSimulatorRoutines(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Asyncronously creates a simulation routine.
        /// </summary>
        /// <param name="items">The simulator routine items to create.</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task<IEnumerable<SimulatorRoutine>> CreateSimulatorRoutinesAsync(IEnumerable<SimulatorRoutineCreateCommandItem> items, CancellationToken token = default)
        {
            var req = Simulators.createSimulatorRoutines(items, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Asyncronously deletes simulator routines.
        /// </summary>
        /// <param name="items">The simulator routine items to delete.</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task<EmptyResponse> DeleteSimulatorRoutinesAsync(IEnumerable<Identity> items, CancellationToken token = default)
        {
            var query = new SimulatorRoutineDelete { Items = items };
            var req = Simulators.deleteSimulatorRoutines(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Asyncronously creates a set of simulation routine revisions .
        /// </summary>
        /// <param name="items">The simulator routine revision items to create.</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task<IEnumerable<SimulatorRoutineRevision>> CreateSimulatorRoutineRevisionsAsync(IEnumerable<SimulatorRoutineRevisionCreate> items, CancellationToken token = default)
        {
            var req = Simulators.createSimulatorRoutineRevisions(items, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// List simulator routine revisions.
        /// </summary>
        /// <param name="query">The simulator routine revisions query to retrieve.</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task<IItemsWithCursor<SimulatorRoutineRevision>> ListSimulatorRoutineRevisionsAsync(SimulatorRoutineRevisionQuery query, CancellationToken token = default)
        {
            var req = Simulators.listSimulatorRoutineRevisions(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Asyncronously retrieves simulator routine revisions.
        /// </summary>
        /// <param name="ids">The simulator routine revision ids to retrieve.</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>The requested simulator routine revisions</returns>
        public async Task<IEnumerable<SimulatorRoutineRevision>> RetrieveSimulatorRoutineRevisionsAsync(IEnumerable<Identity> ids, CancellationToken token = default)
        {
            if (ids is null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var req = Simulators.retrieveSimulatorRoutineRevisions<SimulatorRoutineRevision>(ids, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously updates simulator model revision data.
        /// </summary>
        /// <param name="items">The simulator model revision data items to update.</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task<IItemsWithoutCursor<SimulatorModelRevisionData>> UpdateSimulatorModelRevisionDataAsync(IEnumerable<SimulatorModelRevisionDataUpdateItem> items, CancellationToken token = default)
        {
            if (items is null) throw new ArgumentNullException(nameof(items));
            var req = Simulators.updateSimulatorModelRevisionData(items, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously retrieves simulator model revision data.
        /// </summary>
        /// <param name="modelRevisionExternalId">The external id of the model revision to retrieve data for.</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>The requested simulator model revision data</returns>
        public async Task<SimulatorModelRevisionData> RetrieveSimulatorModelRevisionDataAsync(string modelRevisionExternalId, CancellationToken token = default)
        {
            if (string.IsNullOrEmpty(modelRevisionExternalId))
            {
                throw new ArgumentNullException(nameof(modelRevisionExternalId));
            }

            var request = new ItemsWithoutCursor<SimulatorModelRevisionDataRetrieve>
            {
                Items = new[] { new SimulatorModelRevisionDataRetrieve { ModelRevisionExternalId = modelRevisionExternalId } }
            };

            var req = Simulators.retrieveSimulatorModelRevisionData(request, GetContext(token));
            var result = await RunAsync(req).ConfigureAwait(false);
            return result.Items.FirstOrDefault();
        }

        /// <summary>
        /// Asyncronously updates simulator logs.
        /// </summary>
        public async Task<EmptyResponse> UpdateSimulatorLogsAsync(IEnumerable<UpdateItem<SimulatorLogUpdate>> items, CancellationToken token = default)
        {
            var req = Simulators.updateSimulatorLogs(items, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates simulator logs, applying Gzip compression at level <paramref name="compression"/>.
        /// </summary>
        /// <param name="items">The simulator log update items to apply</param>
        /// <param name="compression">Compression level</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>Empty response</returns>
        public async Task<EmptyResponse> UpdateSimulatorLogsAsync(IEnumerable<UpdateItem<SimulatorLogUpdate>> items, CompressionLevel compression, CancellationToken token = default)
        {
            var req = Simulators.updateSimulatorLogsWithGzip(items, compression, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Asyncronously retrieves simulator logs.
        /// </summary>
        public async Task<IEnumerable<SimulatorLog>> RetrieveSimulatorLogsAsync(IEnumerable<Identity> ids, CancellationToken token = default)
        {
            if (ids is null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var req = Simulators.retrieveSimulatorLogs<SimulatorLog>(ids, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Asyncronously lists all simulators in the project.
        /// </summary>
        /// <param name="query">The simulator query to retrieve.</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task<IItemsWithoutCursor<Simulator>> ListAsync(SimulatorQuery query, CancellationToken token = default)
        {
            var req = Simulators.list(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynctonously creates simulators in the project. Only one at a time is supported as of now.
        /// </summary>
        /// <param name="items">The simulator items to create.</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task<IEnumerable<Simulator>> CreateAsync(IEnumerable<SimulatorCreate> items, CancellationToken token = default)
        {
            var req = Simulators.create(items, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }
        /// <summary>
        /// Asynchronously deletes simulators in the project. Only one at a time is supported as of now. 
        /// </summary>
        /// <param name="items">The simulator items to delete.</param>
        /// <param name="token">Optional cancellation token</param>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<EmptyResponse> DeleteAsync(IEnumerable<Identity> items, CancellationToken token = default)
        {

            if (items is null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            var query = new SimulatorDelete { Items = items };

            var req = Simulators.delete(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously updates simulators in the project. Only one at a time is supported as of now. 
        /// </summary>
        /// <param name="items">The simulator items to update.</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task<IEnumerable<Simulator>> UpdateAsync(IEnumerable<SimulatorUpdateItem> items, CancellationToken token = default)
        {
            var req = Simulators.update(items, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }
    }
}
