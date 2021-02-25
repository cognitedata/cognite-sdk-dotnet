// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Oryx;
using CogniteSdk.Resources;

using static Oryx.Cognite.ContextModule;

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
        /// Client Relationships extension methods
        /// </summary>
        public RelationshipResource Relationships { get; }

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
        /// Client beta extension methods
        /// </summary>
        public BetaResource Beta { get; }

        /// <summary>
        /// Client for making requests to the API.
        /// </summary>
        /// <param name="authHandler">The authentication handler.</param>
        /// <param name="ctx">Context to use for this session.</param>
        private Client(Func<CancellationToken, Task<string>> authHandler, Context ctx)
        {
            // Setup resources.
            Assets = new AssetsResource(authHandler, ctx);
            TimeSeries = new TimeSeriesResource(authHandler, ctx);
            DataPoints = new DataPointsResource(authHandler, ctx);
            Events = new EventsResource(authHandler, ctx);
            Sequences = new SequencesResource(authHandler, ctx);
            Raw = new RawResource(authHandler, ctx);
            Relationships = new RelationshipResource(authHandler, ctx);
            ThreeDModels = new ThreeDModelsResource(authHandler, ctx);
            ThreeDRevisions = new ThreeDRevisionsResource(authHandler, ctx);
            ThreeDAssetMappings = new ThreeDAssetMappingsResource(authHandler, ctx);
            Files = new FilesResource(authHandler, ctx);
            Login = new LoginResource(authHandler, ctx);

            // Playground features (experimental)
            Playground = new PlaygroundResource(authHandler, ctx);
            // Beta features (experimental)
            Beta = new BetaResource(authHandler, ctx);
        }

        /// <summary>
        /// Builder to build a client.
        /// </summary>
        [SuppressMessage("Design", "CA1034:Nested types should not be visible", Justification = "Builder pattern.")]
        public sealed class Builder
        {
            private Context? _context = create();
            private Func<CancellationToken, Task<string>> _authHandler;

            /// <summary>
            /// Create Client builder.
            /// </summary>
            /// <param name="httpClient">Optional HttpClient to use for HTTP requests.</param>
            public Builder(HttpClient httpClient = null)
            {
                _context = httpClient == null ? _context : ContextModule.withHttpClient(httpClient, _context.Value);
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

                _context = ContextModule.withHeader(name, value, _context.Value);
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

                _context = withProject(project, _context.Value);
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

                _context = withAppId(appId, _context.Value);
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

                _context = ContextModule.withHttpClient(client, _context.Value);
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

                _context = withBaseUrl(baseUrl, _context.Value);
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

                _context = ContextModule.withLogger(logger, _context.Value);
                return this;
            }

            /// <summary>
            /// Set the log level to be used by the SDK.
            /// </summary>
            /// <param name="logLevel">The log level to use.</param>
            /// <returns>Updated builder.</returns>
            public Builder SetLogLevel(LogLevel logLevel)
            {
                _context = ContextModule.withLogLevel(logLevel, _context.Value);
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

                _context = ContextModule.withLogFormat(format, _context.Value);
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

                _context = ContextModule.withMetrics(metrics, _context.Value);
                return this;
            }

            /// <summary>
            /// Builds the new client. Builder is invalid after this.
            /// </summary>
            /// <returns>New client.</returns>
            public Client Build()
            {
                // Check for optional fields etc here
                Context ctx = _context.Value;
                Func<CancellationToken, Task<string>> authHandler = _authHandler;

                // Builder is invalid after this
                _context = null;
                _authHandler = null;
                return new Client(authHandler, ctx);
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
