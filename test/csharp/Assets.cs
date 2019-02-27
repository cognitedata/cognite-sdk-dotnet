using NUnit.Framework;

using System;
using System.IO;
using System.Threading.Tasks;

using Cognite.Sdk;
using Cognite.Sdk.Api;

namespace Tests
{
    public class Fetcher
    {
        private readonly string _response;
        private Context _ctx = null;

        public Fetcher(string response)
        {
            this._response = response;
        }

        public Context Ctx {
            get { return _ctx; }
        }

        public Task<string> Fetch(Context ctx)
        {
            _ctx = ctx;
            return Task.FromResult(_response);
        }
    }

    public class AssetTests
    {
        //private Func<Context, string> fetcher;

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public async Task TestGetAssets()
        {
            var apiKey = "api-key";
            var project = "project";

            var response = File.ReadAllText("Assets.json");
            var fetcher = new Fetcher(response);

            var client =
                Client.Create()
                .AddHeader("api-key", apiKey)
                .SetProject(project)
                .SetFetch(fetcher.Fetch);

            var assetArgs =
                AssetArgs.Empty()
                .Name("string3");

            var result = await client.GetAssets(assetArgs);
            Assert.AreEqual(fetcher.Ctx.Method, Method.Get, "Should be equal");
            //Expect.equal fetcher.Ctx.Value.Resource (Resource "/assets/42") "Should be equal"
            //Expect.equal fetcher.Ctx.Value.Query [] "Should be equal"
            Assert.AreEqual(result.Count, 1);
        }
    }
}