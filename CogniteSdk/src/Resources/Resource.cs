// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Threading;
using System.Threading.Tasks;


using Oryx.Cognite;
using static Oryx.Cognite.HandlerModule;
using HttpContext = Oryx.Context<System.Net.Http.HttpResponseMessage>;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// For internal use. Contains all asset methods.
    /// </summary>
    public abstract class Resource
    {
        /// <summary>
        /// The context.
        /// </summary>
        protected readonly HttpContext _ctx;
        /// <summary>
        /// The authentication handler.
        /// </summary>
        protected readonly Func<CancellationToken, Task<string>> _authHandler;

        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">Authentication handler.</param>
        /// <param name="ctx">Context to use for the request.</param>
        internal Resource(Func<CancellationToken, Task<string>> authHandler, HttpContext ctx)
        {
            _ctx = ctx;
            _authHandler = authHandler;
        }

        /// <summary>
        /// Helper method for running an Oryx handler in the client context with authentication handling.
        /// </summary>
        /// <param name="handler">The handler to run.</param>
        /// <param name="token">The cancellation token to use.</param>
        /// <typeparam name="T">The type of the response.</typeparam>
        /// <returns>Result.</returns>
        protected async Task<T> RunAsync<T>(Microsoft.FSharp.Core.FSharpFunc<Microsoft.FSharp.Core.FSharpFunc<Oryx.Context<T>, Task<Microsoft.FSharp.Core.FSharpResult<Oryx.Context<T>, Oryx.HandlerError<ResponseException>>>>, Microsoft.FSharp.Core.FSharpFunc<HttpContext, Task<Microsoft.FSharp.Core.FSharpResult<Oryx.Context<T>, Oryx.HandlerError<ResponseException>>>>> handler, CancellationToken token)
        {
            var req = _authHandler is null ? handler : withTokenRenewer(_authHandler, handler);
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }
    }
}