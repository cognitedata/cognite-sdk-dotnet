// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Oryx;
using CogniteSdk.Resources;

using HttpContext = Oryx.Context<System.Net.Http.HttpResponseMessage>;
using static Oryx.Cognite.ContextModule;
using System.Threading;

namespace CogniteSdk
{
    /// <summary>
    /// Metrics interface.
    /// </summary>
    public interface IMetrics : Oryx.IMetrics { }

    /// <summary>
    /// Cognite SDK client.
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
        public SequencesResource Sequences  { get; }

        /// <summary>
        /// Client 3D Models extension methods
        /// </summary>
        public ThreeDModelsResource ThreeDModels { get; set; }

        /// <summary>
        /// Client 3D Revisions extension methods
        /// </summary>
        public ThreeDRevisionsResource ThreeDRevisions { get; set; }

        /// <summary>
        /// Client 3D Asset Mappings extension methods
        /// </summary>
        public ThreeDAssetMappingsResource ThreeDAssetMappings { get; set; }

        /// <summary>
        /// Client playground extension methods
        /// </summary>
        public PlaygroundResource Playground  { get; }

        /// <summary>
        /// Client for making requests to the API.
        /// </summary>
        /// <param name="authHandler">The authentication handler.</param>
        /// <param name="includeMetadata">Include meta-data in responses or not.</param>
        /// <param name="ctx">Context to use for this session.</param>
        private Client(Func<CancellationToken, Task<string>> authHandler, bool includeMetadata, HttpContext ctx)
        {
            // Setup resources.
            Assets = new AssetsResource(authHandler, includeMetadata, ctx);
            TimeSeries = new TimeSeriesResource(authHandler, includeMetadata, ctx);
            DataPoints = new DataPointsResource(authHandler, ctx);
            Events = new EventsResource(authHandler, includeMetadata, ctx);
            Sequences = new SequencesResource(authHandler, ctx);
            Raw = new RawResource(authHandler, ctx);
            ThreeDModels = new ThreeDModelsResource(authHandler, ctx);
            ThreeDRevisions = new ThreeDRevisionsResource(authHandler, ctx);
            ThreeDAssetMappings = new ThreeDAssetMappingsResource(authHandler, ctx);
            Files = new FilesResource(authHandler, ctx);
            Login = new LoginResource(authHandler, ctx);

            // Playground features (experimental)
            Playground = new PlaygroundResource(authHandler, ctx);
        }

        /// <summary>
        /// Builder to build a client.
        /// </summary>
        [SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "Builder pattern.")]
        public sealed class Builder
        {
            private HttpContext _context = create();
            private Func<CancellationToken, Task<string>> _authHandler;
            private bool _includeMetadata = true;

            /// <summary>
            /// Create Client builder.
            /// </summary>
            /// <param name="httpClient">Optional HttpClient to use for HTTP requests.</param>
            public Builder(HttpClient httpClient = null)
            {
                _context = httpClient == null ? _context : Context.withHttpClient(httpClient, _context);
            }

            /// <summary>
            /// Add header for accessing the API.
            /// </summary>
            /// <param name="name">Name of the header</param>
            /// <param name="value">Value of the header</param>
            /// <returns>Updated builder.</returns>
            public Builder AddHeader(string name, string value)
            {
                if (name is null)
                {
                    throw new ArgumentNullException(nameof(name));
                }

                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _context = Context.withHeader(name, value, _context);
                return this;
            }

            /// <summary>
            /// Add authentication API Key
            /// </summary>
            /// <param name="apiKey">API key</param>
            /// <returns>Updated builder.</returns>
            public Builder SetApiKey(string apiKey)
            {
                if (apiKey is null)
                {
                    throw new ArgumentNullException(nameof(apiKey));
                }

                return AddHeader("api-key", apiKey);
            }

            /// <summary>
            /// Set authentication handler.
            /// </summary>
            /// <param name="tokenProvider">Token provider for getting bearer token to use for the request.</param>
            /// <returns>Updated builder.</returns>
            public Builder SetTokenProvider(Func<CancellationToken, Task<string>> tokenProvider)
            {
                if (tokenProvider is null)
                {
                    throw new ArgumentNullException(nameof(tokenProvider));
                }

                _authHandler = tokenProvider;
                return this;
            }

            /// <summary>
            /// Set project for accessing the API.
            /// </summary>
            /// <param name="project">Name of project.</param>
            /// <returns>Updated builder.</returns>
            public Builder SetProject(string project)
            {
                if (project is null)
                {
                    throw new ArgumentNullException(nameof(project));
                }

                _context = withProject(project, _context);
                return this;
            }

            /// <summary>
            /// Set unique app identifier
            /// </summary>
            /// <param name="appId">ID for the app</param>
            /// <returns>Updated builder.</returns>
            public Builder SetAppId(string appId)
            {
                if (appId is null)
                {
                    throw new ArgumentNullException(nameof(appId));
                }

                _context = withAppId(appId, _context);
                return this;
            }

            /// <summary>
            /// Set the HTTP client to use.
            /// </summary>
            /// <param name="client">The HttpClient to use</param>
            /// <returns>Updated builder.</returns>
            public Builder SetHttpClient(HttpClient client)
            {
                if (client is null)
                {
                    throw new ArgumentNullException(nameof(client));
                }

                _context = Context.withHttpClient(client, _context);
                return this;
            }

            /// <summary>
            /// Set the base URL to be used by the client.
            /// </summary>
            /// <param name="baseUrl">The base URL to use.</param>
            /// <returns>Updated builder.</returns>
            public Builder SetBaseUrl(Uri baseUrl)
            {
                if (baseUrl is null)
                {
                    throw new ArgumentNullException(nameof(baseUrl));
                }

                _context = withBaseUrl(baseUrl, _context);
                return this;
            }

            /// <summary>
            /// Set the logger to be used by the SDK.
            /// </summary>
            /// <param name="logger">The logger to use.</param>
            /// <returns>Updated builder.</returns>
            public Builder SetLogger(ILogger logger)
            {
                if (logger is null)
                {
                    throw new ArgumentNullException(nameof(logger));
                }

                _context = Context.withLogger(logger, _context);
                return this;
            }

            /// <summary>
            /// Set the log level to be used by the SDK.
            /// </summary>
            /// <param name="logLevel">The log level to use.</param>
            /// <returns>Updated builder.</returns>
            public Builder SetLogLevel(LogLevel logLevel)
            {
                _context = Context.withLogLevel(logLevel, _context);
                return this;
            }

            /// <summary>
            /// Set the log format string to be used by the SDK.
            /// </summary>
            /// <param name="format">The log level to use.</param>
            /// <returns>Updated builder.</returns>
            public Builder SetLogFormat(string format)
            {
                if (format is null)
                {
                    throw new ArgumentNullException(nameof(format));
                }

                _context = Context.withLogFormat(format, _context);
                return this;
            }

            /// <summary>
            /// Set the metrics handler to be used by the SDK.
            /// </summary>
            /// <param name="metrics">The metrics handler to use.</param>
            /// <returns>Updated builder.</returns>
            public Builder SetMetrics(IMetrics metrics)
            {
                if (metrics is null)
                {
                    throw new ArgumentNullException(nameof(metrics));
                }

                _context = Context.withMetrics(metrics, _context);
                return this;
            }

            /// <summary>
            /// Include or exclude metadata decoding.
            /// </summary>
            /// <param name="includeMetadata">True if metadata should be included. Set to false to disable meta-data decoding.</param>
            /// <returns>Updated builder.</returns>
            public Builder IncludeMetadata(bool includeMetadata)
            {
                _includeMetadata = includeMetadata;
                return this;
            }

            /// <summary>
            /// Builds the new client. Builder is invalid after this.
            /// </summary>
            /// <returns>New client.</returns>
            public Client Build()
            {
                // Check for optional fields etc here
                HttpContext ctx = _context;
                Func<CancellationToken, Task<string>> authHandler = _authHandler;
                bool includeMetadata = _includeMetadata;

                // Builder is invalid after this
                _context = null;
                _authHandler = null;
                _includeMetadata = false;
                return new Client(authHandler, includeMetadata, ctx);
            }

            /// <summary>
            /// Create new Client builder.
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
