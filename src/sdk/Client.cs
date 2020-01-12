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
        internal HttpContext Ctx { get; private set; }

        /// <summary>
        /// Client Assets extension methods
        /// </summary>
        public Assets Assets { get; }

        // Client TimeSeries extension methods
        //public TimeSeries.ClientExtension TimeSeries { get; }
        // Client DataPoints extension methods
        //public DataPoints.ClientExtension DataPoints { get; }
        // Client Events extension methods
        //public Events.ClientExtension Events { get; }
        // Client Login extension methods
        //public Login.ClientExtension Login  { get; }
        // Client Files extension methods
        //public Files.ClientExtension Files { get; }
        // Client Raw extension methods
        //public Raw.ClientExtension Raw { get; }
        // Client Sequences extension methods
        //public Sequences.ClientExtension Sequences  { get; }

        /// <summary>
        /// Client for making requests to the API.
        /// </summary>
        /// /// <param name="context">Context to use for this session.</param>
        private Client(HttpContext context)
        {
            Ctx = setUrlBuilder(context);
        }

        /// <summary>
        /// Client for making requests to the API.
        /// </summary>
        /// <param name="httpClient">HTTP client to use for making the actual requests.</param>
        public Client(HttpClient httpClient)
        {
            Ctx = Context.setHttpClient(httpClient, Ctx);
        }

        /// <summary>
        /// Builder to build a client.
        /// </summary>
        public sealed class Builder
        {
            private HttpContext context = Context.defaultContext;

            /// <summary>
            /// Create builder using HTTP client
            /// </summary>
            /// <param name="httpClient">The HttpClient to use.</param>
            public Builder(HttpClient httpClient)
            {
                this.context = Context.setHttpClient(httpClient, context);
            }

            /// <summary>
            /// Create builder using default context.
            /// </summary>
            public Builder()
            {
            }

            /// <summary>
            /// Add header for accessing the API.
            /// </summary>
            /// <param name="name">Name of the header</param>
            /// <param name="value">Value of the header</param>
            public Builder AddHeader(string name, string value)
            {
                context = Context.addHeader(name, value, context);
                return this;
            }

            /// <summary>
            /// Add authentication API Key
            /// </summary>
            /// <param name="apiKey">API key</param>
            public Builder SetApiKey(string apiKey)
            {
                return this.AddHeader("api-key", apiKey);
            }

            /// <summary>
            /// Set project for accessing the API.
            /// </summary>
            /// <param name="project">Name of project.</param>
            public Builder SetProject(string project)
            {
                context = setProject(project, context);
                return this;
            }

            /// <summary>
            /// Set unique app identifier
            /// </summary>
            /// <param name="appId">ID for the app</param>
            public Builder SetAppId(string appId)
            {
                context = setAppId(appId, context);
                return this;
            }

            /// <summary>
            /// Set the HTTP client to use.
            /// </summary>
            /// <param name="client">The HttpClient to use</param>
            /// <returns></returns>
            public Builder SetHttpClient(HttpClient client)
            {
                context = Context.setHttpClient(client, context);
                return this;
            }

            /// <summary>
            /// Set the service URL to be used by the client.
            /// </summary>
            /// <param name="serviceUrl">The service URL to use.</param>
            /// <returns></returns>
            public Builder SetServiceUrl(string serviceUrl)
            {
                context = setServiceUrl(serviceUrl, context);
                return this;
            }

            /// <summary>
            /// Builds the new client
            /// </summary>
            /// <returns>New client.</returns>
            public Client Build()
            {
                // Check for optional fields etc here
                HttpContext ctx = context;
                context = null; // Builder is invalid after this
                return new Client(ctx);
            }
        }
    }
}
