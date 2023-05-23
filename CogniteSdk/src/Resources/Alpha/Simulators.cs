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
        /// List all simulation runs
        /// </summary>
        /// <param name="query">Simulation run filter query</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>All simulation runs in project</returns>
        public async Task<IItemsWithoutCursor<SimulationRun>> ListSimulationRuns(SimulationRunQuery query, CancellationToken token = default)
        {
            var req = Simulators.listSimulationRuns(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Change the status of the simulation run (used by the simulator connector to report progress)
        /// </summary>
        /// <param name="query">Simulation run callback query</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>All simulation runs in project</returns>
        public async Task<IItemsWithoutCursor<SimulationRun>> SimulationRunCallback(SimulationRunCallbackItem query, CancellationToken token = default)
        {
            var req = Simulators.simulationRunCallback(query, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }
    }
}
