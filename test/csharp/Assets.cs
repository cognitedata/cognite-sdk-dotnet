using Xunit;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Cognite.Sdk;
using Cognite.Sdk.Assets;
using Cognite.Sdk.Api;

namespace Tests
{
    public class AssetTests
    {
        //private Func<Context, string> fetcher;

        [Fact]
        public void TestGetAssets()
        {
            var apiKey = "api-key";
            var project = "project";

            var json = File.ReadAllText("Assets.json");
            var fetcher = Fetcher.FromJson(200, json);

            var client =
                Client.Create()
                .AddHeader("api-key", apiKey)
                .SetProject(project)
                .SetFetch(fetcher.Fetch);

            var assetArgs =
                AssetArgs.Empty()
                .Name("string3");

            Assert.ThrowsAsync<DecodeException>( async () => await client.GetAssets(assetArgs));
        }

        [Fact]
        public async Task TestGetAssetsServerUnavailable()
        {
            var apiKey = "api-key";
            var project = "project";

            var json = File.ReadAllText("Assets.json");
            var fetcher = Fetcher.FromJson(503, json);

            var client =
                Client.Create()
                .AddHeader("api-key", apiKey)
                .SetProject(project)
                .SetFetch(fetcher.Fetch);

            var assetArgs =
                AssetArgs.Empty()
                .Name("string3");

            var result = await client.GetAssets(assetArgs);
            Assert.Equal(HttpMethod.GET, fetcher.Ctx.Method);
            Assert.Equal("/assets", fetcher.Ctx.Resource);
            //Assert.Equal(fetcher.Ctx.Query, new List<string>());
            Assert.Single(result);
            Console.WriteLine(fetcher.Ctx.Body);
        }

        [Fact]
        public void TestGetInvalidAssetsThrowsException()
        {
            var apiKey = "api-key";
            var project = "project";

            var json = File.ReadAllText("InvalidAsset.json");
            var fetcher = Fetcher.FromJson(200, json);

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

        [Fact]
        public async Task TestGetInvaldAssetThrowsException()
        {
            var apiKey = "api-key";
            var project = "project";

            var json = File.ReadAllText("Assets.json");
            var fetcher = Fetcher.FromJson(200, json);

            var client =
                Client.Create()
                .AddHeader("api-key", apiKey)
                .SetProject(project)
                .SetFetch(fetcher.Fetch);

            var result = await client.GetAsset(42L);
            Assert.Equal(HttpMethod.GET, fetcher.Ctx.Method);
            Assert.Equal("/assets/42", fetcher.Ctx.Resource);
            //Assert.Equal(fetcher.Ctx.Query, new List<string>());
            //Assert.Equal(result.Count, 2);
            Console.WriteLine(fetcher.Ctx.Body);
        }

        [Fact]
        public void TestGetAsset()
        {
            var apiKey = "api-key";
            var project = "project";

            var json = File.ReadAllText("InvalidAsset.json");
            var fetcher = Fetcher.FromJson(200, json);

            var client =
                Client.Create()
                .AddHeader("api-key", apiKey)
                .SetProject(project)
                .SetFetch(fetcher.Fetch);

            Assert.ThrowsAsync<DecodeException>(async () => await client.GetAsset(42L));
        }

        [Fact]
        public async Task TestCreateAssets()
        {
            var apiKey = "api-key";
            var project = "project";

            var json = File.ReadAllText("Assets.json");
            var fetcher = Fetcher.FromJson(200, json);

            var client =
                Client.Create()
                .AddHeader("api-key", apiKey)
                .SetProject(project)
                .SetFetch(fetcher.Fetch);

            var assets = new List<AssetCreateDto> {
                Asset.Create("name1", "description1"),
                Asset.Create("name2", "description2"),
                Asset.Create("name3", "description3").SetParentId("parentId")
            };
            var result = await client.CreateAssets(assets);
        }
    }
}