// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Threading;
using System.Threading.Tasks;

using CogniteSdk.Login;
using static Oryx.Cognite.HandlerModule;
using HttpContext = Oryx.Context<System.Net.Http.HttpResponseMessage>;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// For internal use. Contains all event methods.
    /// </summary>
    public class LoginResource
    {
        private readonly HttpContext _ctx;

        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="ctx">Context to use for the request.</param>
        internal LoginResource(HttpContext ctx)
        {
            _ctx = ctx;
        }

        /// <summary>
        /// Returns the authentication information about the asking entity.
        /// </summary>
        /// <param name="token">Optional cancellation token to use.</param>
        /// <returns>The current authentication status of the request</returns>
        public async Task<LoginStatus> StatusAsync(CancellationToken token = default)
        {
            var req = Oryx.Cognite.Login.status<LoginStatus>();
            return await runUnsafeAsync(_ctx, token, req).ConfigureAwait(false);
        }
    }
}