// Copyright 2026 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Text.Json;
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
        /// Configure the CDF writer for an integration, replacing any existing mapping
        /// for the given integration. This should be called when the extractor starts.
        /// </summary>
        /// <param name="request">Mapping to configure</param>
        /// <param name="token">Optional cancellation token</param>
        public async Task SetupAsync(CdfWriterSetupRequest request, CancellationToken token = default)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));
            var req = CdfWriter.setup(request, GetContext(token));
            await RunAsync(req).ConfigureAwait(false);
        }

        /// <summary>
        /// Send a batch of items for a registered mapping task to the CDF writer, which applies the
        /// mapping and forwards the result to CDF. The response from CDF is returned as-is.
        /// </summary>
        /// <typeparam name="T">Type of the items payload</typeparam>
        /// <param name="request">Items to send and the task they belong to</param>
        /// <param name="token">Optional cancellation token</param>
        /// <returns>The CDF response, passed through unchanged by the CDF writer</returns>
        public async Task<JsonElement> PayloadAsync<T>(CdfWriterPayloadRequest<T> request, CancellationToken token = default)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));
            var req = CdfWriter.payload(request, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }
    }
}
