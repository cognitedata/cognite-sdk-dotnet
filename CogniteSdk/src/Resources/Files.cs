// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CogniteSdk.Files;
using static Oryx.Cognite.HandlerModule;
using HttpContext = Oryx.Context<System.Net.Http.HttpResponseMessage>;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// For internal use. Contains all event methods.
    /// </summary>
    public class FilesResource
    {
        private readonly HttpContext _ctx;

        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="ctx">Context to use for the request.</param>
        internal FilesResource(HttpContext ctx)
        {
            _ctx = ctx;
        }

        /// <summary>
        /// Retrieves list of files matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of assets matching given filters and optional cursor</returns>
        public async Task<ItemsWithCursor<FileReadDto>> ListAsync(FileQueryDto query, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Files.list<ItemsWithCursor<FileReadDto>>(query);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves information about an file given a file id.
        /// </summary>
        /// <param name="fileId">The id of the file to get.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>File with the given id.</returns>
        public async Task<FileReadDto> GetAsync(long fileId, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Files.get<FileReadDto>(fileId);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        #region Retrieve overloads
        /// <summary>
        /// Retrieves metadata information about multiple specific files in the same project. Results are returned in the same order as in the request. This operation does not return the file contents.
        /// </summary>
        /// <param name="ids">The list of file identities to retrieve.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<FileReadDto>> RetrieveAsync(IEnumerable<Identity> ids, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Files.retrieve<IEnumerable<FileReadDto>>(ids);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves metadata information about multiple specific files in the same project. Results are returned in the same order as in the request. This operation does not return the file contents.
        /// </summary>
        /// <param name="internalIds">The list of file internal identities to retrieve.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<FileReadDto>> RetrieveAsync(IEnumerable<long> internalIds, CancellationToken token = default)
        {
            var ids = internalIds.Select(Identity.Create);
            return await RetrieveAsync(ids, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves metadata information about multiple specific files in the same project. Results are returned in the same order as in the request. This operation does not return the file contents.
        /// </summary>
        /// <param name="externalIds">The list of file internal identities to retrieve.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<FileReadDto>> RetrieveAsync(IEnumerable<string> externalIds, CancellationToken token = default)
        {
            var ids = externalIds.Select(Identity.Create);
            return await RetrieveAsync(ids, token).ConfigureAwait(false);
        }
        #endregion

        #region Download overloads
        public async Task<IEnumerable<FileDownloadDto>> DownloadAsync(IEnumerable<Identity> ids, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Files.download<IEnumerable<FileDownloadDto>>(ids);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }

        public async Task<IEnumerable<FileDownloadDto>> DownloadAsync(IEnumerable<long> internalIds, CancellationToken token = default)
        {
            var ids = internalIds.Select(Identity.Create);
            return await DownloadAsync(ids, token).ConfigureAwait(false);
        }

        public async Task<IEnumerable<FileDownloadDto>> DownloadAsync(IEnumerable<string> externalIds, CancellationToken token = default)
        {
            var ids = externalIds.Select(Identity.Create);
            return await DownloadAsync(ids, token).ConfigureAwait(false);
        }
        #endregion
    }
}

