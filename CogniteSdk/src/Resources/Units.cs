// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.FSharp.Core;
using Oryx;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// Contains all units methods.
    /// </summary>
    public class UnitsResource : Resource
    {
        /// <summary>
        /// The class constructor. Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">Authentication handler.</param>
        /// <param name="ctx">The HTTP context to use for the request.</param>
        internal UnitsResource(Func<CancellationToken, Task<string>> authHandler, FSharpFunc<IHttpNext<Unit>, Task<Unit>> ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Retrieve a list of all units in the unit catalog.
        /// </summary>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of all units in the unit catalog</returns>
        public async Task<IItemsWithoutCursor<UnitItem>> ListUnitsAsync(CancellationToken token = default)
        {
            var req = Oryx.Cognite.Units.listUnits(GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves a unit given it's external id.
        /// </summary>
        /// <param name="unitExternalId">The external id of the unit to get.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Unit with the given external id.</returns>
        public async Task<IItemsWithoutCursor<UnitItem>> GetUnitAsync(string unitExternalId, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Units.getUnit(unitExternalId, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves multiple units by external id.
        /// </summary>
        /// <param name="externalIds">The list of units to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>A sequence of the requested units.</returns>
        public async Task<ItemsWithIgnoreUnknownIds<UnitItem>> RetrieveUnitsAsync(IEnumerable<string> externalIds, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            if (externalIds is null)
            {
                throw new ArgumentNullException(nameof(externalIds));
            }

            var req = Oryx.Cognite.Units.retrieveUnits(externalIds, ignoreUnknownIds, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieve a list of all unit systems in the unit catalog.
        /// </summary>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of all unit systems in the unit catalog</returns>
        public async Task<IItemsWithoutCursor<UnitSystem>> ListUnitSystemsAsync(CancellationToken token = default)
        {
            var req = Oryx.Cognite.Units.listUnitSystems(GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }
    }
}