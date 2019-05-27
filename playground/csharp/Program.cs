using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Cognite.Sdk;
using Cognite.Sdk.Api;
using Cognite.Sdk.Timeseries;


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
            var assetArgs =
                AssetArgs.Empty()
                .Name("string3");

            var result = await client.GetAssetsAsync(assetArgs);

            Console.WriteLine("{0}", result.Items.First().TryGetParentId(out long parentId));
            Console.WriteLine("{0}", parentId);
            Console.WriteLine("{0}", result);
        }

        static async Task QueryTimeseriesExample(Client client)
        {
            var query =
                Query.Create()
                .Aggregates(new List<Aggregate> { Aggregate.Average });

            //var result = await client.GetTimeseriesDataAsync("myseries", query);

            //Console.WriteLine("{0}", result.First().DataPoints.First().TryGetValue(out long value));
            //Console.WriteLine("{0}", value);
            //Console.WriteLine("{0}", result);
        }

        static async Task Main(string[] args)
        {
            Console.WriteLine("C# Client");

            var apiKey = Environment.GetEnvironmentVariable("API_KEY");
            var project = Environment.GetEnvironmentVariable("PROJECT");

            var client =
                Client.Create()
                .AddHeader("api-key", apiKey)
                .SetProject(project);

            await GetAssetsExample(client);
        }
    }
}
