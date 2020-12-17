// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Oryx.Cognite;
using static Oryx.Cognite.HandlerModule;
using HttpContext = Oryx.Context<Microsoft.FSharp.Core.Unit>;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// For internal use. Contains all ThreeD methods.
    /// </summary>
    public class ThreeDModelsResource : Resource
    {
        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">Authentication handler.</param>
        /// <param name="ctx">Context to use for the request.</param>
        internal ThreeDModelsResource(Func<CancellationToken, Task<string>> authHandler, HttpContext ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Retrieves list of ThreeDModels matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of ThreeDModel matching given filters and optional cursor</returns>
        public async Task<ItemsWithCursor<ThreeDModel>> ListAsync(ThreeDModelQuery query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = ThreeDModels.list<ItemsWithCursor<ThreeDModel>>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Create ThreeDModel.
        /// </summary>
        /// <param name="ThreeDModel">ThreeDModel to create.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Sequence of created ThreeDModel.</returns>
        public async Task<IEnumerable<ThreeDModel>> CreateAsync(IEnumerable<ThreeDModelCreate> ThreeDModel, CancellationToken token = default)
        {
            if (ThreeDModel is null)
            {
                throw new ArgumentNullException(nameof(ThreeDModel));
            }

            var req = ThreeDModels.create<IEnumerable<ThreeDModel>>(ThreeDModel);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        #region Delete overloads
        /// <summary>
        /// Delete multiple ThreeDModel in the same project, along with all their descendants in the ThreeD hierarchy if
        /// recursive is true.
        /// </summary>
        /// <param name="ids">Ids of ThreeDModels to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(IEnumerable<Identity> ids, CancellationToken token = default)
        {
            if (ids is null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var req = ThreeDModels.delete<EmptyResponse>(ids);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple ThreeDModel in the same project by internal ids.
        /// </summary>
        /// <param name="internalIds">The list of ThreeDModel ids to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(IEnumerable<long> internalIds, CancellationToken token = default)
        {
            if (internalIds is null)
            {
                throw new ArgumentNullException(nameof(internalIds));
            }

            var query = internalIds.Select(Identity.Create);
            return await DeleteAsync(query, token).ConfigureAwait(false);
        }

        #endregion

        #region Retrieve overloads
        /// <summary>
        /// Retrieves information about multiple ThreeDModel in the same project. A maximum of 1000 ThreeDModel IDs may be listed
        /// per request and all of them must be unique.
        /// </summary>
        /// <param name="modelId">The ThreeDModel id to retrieve.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<ThreeDModel> RetrieveAsync(long modelId, CancellationToken token = default)
        {
            var req = ThreeDModels.retrieve<ThreeDModel>(modelId);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        #endregion

        /// <summary>
        /// Update one or more ThreeDModel. Supports partial updates, meaning that fields omitted from the requests are not
        /// changed
        /// </summary>
        /// <param name="query">The list of ThreeDModel to update.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of updated ThreeDModel.</returns>
        public async Task<IEnumerable<ThreeDModel>> UpdateAsync (IEnumerable<ThreeDModelUpdateItem> query, CancellationToken token = default )
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = ThreeDModels.update<IEnumerable<ThreeDModel>>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }
    }
}