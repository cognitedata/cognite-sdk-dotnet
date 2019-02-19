using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

using Cognite.Sdk.Api;


namespace csharp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var apiKey = Environment.GetEnvironmentVariable("API_KEY");
            var project = Environment.GetEnvironmentVariable("PROJECT");

            var ctx = Context.Create()
                .AddHeader("api-key", apiKey)
                .SetProject(project);

            var client = new Client();
            var result = await client.GetAssets(ctx, new List<int> {});
            Console.WriteLine("{0}", result);
        }
    }
}
