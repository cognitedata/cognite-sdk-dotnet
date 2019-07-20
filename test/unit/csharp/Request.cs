using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Fusion;
using Fusion.Assets;
using Fusion.Api;
using System.Net.Http;
using System.Threading;
using System.Net;

namespace Tests
{
    /// <summary>
    /// Mock http message handler for http client.
    /// </summary>
    public class HttpMessageHandlerStub : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _sendAsync;

        public HttpMessageHandlerStub(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> sendAsync)
        {
            _sendAsync = sendAsync;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await _sendAsync(request, cancellationToken);
        }
    }
}