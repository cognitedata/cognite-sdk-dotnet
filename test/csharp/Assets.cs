using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Cognite.Sdk;
using Cognite.Sdk.Assets;
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

        public Context Ctx => _ctx;

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
            Assert.AreEqual(fetcher.Ctx.Method, HttpMethod.GET, "Should be equal");
            Assert.AreEqual(fetcher.Ctx.Resource, "/assets", "Should be equal");
            //Assert.AreEqual(fetcher.Ctx.Query, new List<string>(), "Should be equal");
            Assert.AreEqual(result.Count, 1);
            Console.WriteLine(fetcher.Ctx.Body);
        }

        [Test]
        public void TestGetInvalidAssetsThrowsException()
        {
            var apiKey = "api-key";
            var project = "project";

            var response = File.ReadAllText("InvalidAsset.json");
            var fetcher = new Fetcher(response);

            var client =
                Client.Create()
                .AddHeader("api-key", apiKey)
                .SetProject(project)
                .SetFetch(fetcher.Fetch);

            var assetArgs =
                AssetArgs.Empty()
                .Name("string3")
                .Description("my description")
                .Depth(3);

            Assert.ThrowsAsync<DecodeException>( async () => await client.GetAssets(assetArgs));
        }

        [Test]
        public async Task TestGetInvaldAssetThrowsException()
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

            var result = await client.GetAsset(42L);
            Assert.AreEqual(fetcher.Ctx.Method, HttpMethod.GET, "Should be equal");
            Assert.AreEqual(fetcher.Ctx.Resource, "/assets/42", "Should be equal");
            //Assert.AreEqual(fetcher.Ctx.Query, new List<string>(), "Should be equal");
            //Assert.AreEqual(result.Count, 2);
            Console.WriteLine(fetcher.Ctx.Body);
        }

        [Test]
        public void TestGetAsset()
        {
            var apiKey = "api-key";
            var project = "project";

            var response = File.ReadAllText("InvalidAsset.json");
            var fetcher = new Fetcher(response);

            var client =
                Client.Create()
                .AddHeader("api-key", apiKey)
                .SetProject(project)
                .SetFetch(fetcher.Fetch);

            Assert.ThrowsAsync<DecodeException>(async () => await client.GetAsset(42L));
        }

        [Test]
        public async Task TestCreateAssets()
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

            var assets = new List<AssetCreateDto> {
                Asset.Create("name1", "description1"),
                Asset.Create("name2", "description2"),
                Asset.Create("name3", "description3")
            };
            var result = await client.CreateAssets(assets);
        }
    }
}