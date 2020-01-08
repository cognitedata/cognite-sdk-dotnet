// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Net.Http;
using Oryx;

namespace CogniteSdk
{
    public class Client
    {
        internal HttpContext Ctx { get; }

        /// Client Assets extension methods
        public Assets.ClientExtension Assets { get; }

        /// Client TimeSeries extension methods
        public TimeSeries.ClientExtension TimeSeries { get; }
        /// Client DataPoints extension methods
        public DataPoints.ClientExtension DataPoints { get; }
        /// Client Events extension methods
        public Events.ClientExtension Events { get; }
        /// Client Login extension methods
        public Login.ClientExtension Login  { get; }
        /// Client Files extension methods
        public Files.ClientExtension Files { get; }
        /// Client Raw extension methods
        public Raw.ClientExtension Raw { get; }
        /// Client Sequences extension methods
        public Sequences.ClientExtension Sequences  { get; }

        /// <summary>
        /// Client for making requests to the API.
        /// </summary>
        /// /// <param name="context">Context to use for this session.</param>
        private Client(HttpContext context)
        {
            Ctx = context;
        }

        /// <summary>
        /// Client for making requests to the API.
        /// </summary>
        /// <param name="client">HTTP client to use for making the actual requests.</param>
        public Client(HttpClient client)
        {
            var ctx = Context.create();
            ctx = Context.setUrlBuilder(Context.urlBuilder, this.Ctx);
            ctx = Context.setHttpClient(httpClient, ctx);
            return new Client(ctx);
        }

        /// <summary>
        /// Creates a Client for accessing the API.
        /// </summary>
        public static Create()
        {
            var ctx = Context.create();
            return new Client(ctx);
        }

        /// <summary>
        /// Add header for accessing the API.
        /// </summary>
        /// <param name="name">Name of the header</param>
        /// <param name="value">Value of the header</param>
        public AddHeader (String name, String value)
        {
            var ctx = Context.addHeader(name, value, this.Ctx);
            return new Client(ctx);
        }

        /// <summary>
        /// Add authentication API Key
        /// </summary>
        /// <param name="apiKey">API key</param>
        public Client SetApiKey(String apiKey)
        {
            return this.AddHeader("api-key", apiKey);
        }

        /// <summary>
        /// Set project for accessing the API.
        /// </summary>
        /// <param name="project">Name of project.</param>
        public SetProject(String project)
        {
            var ctx = Context.setProject(project, this.Ctx);
            return new Client(ctx);
        }

        /// <summary>
        /// Set unique app identifier
        /// </summary>
        /// <param name="appId">ID for the app</param>
        public SetAppId(String appId)
        {
            var ctx = Context.setAppId(appId, this.Ctx);
            return new Client(ctx);
        }

        public SetHttpClient(HttpClient client)
        {
            var ctx = Context.setHttpClient(client, this.Ctx);
            return new Client(ctx);
        }

        public SetServiceUrl(String serviceUrl)
        {
            var ctx = Context.setServiceUrl(serviceUrl, this.Ctx);
            return new Client(ctx);
        }
    }
}
