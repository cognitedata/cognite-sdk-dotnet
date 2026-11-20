// Copyright 2026 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Threading;
using System.Threading.Tasks;
using CogniteSdk.Alpha;
using Microsoft.FSharp.Core;
using Oryx;
using Oryx.Cognite.Alpha;

namespace CogniteSdk.Resources.Alpha
{
    /// <summary>
    /// Resource for the CDF writer API.
    /// </summary>
    public class CdfWriterResource : Resource
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CdfWriterResource(Func<CancellationToken, Task<string>> authHandler, FSharpFunc<IHttpNext<Unit>, Task<Unit>> ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Register the tasks, emitted views, merge mode, mappings and bindings for an integration,
        /// replacing its live CDF writer config. This should be called when the extractor starts.
        /// </summary>
        /// <param name="request">Config to register</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>The integration and the config revision that was stored</returns>
        public async Task<CdfWriterSetupResponse> SetupAsync(CdfWriterSetupRequest request, CancellationToken token = default)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));
            var req = CdfWriter.setup(request, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Send items to the CDF writer, grouped by task, emitted resource and operation. The CDF writer applies
        /// the mapping bound to each (task, resource) and forwards the result to CDF, making one CDF call per
        /// (task, resource, operation). If any item fails validation, nothing is forwarded and the request fails
        /// with status 422. If some CDF calls fail, the request still succeeds: check the status of each operation in
        /// the response.
        /// </summary>
        /// <param name="request">Items to send</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>The result of each CDF call, including the unchanged CDF response body</returns>
        public async Task<CdfWriterPayloadResponse> PayloadAsync(CdfWriterPayloadRequest request, CancellationToken token = default)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));
            var req = CdfWriter.payload(request, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }
    }
}
