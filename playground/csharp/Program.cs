using System;
using System.Threading.Tasks;

using Cognite.Sdk;
using Cognite.Sdk.Api;


namespace csharp
{
    class Program
    {
        /// <summary>
        /// Get Assets example.
        /// </summary>
        /// <param name="client">CDP client.</param>
        /// <returns>Task</returns>
        static async Task GetAssetsExample(Client client)
        {
            var assetArgs =
                AssetArgs.Empty()
                .Name("string3");

            var result = await client.GetAssets(assetArgs);

            Console.WriteLine("{0}", result[0].TryGetParentId(out long parentId));
            Console.WriteLine("{0}", parentId);
            Console.WriteLine("{0}", result);
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
