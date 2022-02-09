// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.FSharp.Core;
using Oryx;
using Oryx.Pipeline;

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
        private readonly FSharpFunc<IAsyncNext<HttpContext, Unit>, Task<Unit>> _ctx;
        /// <summary>
        /// The authentication handler.
        /// </summary>
        protected readonly Func<CancellationToken, Task<string>> _authHandler;

        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">Authentication handler.</param>
        /// <param name="ctx">The HTTP context to use for the request.</param>
        internal Resource(Func<CancellationToken, Task<string>> authHandler, FSharpFunc<IAsyncNext<HttpContext, Unit>, Task<Unit>> ctx)
        {
            _ctx = ctx;
            _authHandler = authHandler;
        }

        /// <summary>
        /// Get initial HTTP handler with context.
        /// </summary>
        /// <param name="token">The cancellation token to use.</param>
        /// <returns>HTTP handler with context</returns>
        internal FSharpFunc<IAsyncNext<HttpContext, Unit>, Task<Unit>> GetContext(CancellationToken token)
        {
            var ctx = _authHandler is null ? _ctx : withTokenRenewer(_authHandler, _ctx);
            return HttpHandler.withCancellationToken(token, ctx);
        }

        /// <summary>
        /// Helper method for running an Oryx handler in the client context with authentication handling.
        /// </summary>
        /// <param name="handler">The handler to run.</param>
        /// <typeparam name="T">The type of the response.</typeparam>
        /// <returns>Result.</returns>
        protected async Task<T> RunAsync<T>(FSharpFunc<IAsyncNext<HttpContext, T>, Task<Unit>> handler)
        {
            return await HttpHandler.runUnsafeAsync(handler).ConfigureAwait(false);
        }
    }
}