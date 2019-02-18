using System;
using System.IO;

using Cognite.Sdk.Api;


namespace csharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var apiKey = Environment.GetEnvironmentVariable("API_KEY");
            var project = Environment.GetEnvironmentVariable("PROJECT");

            var ctx = new Context();
            ctx.AddHeader(("api-key", apiKey));

        //|> addHeader ("api-key", Uri.EscapeDataString config.ApiKey)
        //|> setProject (Uri.EscapeDataString config.Project)
        }
    }
}
