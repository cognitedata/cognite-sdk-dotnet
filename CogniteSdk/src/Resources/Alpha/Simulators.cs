// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using CogniteSdk.Alpha;

using Oryx;
using Oryx.Cognite.Alpha;
using Oryx.Pipeline;

using Microsoft.FSharp.Core;
using System.Linq;

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
        public SimulatorsResource(Func<CancellationToken, Task<string>> authHandler, FSharpFunc<IAsyncNext<HttpContext, Unit>, Task<Unit>> ctx) : base(authHandler, ctx)
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
        public async Task<IItemsWithoutCursor<SimulationRun>> ListSimulationRunsAsync(SimulationRunQuery query, CancellationToken token = default)
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
        public async Task<IItemsWithoutCursor<SimulatorModel>> ListSimulatorModelsAsync(SimulatorModelQuery query, CancellationToken token = default)
        {
            var req = Simulators.listSimulatorModels(query, GetContext(token));
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
        public async Task<IItemsWithoutCursor<SimulatorModelRevision>> ListSimulatorModelRevisionsAsync(SimulatorModelRevisionQuery query, CancellationToken token = default)
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
        /// Asyncronously creates a simulation routine of predefined type.
        /// </summary>
        /// <param name="items">The simulator routine items to create.</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task<IEnumerable<SimulatorRoutine>> CreateSimulatorRoutinesPredefinedAsync(IEnumerable<SimulatorRoutineCreateCommandPredefined> items, CancellationToken token = default) 
        {
            var req = Simulators.createSimulatorRoutinesPredefined(items, GetContext(token));
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
        /// <param name="query">The simulator routine revisions query to retreive.</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task<IItemsWithoutCursor<SimulatorRoutineRevision>> ListSimulatorRoutineRevisionsAsync(SimulatorRoutineRevisionQuery query, CancellationToken token = default) 
        {
            var req = Simulators.listSimulatorRoutineRevisions(query, GetContext(token));
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
