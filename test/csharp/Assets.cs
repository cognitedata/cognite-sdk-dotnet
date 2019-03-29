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
        [Fact]
        public async Task TestGetAssets()
        {
            // Arrenge
            var apiKey = "api-key";
            var project = "project";

            var json = File.ReadAllText("Assets.json");
            var fetcher = Fetcher.FromJson(200, json);
            var query = new List<Tuple<string, string>> { ("name", "string3").ToTuple () };

            var client =
                Client.Create()
                .AddHeader("api-key", apiKey)
                .SetProject(project)
                .SetFetch(fetcher.Fetch);

            var assetArgs =
                AssetArgs.Empty()
                .Name("string3");

            // Act
            var result = await client.GetAssets(assetArgs);

            // Assert
            Assert.Equal(HttpMethod.GET, fetcher.Ctx.Method);
            Assert.Equal("/assets", fetcher.Ctx.Resource);
            Assert.Equal(new List<Tuple<string, string>>(fetcher.Ctx.Query), query);
            Assert.Single(result);
        }

        [Fact]
        public async Task TestGetAssetsServerUnavailable()
        {
            // Arrange
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

            // Act/Assert
            await Assert.ThrowsAsync<ResponseException>(() => client.GetAssets(assetArgs));
        }

        [Fact]
        public async Task TestGetInvalidAssetsThrowsException()
        {
            // Arrange
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

            // Act/Assert
            await Assert.ThrowsAsync<DecodeException>(() => client.GetAssets(assetArgs));
        }

        [Fact]
        public async Task TestGetAsset()
        {
            // Arrange
            var apiKey = "api-key";
            var project = "project";

            var json = File.ReadAllText("Assets.json");
            var fetcher = Fetcher.FromJson(200, json);

            var client =
                Client.Create()
                .AddHeader("api-key", apiKey)
                .SetProject(project)
                .SetFetch(fetcher.Fetch);

            // Act
            var result = await client.GetAsset(42L);

            // Assert
            Assert.Equal(HttpMethod.GET, fetcher.Ctx.Method);
            Assert.Equal("/assets/42", fetcher.Ctx.Resource);
            Assert.Equal(fetcher.Ctx.Query, new List<Tuple<string, string>>());
            Console.WriteLine(fetcher.Ctx.Body);
        }

        [Fact]
        public async Task TestGetInvaldAssetThrowsException()
        {
            // Arrange
            var apiKey = "api-key";
            var project = "project";

            var json = File.ReadAllText("InvalidAsset.json");
            var fetcher = Fetcher.FromJson(200, json);

            var client =
                Client.Create()
                .AddHeader("api-key", apiKey)
                .SetProject(project)
                .SetFetch(fetcher.Fetch);

            // Act/Assert
            await Assert.ThrowsAsync<DecodeException>(() => client.GetAsset(42L));
        }

        [Fact]
        public async Task TestCreateAssets()
        {
            // Arrange
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

            // Act
            var result = await client.CreateAssets(assets);

            // Assert
            Assert.Single(result);
        }
    }
}