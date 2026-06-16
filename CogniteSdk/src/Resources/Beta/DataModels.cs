// Copyright 2026 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CogniteSdk.DataModels;
using Microsoft.FSharp.Core;
using Oryx;

namespace CogniteSdk.Resources.Beta
{
    /// <summary>
    /// Beta data modeling operations
    /// </summary>
    public class DataModelsResource : Resource
    {
        /// <summary>
        /// Will only be instantiated by the client
        /// </summary>
        /// <param name="authHandler">Authentication handler.</param>
        /// <param name="ctx">The HTTP context to use for the request.</param>
        internal DataModelsResource(Func<CancellationToken, Task<string>> authHandler, FSharpFunc<IHttpNext<Unit>, Task<Unit>> ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Create or update a list of instances using the beta API
        /// </summary>
        /// <param name="request">Instance write request.</param>
        /// <param name="token">Optional cancellation token.</param>
        public async Task<IEnumerable<SlimInstance>> UpsertInstances(InstanceWriteRequest request, CancellationToken token = default)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var req = Oryx.Cognite.Beta.DataModels.upsertInstances(request, GetContext(token));
            return await RunAsync(req).ConfigureAwait(false);
        }
    }
}
