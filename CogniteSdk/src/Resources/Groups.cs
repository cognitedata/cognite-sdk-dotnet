// Copyright 2021 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Oryx;


namespace CogniteSdk.Resources
{
    /// <summary>
    /// For internal use. Contains all group methods.
    /// </summary>
    public class GroupsResource : Resource
    {
        /// <summary>
        /// The class constructor. Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">Authentication handler.</param>
        /// <param name="ctx">The HTTP context to use for the request.</param>
        internal GroupsResource(Func<CancellationToken, Task<string>> authHandler, HttpContext ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// List groups
        /// </summary>
        /// <param name="all">True to fetch all groups, false to only fetch those belonging to the current user</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>List of retrieved groups</returns>
        public async Task<IEnumerable<Group>> ListAsync(bool all, CancellationToken token = default)
        {
            var query = new GroupQuery { All = all };

            var req = Oryx.Cognite.Groups.list(query);
            var result = await RunAsync(req, token).ConfigureAwait(false);
            return result.Items;
        }

        /// <summary>
        /// List groups, fetching only those belonging to the current user.
        /// </summary>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>List of retrieved groups</returns>
        public async Task<IEnumerable<Group>> ListAsync(CancellationToken token = default)
        {
            return await ListAsync(false, token);
        }

        /// <summary>
        /// Create a list of groups in CDF.
        /// </summary>
        /// <param name="items">List of groups to create.</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>List of created groups</returns>
        public async Task<IEnumerable<Group>> CreateAsync(IEnumerable<GroupCreate> items, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Groups.create(items);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete a list of groups by internal id.
        /// </summary>
        /// <param name="ids">Internal ids of groups to delete</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns></returns>
        public async Task DeleteAsync(IEnumerable<long> ids, CancellationToken token = default)
        {
            var query = new GroupDelete
            {
                Items = ids
            };

            var req = Oryx.Cognite.Groups.delete(query);
            await RunAsync(req, token).ConfigureAwait(false);
        }
    }
}
