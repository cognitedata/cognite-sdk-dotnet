// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using static Oryx.Cognite.HandlerModule;
using HttpContext = Oryx.Context<System.Net.Http.HttpResponseMessage>;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// For internal use. Contains all event methods.
    /// </summary>
    public class FilesResource : Resource
    {
        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">The authetication handler.</param>
        /// <param name="ctx">Context to use for the request.</param>
        internal FilesResource(Func<CancellationToken, Task<string>> authHandler, HttpContext ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Retrieves list of files matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>List of assets matching given filters and optional cursor</returns>
        public async Task<ItemsWithCursor<File>> ListAsync(FileQuery query, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Files.list<ItemsWithCursor<File>>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves number of files matching query.
        /// </summary>
        /// <param name="query">The query filter to use.</param>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>Number of files matching given filters and optional cursor</returns>
        public async Task<Int32> AggregateAsync(FileQuery query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Oryx.Cognite.Files.aggregate<Int32>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves information about an file given a file id.
        /// </summary>
        /// <param name="fileId">The id of the file to get.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>File with the given id.</returns>
        public async Task<File> GetAsync(long fileId, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Files.get<File>(fileId);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        #region Retrieve overloads
        /// <summary>
        /// Retrieves metadata information about multiple specific files in the same project. Results are returned in
        /// the same order as in the request. This operation does not return the file contents.
        /// </summary>
        /// <param name="ids">The list of file identities to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<File>> RetrieveAsync(IEnumerable<Identity> ids, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Files.retrieve<IEnumerable<File>>(ids, ignoreUnknownIds);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves metadata information about multiple specific files in the same project. Results are returned in
        /// the same order as in the request. This operation does not return the file contents.
        /// </summary>
        /// <param name="internalIds">The list of file internal identities to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<File>> RetrieveAsync(IEnumerable<long> internalIds, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            var ids = internalIds.Select(Identity.Create);
            return await RetrieveAsync(ids, ignoreUnknownIds, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves metadata information about multiple specific files in the same project. Results are returned in
        /// the same order as in the request. This operation does not return the file contents.
        /// </summary>
        /// <param name="externalIds">The list of file internal identities to retrieve.</param>
        /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<File>> RetrieveAsync(IEnumerable<string> externalIds, bool? ignoreUnknownIds = null, CancellationToken token = default)
        {
            var ids = externalIds.Select(Identity.Create);
            return await RetrieveAsync(ids, ignoreUnknownIds, token).ConfigureAwait(false);
        }
        #endregion

        /// <summary>
        /// Retrieves a list of download URLs for the specified list of file IDs. After getting the download links, the
        /// client has to issue a GET request to the returned URLs, which will respond with the contents of the file.
        /// The link will expire after 30 seconds.
        /// </summary>
        /// <param name="ids">List of file IDs to retrieve the download URL for.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of file download URLs.</returns>
        #region Download overloads
        public async Task<IEnumerable<FileDownload>> DownloadAsync(IEnumerable<Identity> ids, CancellationToken token = default)
        {
            var req = Oryx.Cognite.Files.download<IEnumerable<FileDownload>>(ids);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves a list of download URLs for the specified list of file IDs. After getting the download links, the
        /// client has to issue a GET request to the returned URLs, which will respond with the contents of the file.
        /// The link will expire after 30 seconds.
        /// </summary>
        /// <param name="internalIds">List of file internal IDs to retrieve the download URL for.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of file download URLs.</returns>
        public async Task<IEnumerable<FileDownload>> DownloadAsync(IEnumerable<long> internalIds, CancellationToken token = default)
        {
            var ids = internalIds.Select(Identity.Create);
            return await DownloadAsync(ids, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves a list of download URLs for the specified list of file IDs. After getting the download links, the
        /// client has to issue a GET request to the returned URLs, which will respond with the contents of the file.
        /// The link will expire after 30 seconds.
        /// </summary>
        /// <param name="externalIds">List of file external IDs to retrieve the download URL for.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of file download URLs.</returns>

        public async Task<IEnumerable<FileDownload>> DownloadAsync(IEnumerable<string> externalIds, CancellationToken token = default)
        {
            var ids = externalIds.Select(Identity.Create);
            return await DownloadAsync(ids, token).ConfigureAwait(false);
        }
        #endregion

        /// <summary>
        /// Create metadata information and get upload link for one file. The uploadUrl link which is returned in the
        /// response is a Google Cloud Storage(GCS) resumable upload URL.It should be used in a separate request to
        /// upload the file, as documented in
        /// https://cloud.google.com/storage/docs/json_api/v1/how-tos/resumable-upload. The uploadUrl expires after one
        /// week.Any file info entry that do not have the actual file uploaded within one week will be automatically
        /// deleted. The 'Origin' header parameter is forwarded as a 'Origin' header to the GCS initiate upload session
        /// request. Also, the 'mimeType' query parameter is forwarded as a 'X-Upload-Content-Type' heade
        /// </summary>
        /// <param name="file">The file to upload.</param>
        /// <param name="overwrite">
        /// If 'overwrite' is set to true, and the POST body content specifies a 'externalId' field, fields for the file
        /// found for externalId can be overwritten. The default setting is false. If metadata is included in the
        /// request body, all of the original metadata will be overwritten.The actual file will be overwritten after a
        /// successful upload with the uploadUrl from the response.If there is no successful upload, the current file
        /// contents will be kept. File-Asset mappings only change if explicitly stated in the assetIds field of the
        /// POST json body. Do not set assetIds in request body if you want to keep the current file-asset mappings.
        /// </param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>The Updated file.</returns>
        public async Task<FileUploadRead> UploadAsync(FileCreate file, bool overwrite=false, CancellationToken token = default)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            var req = Oryx.Cognite.Files.upload<FileUploadRead>(file, overwrite);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates the information for the files specified in the request body.
        /// </summary>
        /// <param name="update">The update for the files.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>The updated files.</returns>
        public async Task<IEnumerable<File>> UpdateAsync(IEnumerable<FileUpdateItem> update, CancellationToken token = default)
        {
            if (update is null)
            {
                throw new ArgumentNullException(nameof(update));
            }

            var req = Oryx.Cognite.Files.update<IEnumerable<File>>(update);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        #region Delete overloads
        /// <summary>
        /// Delete multiple events in the same project.
        /// </summary>
        /// <param name="query">The list of events to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>Empty response.</returns>
        public async Task<EmptyResponse> DeleteAsync(FileDelete query, CancellationToken token = default)
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Oryx.Cognite.Files.delete<EmptyResponse>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple events in the same project.
        /// </summary>
        /// <param name="identities">The list of event ids to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(IEnumerable<Identity> identities, CancellationToken token = default)
        {
            if (identities is null)
            {
                throw new ArgumentNullException(nameof(identities));
            }

            var query = new FileDelete { Items = identities };
            return await DeleteAsync(query, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple events in the same project.
        /// </summary>
        /// <param name="ids">The list of event ids to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(IEnumerable<long> ids, CancellationToken token = default)
        {
            if (ids is null)
            {
                throw new ArgumentNullException(nameof(ids));
            }

            var query = new FileDelete { Items = ids.Select(Identity.Create) };
            return await DeleteAsync(query, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Delete multiple events in the same project.
        /// </summary>
        /// <param name="externalIds">The list of event externalids to delete.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<EmptyResponse> DeleteAsync(IEnumerable<string> externalIds, CancellationToken token = default)
        {
            if (externalIds is null)
            {
                throw new ArgumentNullException(nameof(externalIds));
            }

            var query = new FileDelete { Items = externalIds.Select(Identity.Create) };
            return await DeleteAsync(query, token).ConfigureAwait(false);
        }

        #endregion

        /// <summary>
        /// Retrieves a list of files matching the given criteria. This operation does not support pagination.
        /// </summary>
        /// <param name="query">Search query.</param>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns>List of assets matching given criteria.</returns>
        public async Task<IEnumerable<File>> SearchAsync (FileSearch query, CancellationToken token = default )
        {
            if (query is null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            var req = Oryx.Cognite.Files.search<IEnumerable<File>>(query);
            return await RunAsync(req, token).ConfigureAwait(false);
        }
    }
}

