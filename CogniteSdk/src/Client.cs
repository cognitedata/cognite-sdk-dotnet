﻿// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

using Oryx;
using CogniteSdk.Resources;

using HttpContext = Oryx.Context<System.Net.Http.HttpResponseMessage>;
using static Oryx.Cognite.ContextModule;

namespace CogniteSdk
{
    /// <summary>
    /// The Cognite SDK client. This is the client you use to handle the different resources, i.e Assets, Events,
    /// TimeSeries. To create a new client you need to use the <see cref="Client.Builder" />.
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Client Assets extension methods
        /// </summary>
        public AssetsResource Assets { get; }

        /// <summary>
        /// Client TimeSeries extension methods.
        /// </summary>
        public TimeSeriesResource TimeSeries { get; }

        /// <summary>
        /// Client DataPoints extension methods
        /// </summary>
        public DataPointsResource DataPoints { get; }

        /// <summary>
        /// Client Events extension methods.
        /// </summary>
        public EventsResource Events { get; }

        /// <summary>
        /// Client Login extension methods
        /// </summary>
        public LoginResource Login { get; }

        /// <summary>
        /// Client Files extension methods
        /// </summary>
        public FilesResource Files { get; }

        /// <summary>
        /// Client Raw extension methods
        /// </summary>
        public RawResource Raw { get; }

        /// <summary>
        /// Client Sequences extension methods
        /// </summary>
        /// <value></value>
        public SequencesResource Sequences  { get; }

        /// <summary>
        /// Client for making requests to the API.
        /// </summary>
        /// <param name="context">Context to use for this session.</param>
        private Client(HttpContext context)
        {
            var ctx = setUrlBuilder(context);

            // Setup resources.
            Assets = new AssetsResource(ctx);
            TimeSeries = new TimeSeriesResource(ctx);
            DataPoints =new DataPointsResource(ctx);
            Events = new EventsResource(ctx);
            Sequences = new SequencesResource(ctx);
            Raw = new RawResource(ctx);
            Files = new FilesResource(ctx);
            Login = new LoginResource(ctx);
        }

        /// <summary>
        /// The client builder. This is the builder you need to use in order to build a new <see cref="Client" />..
        /// </summary>
        [SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "Builder pattern.")]
        public sealed class Builder
        {
            private HttpContext _context = Context.defaultContext;

            /// <summary>
            /// Create new Client builder.
            /// </summary>
            /// <param name="httpClient">Optional HttpClient to use for HTTP requests.</param>
            public Builder(HttpClient httpClient = null)
            {
                _context = httpClient == null ? _context : Context.setHttpClient(httpClient, _context);
            }

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
            public Builder SetServiceUrl(Uri serviceUrl)
            {
                if (serviceUrl is null)
                {
                    throw new ArgumentNullException(nameof(serviceUrl));
                }

                _context = setServiceUrl(serviceUrl.ToString(), _context);
                return this;
            }

            /// <summary>
            /// Builds the new client. Finishes the build and returns a new
            /// <see cref="Client" />. Note that the builder will not be valid after the client has been built.
            /// </summary>
            /// <returns>New <see cref="Client" />.</returns>
            public Client Build()
            {
                // Check for optional fields etc here
                HttpContext ctx = _context;
                _context = null; // Builder is invalid after this
                return new Client(ctx);
            }

            /// <summary>
            /// Create new Client builder. This static create method takes an optional <see cref="HttpClient" /> which
            /// may help if dependency injection (DI) scenarios.
            /// </summary>
            /// <param name="httpClient">Optional httpClient</param>
            /// <returns>New client builder.</returns>
            public static Builder Create(HttpClient httpClient = null)
            {
                return new Builder(httpClient);
            }
        }
    }
}
