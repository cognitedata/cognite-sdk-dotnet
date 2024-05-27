﻿// Disable FxCop: <auto-generated />

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;

using Com.Cognite.V1.Timeseries.Proto;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;

using CogniteSdk;

namespace csharp
{

    class Program
    {

        static async Task CreateAssetsExample(Client client, string externalId, string name)
        {
            var asset = new AssetCreate
            {
                ExternalId = externalId,
                Name = name
            };
            var assets = new List<AssetCreate> { asset };

            var result = await client.Assets.CreateAsync(assets).ConfigureAwait(false);
            var newAsset = result.FirstOrDefault();

            Console.WriteLine(newAsset.Name);
        }

        static async Task UpdateAssetExample(Client client, string externalId, string newName, Dictionary<string, string> metaData)
        {
            var query = new List<AssetUpdateItem>() {
                new AssetUpdateItem(externalId) {
                    Update = new AssetUpdate {
                        Metadata = new UpdateDictionary<string>(metaData),
                        Name = new Update<string>(newName)
                    }
                }
            };

            var result = await client.Assets.UpdateAsync(query).ConfigureAwait(false);

            var updatedAsset = result.FirstOrDefault();
            Console.WriteLine(updatedAsset.Name);
        }

        static async Task<Asset> GetAssetsExample(Client client, string assetName)
        {
            var query = new AssetQuery
            {
                Filter = new AssetFilter { Name = assetName }
            };

            var result = await client.Assets.ListAsync(query).ConfigureAwait(false);

            var asset = result.Items.FirstOrDefault();
            return asset;
        }

        static async Task<AggregateDatapoint> QueryTimeseriesDataExample(Client client)
        {
            var query = new DataPointsQuery()
            {
                Items = new List<DataPointsQueryItem> {
                    new DataPointsQueryItem {
                        Id = 592785165400753L,
                        Aggregates = new List<string> { "average" },
                        Granularity="1d"
                    }
                }
            };

            var result = await client.DataPoints.ListAsync(query);
            var timeseries = result.Items.FirstOrDefault();
            var datapoints = timeseries.AggregateDatapoints.Datapoints.FirstOrDefault();

            return datapoints;
        }

        static async Task CreateTimeseriesDataExample(Client client, string timeseriesName, string timeseriesExternalId)
        {
            var timeseries = new TimeSeriesCreate
            {
                Name = timeseriesName
            };

            var result = await client.TimeSeries.CreateAsync(new List<TimeSeriesCreate> { timeseries });

            Console.WriteLine(result);

            var dataPoints = new NumericDatapoints();
            dataPoints.Datapoints.Add(new NumericDatapoint { Timestamp = 0L, Value = 1.0 });

            var points = new DataPointInsertionRequest();
            points.Items.Add(new List<DataPointInsertionItem> {
                new DataPointInsertionItem
                {
                    ExternalId = timeseriesExternalId,
                    NumericDatapoints = dataPoints
                }
            });
            await client.DataPoints.CreateAsync(points);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(configure => configure.AddConsole().SetMinimumLevel(
                Microsoft.Extensions.Logging.LogLevel.Debug
            ));
        }

        static async Task GzipPerformanceTest(Client client)
        {
            try
            {
                var ts = await client.TimeSeries.CreateAsync(
                    Enumerable.Range(0, 100).Select(idx => new TimeSeriesCreate
                    {
                        Name = $"gzip-ts-test-{idx}",
                        ExternalId = $"gzip-ts-test-{idx}"
                    }).ToList());
            }
            catch (ResponseException rex) when (rex.Duplicated?.Any() ?? false) { }

            var chunks = new[]
            {
                (1, 100), (10, 100), (100, 100), (1, 1000), (10, 1000), (100, 1000), (1, 10000), (10, 10000)
            };


            Console.WriteLine("Gzip: ");

            long start = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            foreach (var pair in chunks)
            {
                var data = new DataPointInsertionRequest();
                for (int i = 0; i < pair.Item1; i++)
                {
                    var req = new NumericDatapoints();
                    for (int j = 0; j < pair.Item2; j++)
                    {
                        req.Datapoints.Add(new NumericDatapoint
                        {
                            Timestamp = start + i * pair.Item2 + j,
                            Value = i * pair.Item2 + j
                        });
                    }
                    data.Items.Add(new DataPointInsertionItem
                    {
                        ExternalId = $"gzip-ts-test-{i}",
                        NumericDatapoints = req
                    });
                }

                var sw = new Stopwatch();
                sw.Start();
                await client.DataPoints.CreateAsync(data, System.IO.Compression.CompressionLevel.Fastest);
                sw.Stop();
                Console.WriteLine($"Inserting {pair.Item2} datapoints for {pair.Item1} timeseries took {sw.ElapsedMilliseconds} ms");
            }

            Console.Write("Non-gzip:");

            foreach (var pair in chunks)
            {
                var data = new DataPointInsertionRequest();
                for (int i = 0; i < pair.Item1; i++)
                {
                    var req = new NumericDatapoints();
                    for (int j = 0; j < pair.Item2; j++)
                    {
                        req.Datapoints.Add(new NumericDatapoint
                        {
                            Timestamp = start + i * pair.Item2 + j,
                            Value = i * pair.Item2 + j
                        });
                    }
                    data.Items.Add(new DataPointInsertionItem
                    {
                        ExternalId = $"gzip-ts-test-{i}",
                        NumericDatapoints = req
                    });
                }

                var sw = new Stopwatch();
                sw.Start();
                await client.DataPoints.CreateAsync(data);
                sw.Stop();
                Console.WriteLine($"Inserting {pair.Item2} datapoints for {pair.Item1} timeseries took {sw.ElapsedMilliseconds} ms");
            }

            await client.TimeSeries.DeleteAsync(new TimeSeriesDelete
            {
                IgnoreUnknownIds = true,
                Items = Enumerable.Range(0, 100).Select(idx => Identity.Create($"gzip-ts-test-{idx}"))
            });
        }

        private static async Task Main()
        {
            Console.WriteLine("C# Client");

            var tenantId = Environment.GetEnvironmentVariable("TENANT_ID");
            var clientId = Environment.GetEnvironmentVariable("CLIENT_ID");
            var clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET");
            var cluster = Environment.GetEnvironmentVariable("CDF_CLUSTER");
            var project = Environment.GetEnvironmentVariable("CDF_PROJECT");

            var scopes = new List<string> { $"https://{cluster}.cognitedata.com/.default" };

            var app = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithAuthority(AzureCloudInstance.AzurePublic, tenantId)
                .WithClientSecret(clientSecret)
                .Build();

            var result = await app.AcquireTokenForClient(scopes).ExecuteAsync();
            var accessToken = result.AccessToken;

            using var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<Program>();

            using var httpClient = new HttpClient(handler);
            var builder = new Client.Builder();
            var client =
                builder
                    .SetAppId("playground")
                    .SetHttpClient(httpClient)
                    .AddHeader("Authorization", $"Bearer {accessToken}")
                    .SetProject(project)
                    .SetBaseUrl(new Uri($"https://{cluster}.cognitedata.com"))
                    .SetLogger(logger)
                    .SetLogLevel(Microsoft.Extensions.Logging.LogLevel.Debug)
                    .Build();

            // var asset = await GetAssetsExample(client, "23-TE-96116-04").ConfigureAwait(false);
            // Console.WriteLine($"{asset}");
            // var data = await QueryTimeseriesDataExample(client);
            await GzipPerformanceTest(client);
        }
    }
}
