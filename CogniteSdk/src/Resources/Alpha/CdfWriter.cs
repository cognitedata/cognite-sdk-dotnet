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
    }
}
