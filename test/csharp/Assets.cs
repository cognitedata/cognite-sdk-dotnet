using Xunit;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

using Cognite.Sdk;
using Cognite.Sdk.Assets;
using Cognite.Sdk.Api;

using Newtonsoft.Json;

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
            var metadata = ("metadata", JsonConvert.SerializeObject(new Dictionary<string, string> { { "option1", "value1" } }));
            var query = new List<(string, string)> {
                    metadata,
                    ("id", "42"),
                    ("depth", "5"),
                    ("path", "test"),
                    ("desc", "my description"),
                    ("name", "string3")
                };

            var client =
                Client.Create()
                .AddHeader("api-key", apiKey)
                .SetProject(project)
                .SetFetch(fetcher.Fetch);

            var assetArgs =
                AssetArgs.Empty()
                .Name("string3")
                .Description("my description")
                .Path("test")
                .Depth(5)
                .Id(42L)
                .MetaData(new Dictionary<string, string> {{ "option1", "value1"}});

            // Act
            var result = await client.GetAssetsAsync(assetArgs);

            // Assert
            Assert.Equal(HttpMethod.GET, fetcher.Ctx.Request.Method);
            Assert.Equal("/assets", fetcher.Ctx.Request.Resource);
            var expectedQuery = new List<Tuple<string, string>>(fetcher.Ctx.Request.Query);
            Assert.Equal(expectedQuery, query.Select(x => x.ToTuple ()).ToList());
            Assert.Single(result.Items);
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
            await Assert.ThrowsAsync<ResponseException>(() => client.GetAssetsAsync(assetArgs));
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
            await Assert.ThrowsAsync<DecodeException>(() => client.GetAssetsAsync(assetArgs));
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
            var result = await client.GetAssetAsync(42L);

            // Assert
            Assert.Equal(HttpMethod.GET, fetcher.Ctx.Request.Method);
            Assert.Equal("/assets/42", fetcher.Ctx.Request.Resource);
            Assert.Equal(fetcher.Ctx.Request.Query, new List<Tuple<string, string>>());
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
            await Assert.ThrowsAsync<DecodeException>(() => client.GetAssetAsync(42L));
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
                Asset.Create("name3", "description3")
                    .SetParentId("parentId")
                    .SetParentName("parent")
                    .SetSource("source")
                    .SetSourceId("sourceId")
                    .SetParentRefId("parentRefId")
                    .SetRefId("refId")
                    .SetMetaData(new Dictionary<string, string> {{ "data1", "value" }})
            };

            // Act
            var result = await client.CreateAssetsAsync (assets);

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task TestUpdateAssets()
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

            var assets = new List<AssetUpdate> {
                new AssetUpdate(42L)
            };

            // Act
            var result = await client.UpdateAssetsAsync (assets);

            // Assert
            Assert.Equal(200, result.Code);
        }
    }
}