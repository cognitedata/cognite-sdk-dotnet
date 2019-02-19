using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

using Cognite.Sdk;
using Cognite.Sdk.Api;


namespace csharp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("C# Client");

            var apiKey = Environment.GetEnvironmentVariable("API_KEY");
            var project = Environment.GetEnvironmentVariable("PROJECT");

            var client =
                Client.Create()
                .AddHeader("api-key", apiKey)
                .SetProject(project);

            var assetArgs =
                AssetArgs.Create()
                .Name("string3");

            var result = await client.GetAssets(assetArgs);

            Console.WriteLine("{0}", result);
        }
    }
}
