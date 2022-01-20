// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.FSharp.Core;
using Oryx;
using Oryx.Cognite;
using static Oryx.Cognite.HttpHandlerModule;

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
        protected readonly  FSharpFunc<FSharpFunc<HttpContext,FSharpFunc<Unit,Task<Unit>>>,Task<Unit>> _ctx;
        /// <summary>
        /// The authentication handler.
        /// </summary>
        protected readonly Func<CancellationToken, Task<string>> _authHandler;

        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">Authentication handler.</param>
        /// <param name="ctx">The HTTP context to use for the request.</param>
        internal Resource(Func<CancellationToken, Task<string>> authHandler, FSharpFunc<FSharpFunc<HttpContext,FSharpFunc<Unit,Task<Unit>>>,Task<Unit>> ctx)
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
        protected async Task<T> RunAsync<T>(FSharpFunc<FSharpFunc<HttpContext,FSharpFunc<T,Task<Unit>>>,Task<Unit>> handler, CancellationToken token)
        {
            var req = _authHandler is null ? handler : withTokenRenewer(_authHandler, handler);
            var req_ = HttpHandler.withCancellationToken(token, req);
            return await HttpHandler.runUnsafeAsync(req_).ConfigureAwait(false);
        }
    }
}