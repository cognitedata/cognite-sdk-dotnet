// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Net.Http;
using Oryx;

using HttpContext = Oryx.Context<System.Net.Http.HttpResponseMessage>;

using static Oryx.Cognite.ContextModule;

namespace CogniteSdk
{
    /// <summary>
    /// Cognite SDK client.
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Client Assets extension methods
        /// </summary>
        public Resources.Assets Assets { get; }

        /// <summary>
        /// Client TimeSeries extension methods.
        /// </summary>
        public Resources.TimeSeries TimeSeries { get; }

        // Client DataPoints extension methods
        //public DataPoints.ClientExtension DataPoints { get; }

        /// Client Events extension methods
        public Resources.Events Events { get; }

        // Client Login extension methods
        //public Login.ClientExtension Login  { get; }
        // Client Files extension methods
        //public Files.ClientExtension Files { get; }

        /// <summary>
        /// Client Raw extension methods
        /// </summary>
        public Resources.Raw Raw { get; }

        // Client Sequences extension methods
        //public Sequences.ClientExtension Sequences  { get; }

        /// <summary>
        /// Client for making requests to the API.
        /// </summary>
        /// <param name="context">Context to use for this session.</param>
        private Client(HttpContext context)
        {
            var ctx = setUrlBuilder(context);

            // Setup extension methods.
            Assets = new Resources.Assets(ctx);
            TimeSeries = new Resources.TimeSeries(ctx);
            Events = new Resources.Events(ctx);
            Raw = new Resources.Raw(ctx);
        }

        /// <summary>
        /// Builder to build a client.
        /// </summary>
        public sealed class Builder
        {
            private HttpContext _context = Context.defaultContext;

            /// <summary>
            /// Create builder using HTTP client
            /// </summary>
            /// <param name="httpClient">The HttpClient to use.</param>
            public Builder(HttpClient httpClient)
            {
                _context = Context.setHttpClient(httpClient, _context);
            }

            /// <summary>
            /// Create builder using default context.
            /// </summary>
            public Builder() {}

            /// <summary>
            /// Add header for accessing the API.
            /// </summary>
            /// <param name="name">Name of the header</param>
            /// <param name="value">Value of the header</param>
            /// <returns>Updated builder.</returns>
            public Builder AddHeader(string name, string value)
            {
                _context = Context.addHeader(name, value, _context);
                return this;
            }

            /// <summary>
            /// Add authentication API Key
            /// </summary>
            /// <param name="apiKey">API key</param>
            /// <returns>Updated builder.</returns>
            public Builder SetApiKey(string apiKey)
            {
                return AddHeader("api-key", apiKey);
            }

            /// <summary>
            /// Set project for accessing the API.
            /// </summary>
            /// <param name="project">Name of project.</param>
            /// <returns>Updated builder.</returns>
            public Builder SetProject(string project)
            {
                _context = setProject(project, _context);
                return this;
            }

            /// <summary>
            /// Set unique app identifier
            /// </summary>
            /// <param name="appId">ID for the app</param>
            /// <returns>Updated builder.</returns>
            public Builder SetAppId(string appId)
            {
                _context = setAppId(appId, _context);
                return this;
            }

            /// <summary>
            /// Set the HTTP client to use.
            /// </summary>
            /// <param name="client">The HttpClient to use</param>
            /// <returns>Updated builder.</returns>
            public Builder SetHttpClient(HttpClient client)
            {
                _context = Context.setHttpClient(client, _context);
                return this;
            }

            /// <summary>
            /// Set the service URL to be used by the client.
            /// </summary>
            /// <param name="serviceUrl">The service URL to use.</param>
            /// <returns>Updated builder.</returns>
            public Builder SetServiceUrl(string serviceUrl)
            {
                _context = setServiceUrl(serviceUrl, _context);
                return this;
            }

            /// <summary>
            /// Builds the new client
            /// </summary>
            /// <returns>New client.</returns>
            public Client Build()
            {
                // Check for optional fields etc here
                HttpContext ctx = _context;
                _context = null; // Builder is invalid after this
                return new Client(ctx);
            }
        }
    }
}
