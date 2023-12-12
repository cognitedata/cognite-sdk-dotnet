﻿// Copyright 2023 Cognite AS
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
        /// Asynchronously retrieves information about multiple simulation runs in the same project. A maximum of 1000
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
        /// Asynchronously retrieves information about multiple simulator integrations in the same project. A maximum of 1000
        /// </summary>
        /// <param name="query">The simulator integration query to retrieve.</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task<IItemsWithoutCursor<SimulatorIntegration>> ListSimulatorIntegrationsAsync(SimulatorIntegrationQuery query, CancellationToken token = default)
        {
            var req = Simulators.listSimulatorIntegrations(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously updates multiple simulator integrations in the same project. Only one at a time is supported as of now.
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
        /// Asynchronously creates multiple simulator integrations in the same project. Only one at a time is supported as of now.
        /// </summary>
        /// <param name="items">The simulator integration items to create.</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task<IEnumerable<SimulatorIntegration>> CreateSimulatorIntegrationAsync(IEnumerable<SimulatorIntegrationCreate> items, CancellationToken token = default)
        {
            var req = Simulators.createSimulatorIntegrations(items, GetContext(token));
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
