﻿// Disable FxCop: <auto-genevrated />

using CogniteSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using CogniteSdk.Assets;
using CogniteSdk.DataPoints;
using CogniteSdk.TimeSeries;
using Com.Cognite.V1.Timeseries.Proto;

namespace csharp {

    class Program {

        static async Task CreateAssetsExample(Client client, string externalId, string name) {
            var asset = new AssetWriteDto
            {
                ExternalId = externalId,
                Name = name
            };
            var assets = new List<AssetWriteDto> {asset};

            var result = await client.Assets.CreateAsync(assets).ConfigureAwait(false);
            var newAsset = result.FirstOrDefault();

            Console.WriteLine(newAsset.Name);
        }

        static async Task UpdateAssetExample(Client client, string externalId, string newName, Dictionary<string, string> metaData) {
            var query = new List<AssetUpdateItem>() {
                new AssetUpdateItem(externalId) {
                    Update = new AssetUpdateDto {
                        Metadata = new DictUpdate<string>(metaData),
                        Name = new SetUpdate<string>(newName)
                    }
                }
            };

            var result = await client.Assets.UpdateAsync(query).ConfigureAwait(false);

            var updatedAsset = result.FirstOrDefault();
            Console.WriteLine(updatedAsset.Name);
        }

        static async Task<AssetReadDto> GetAssetsExample(Client client, string assetName) {
            var query = new AssetQueryDto
            {
                Filter = new AssetFilterDto { Name = assetName }
            };

            var result = await client.Assets.ListAsync(query).ConfigureAwait(false);

            var asset = result.Items.FirstOrDefault();
            Console.WriteLine(asset.ParentId);
            Console.WriteLine(result);
            return asset;
        }

        static async Task QueryTimeseriesDataExample(Client client) {
            var query = new DataPointsQuery() {
                Items = new List<DataPointsQueryItem> {
                    new DataPointsQueryItem {
                        Id = 42L,
                        Aggregates = new List<string> { "average" }
                    }
                }
            };

            var result = await client.DataPoints.ListAsync(query);
            var timeseries = result.Items.FirstOrDefault();
            var datapoints = timeseries.AggregateDatapoints.Datapoints.FirstOrDefault();

            Console.WriteLine(datapoints.Average);
            Console.WriteLine(result);
        }

        static async Task CreateTimeseriesDataExample(Client client, string timeseriesName, string timeseriesExternalId) {
            var timeseries = new TimeSeriesWriteDto {
                Name = timeseriesName
            };

            var result = await client.TimeSeries.CreateAsync(new List<TimeSeriesWriteDto> { timeseries });

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

        private static async Task Main(string[] args) {
            Console.WriteLine("C# Client");

            var apiKey = Environment.GetEnvironmentVariable("API_KEY");
            var project = Environment.GetEnvironmentVariable("PROJECT");

            using var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };

            using var httpClient = new HttpClient(handler);
            var builder = new Client.Builder();
            var client =
                builder
                    .SetAppId("playground")
                    .SetHttpClient(httpClient)
                    .SetApiKey(apiKey)
                    .SetProject(project)
                    .Build();

            var asset = await GetAssetsExample(client, "23-TE-96116-04").ConfigureAwait(false);
        }
    }
}
