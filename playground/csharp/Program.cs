﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

using Cognite.Sdk;
using Cognite.Sdk.Api;
using Cognite.Sdk.Timeseries;
using Cognite.Sdk.Assets;

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
            var assetArgs = new List<GetAssets.Option> {
                GetAssets.Option.Name("string3")
            };
            var result = await client.GetAssetsAsync(assetArgs);

            var asset = result.Items.First();
            Console.WriteLine("{0}", asset.ParentId);
            Console.WriteLine("{0}", result);
        }

        static async Task QueryTimeseriesDataExample(Client client)
        {
            var aggregates = new List<GetAggregatedDataPoints.Aggregate> { GetAggregatedDataPoints.Aggregate.Average };
            var defaultOptions = new List<GetAggregatedDataPoints.Option> {
                GetAggregatedDataPoints.Option.Aggregates(aggregates)
            };
            var options = new List<(long, IEnumerable<GetAggregatedDataPoints.Option>)> ();

            var result = await client.GetAggregatedDataPointsAsync(defaultOptions, options);
            Console.WriteLine("{0}", result.First().DataPoints.First().Average);
            Console.WriteLine("{0}", result);
        }

        static async Task CreateTimeseriesDataExample(Client client)
        {
            var timeseries = new TimeseriesWritePoco {
                    Name = "Testing"
            };

            var result = await client.CreateTimeseriesAsync(new List<TimeseriesWritePoco> { timeseries });

            Console.WriteLine("{0}", result);

            var points = new List<DataPointsWritePoco> {
                new DataPointsWritePoco {
                    Identity = Identity.ExternalId("test"),
                    DataPoints = new List<DataPointPoco> {
                        new DataPointPoco {
                            TimeStamp = 0L,
                            Value = Numeric.Float(1.0)
                        }
                    }
                }
            };
            var result2 = await client.InsertDataAsync(points);
        }

        static async Task Main(string[] args)
        {
            Console.WriteLine("C# Client");

            var apiKey = Environment.GetEnvironmentVariable("API_KEY");
            var project = Environment.GetEnvironmentVariable("PROJECT");

            using (var httpClient = new HttpClient ()) {
                var client =
                    Client.Create(httpClient)
                    .AddHeader("api-key", apiKey)
                    .SetProject(project);

                await GetAssetsExample(client);
            }
        }
    }
}
