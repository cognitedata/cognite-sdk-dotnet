using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

using Com.Cognite.V1.Timeseries.Proto;

using CogniteSdk;
using CogniteSdk.DataPoints;

using Assets = CogniteSdk.Assets;
using TimeSeries = CogniteSdk.TimeSeries;

namespace csharp
{
    class Program
    {
        /// <summary>
        /// Get Assets example.
        /// </summary>
        /// <param name="client">Fusion client.</param>
        /// <returns>Task</returns>
        static async Task GetAssetsExample(Client client)
        {
            var assetArgs = new List<Assets.List.Option> {
                Assets.List.Option.Name("string3")
            };
            var result = await client.Assets.ListAsync(assetArgs);

            var asset = result.Items.First();
            Console.WriteLine("{0}", asset.ParentId);
            Console.WriteLine("{0}", result);
        }

        static async Task QueryTimeseriesDataExample(Client client)
        {
            var aggregates = new List<Aggregated.Aggregate> { Aggregated.Aggregate.Average };
            var defaultOptions = new List<Aggregated.QueryOption> {
                Aggregated.QueryOption.Aggregates(aggregates)
            };
            var options = new List<Aggregated.Option> () {
                new Aggregated.Option () { Id = Identity.Id(42L), QueryOptions = new List<Aggregated.QueryOption> ()}
            };

            var result = await client.DataPoints.GetAggregatedMultipleAsync(options, defaultOptions);
            Console.WriteLine("{0}", result.Items.First().AggregateDatapoints.Datapoints.First().Average);
            Console.WriteLine("{0}", result);
        }

        static async Task CreateTimeseriesDataExample(Client client)
        {
            var timeseries = new TimeSeries.WritePoco {
                Name = "Testing"
            };

            var result = await client.TimeSeries.CreateAsync(new List<TimeSeries.WritePoco> { timeseries });

            Console.WriteLine("{0}", result);

            var dataPoints = new NumericDatapoints();
            dataPoints.Datapoints.Add(new NumericDatapoint { Timestamp = 0L, Value = 1.0 });

            var points = new DataPointInsertionRequest();
            points.Items.Add(new List<DataPointInsertionItem>
            {
                new DataPointInsertionItem
                {
                    ExternalId = "test",
                    NumericDatapoints = dataPoints
                }
            });
            await client.DataPoints.InsertAsync(points);
        }

        static async Task Main(string[] args)
        {
            Console.WriteLine("C# Client");

            var apiKey = Environment.GetEnvironmentVariable("API_KEY");
            var project = Environment.GetEnvironmentVariable("PROJECT");

            using (var httpClient = new HttpClient ()) {
                var client =
                    Client.Create()
                    .SetAppId("playground")
                    .SetHttpClient(httpClient)
                    .AddHeader("api-key", apiKey)
                    .SetProject(project);

                await GetAssetsExample(client);
            }
        }
    }
}
