using CogniteSdk;
using CogniteSdk.Assets;
using CogniteSdk.DataPoints;
using CogniteSdk.TimeSeries;
using Com.Cognite.V1.Timeseries.Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace csharp {

    class Program {

        static async Task CreateAssetsExample(Client client, string externalId, string name) {
            var asset = new AssetEntity();
            asset.ExternalId = externalId;
            asset.Name = name;

            var result = await client.Assets.CreateAsync(new List<AssetEntity>() { asset });
            var newAsset = result.FirstOrDefault();

            Console.WriteLine(newAsset.Name);
        }

        static async Task UpdateAssetExample(Client client, string externalId, string newName, Dictionary<string, string> metaData) {
            var fieldsToUpdate = new List<AssetUpdate>() {
                AssetUpdate.ChangeMetaData(metaData, null),
                AssetUpdate.SetName(newName)
            };

            var result = await client.Assets.UpdateAsync(new List<(Identity, IEnumerable<AssetUpdate>)>() {
                (Identity.ExternalId(externalId), fieldsToUpdate)
            });

            var updatedAsset = result.FirstOrDefault();
            Console.WriteLine(updatedAsset.Name);
        }

        static async Task<AssetEntity> GetAssetsExample(Client client, string assetName) {
            var filter = new List<AssetFilter> {
                AssetFilter.Name(assetName)
            };
            var result = await client.Assets.ListAsync(filter);

            var asset = result.Items.FirstOrDefault();
            Console.WriteLine(asset.ParentId);
            Console.WriteLine(result);
            return asset;
        }

        static async Task QueryTimeseriesDataExample(Client client) {
            var aggregates = new List<Aggregate> { Aggregate.Average };
            var defaultOptions = new List<AggregateQuery> {
                AggregateQuery.Aggregates(aggregates)
            };
            var query = new List<AggregateMultipleQuery>() {
                new AggregateMultipleQuery () { Id = Identity.Id(42L), AggregateQuery = new List<AggregateQuery> () }
            };

            var result = await client.DataPoints.GetAggregatedMultipleAsync(query, defaultOptions);
            var timeseries = result.Items.FirstOrDefault();
            var datapoints = timeseries.AggregateDatapoints.Datapoints.FirstOrDefault();

            Console.WriteLine(datapoints.Average);
            Console.WriteLine(result);
        }

        static async Task CreateTimeseriesDataExample(Client client, string timeseriesName, string timeseriesExternalId) {
            var timeseries = new TimeSeriesEntity {
                Name = timeseriesName
            };

            var result = await client.TimeSeries.CreateAsync(new List<TimeSeriesEntity> { timeseries });

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
            await client.DataPoints.InsertAsync(points);
        }

        static async Task Main(string[] args) {
            Console.WriteLine("C# Client");

            var apiKey = Environment.GetEnvironmentVariable("API_KEY");
            var project = Environment.GetEnvironmentVariable("PROJECT");

            using (var httpClient = new HttpClient()) {
                var client =
                    Client.Create()
                    .SetAppId("playground")
                    .SetHttpClient(httpClient)
                    .SetApiKey(apiKey)
                    .SetProject(project);

                var asset = await GetAssetsExample(client, "23-TE-96116-04");
            }
        }
    }
}
