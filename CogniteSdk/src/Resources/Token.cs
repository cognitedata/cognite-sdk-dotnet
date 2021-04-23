// Copyright 2021 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Threading;
using System.Threading.Tasks;
using CogniteSdk.Token;
using Oryx;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// For internal use. Contains token methods.
    /// </summary>
    public class TokenResource : Resource
    {
        /// <summary>
        /// Will only be instantiated by the client.
        /// </summary>
        /// <param name="authHandler">The authentication handler.</param>
        /// <param name="ctx">The HTTP context to use for the request.</param>
        public TokenResource(Func<CancellationToken, Task<string>> authHandler, HttpContext ctx) : base(authHandler, ctx)
        {
        }

        /// <summary>
        /// Inspect the OpenID-Connect/OAuth 2 token used to access CDF resources.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<TokenInspect> InspectAsync(CancellationToken token = default)
        {
            var req = Oryx.Cognite.Token.inspect();
            return await RunAsync(req, token).ConfigureAwait(false);
        }

    }
}