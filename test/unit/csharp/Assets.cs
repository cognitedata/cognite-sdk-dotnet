using Xunit;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Fusion;
using Fusion.Assets;
using Fusion.Api;
using System.Linq;
using Thoth.Json.Net;
using System.Threading;

namespace Tests
{
    public class AssetTests
    {
        [Fact]
        public async Task TestGetAssets()
        {
            // Arrenge
            HttpRequestMessage request = null;
            var apiKey = "api-key";
            var project = "project";

            var json = File.ReadAllText("Assets.json");
            var httpClient = new HttpClient(new HttpMessageHandlerStub(async (req, cancellationToken) =>
            {
                request = req;

                var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json)
                };

                return await Task.FromResult(responseMessage);
            }));

            var query = new List<(string, string)> {
                    ("parentIds", "[42,43]"),
                    ("source", "source"),
                    ("root", "false"),
                    ("externalIdPrefix", "prefix"),
                    ("name", "string3")
                };

            var client =
                Client.Create()
                .SetAppId("test")
                .SetHttpClient(httpClient)
                .AddHeader("api-key", apiKey)
                .SetProject(project);

            var options = new List<GetAssets.Option> {
                GetAssets.Option.Name("string3"),
                GetAssets.Option.ExternalIdPrefix("prefix"),
                GetAssets.Option.Root(false),
                GetAssets.Option.Source("source"),
                GetAssets.Option.ParentIds(new List<long> {42L, 43L })
                //.MetaData(new Dictionary<string, string> {{ "option1", "value1"}});
            };

            // Act
            var result = await client.GetAssetsAsync(options);

            // Assert
            Assert.Single(result.Items);
            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.Equal("api.cognitedata.com", request.RequestUri.Host);
            Assert.Equal("?name=string3&externalIdPrefix=prefix&root=false&source=source&parentIds=%5b42%2c43%5d", request.RequestUri.Query);
            Assert.Equal("/api/v1/projects/project/assets", request.RequestUri.AbsolutePath);
        }

        [Fact]
        public async Task TestGetAssetsServerUnavailable()
        {
            // Arrenge
            HttpRequestMessage request;
            var apiKey = "api-key";
            var project = "project";

            var json = File.ReadAllText("Assets.json");
            var httpClient = new HttpClient(new HttpMessageHandlerStub(async (req, cancellationToken) =>
            {
                request = req;

                var responseMessage = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                {
                    Content = new StringContent(json)
                };

                return await Task.FromResult(responseMessage);
            }));

            var client =
                Client.Create()
                .SetAppId("test")
                .SetHttpClient(httpClient)
                .AddHeader("api-key", apiKey)
                .SetProject(project);

            var options = new List<GetAssets.Option> {
                GetAssets.Option.Name("string3")
            };

            // Act/Assert
            await Assert.ThrowsAsync<ResponseException>(() => client.GetAssetsAsync(options));
        }

        [Fact]
        public async Task TestGetInvalidAssetsThrowsException()
        {
            // Arrange
            HttpRequestMessage request;
            var apiKey = "api-key";
            var project = "project";

            var json = File.ReadAllText("InvalidAsset.json");
            var httpClient = new HttpClient(new HttpMessageHandlerStub(async (req, cancellationToken) =>
            {
                request = req;

                var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json)
                };

                return await Task.FromResult(responseMessage);
            }));

            var client =
                Client.Create()
                .SetAppId("test")
                .SetHttpClient(httpClient)
                .AddHeader("api-key", apiKey)
                .SetProject(project);

            var assetArgs = new List<GetAssets.Option> {
                GetAssets.Option.Name("string3"),
                GetAssets.Option.Source("source"),
                GetAssets.Option.Root(true)
            };

            // Act/Assert
            await Assert.ThrowsAsync<ResponseException>(() => client.GetAssetsAsync(assetArgs));
        }

        [Fact]
        public async Task TestGetAsset()
        {
            // Arrange
            HttpRequestMessage request = null;
            var apiKey = "api-key";
            var project = "project";

            var json = File.ReadAllText("Asset.json");
            var httpClient = new HttpClient(new HttpMessageHandlerStub(async (req, cancellationToken) =>
            {
                request = req;

                var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json)
                };

                return await Task.FromResult(responseMessage);
            }));

            var client =
                Client.Create()
                .SetAppId("test")
                .SetHttpClient(httpClient)
                .AddHeader("api-key", apiKey)
                .SetProject(project);

            // Act
            var result = await client.GetAssetAsync(42L);

            // Assert
            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.Equal("api.cognitedata.com", request.RequestUri.Host);
            Assert.Equal("", request.RequestUri.Query);
            Assert.Equal("/api/v1/projects/project/assets/42", request.RequestUri.AbsolutePath);

        }

        [Fact]
        public async Task TestGetInvaldAssetThrowsException()
        {
            // Arrange
            HttpRequestMessage request = null;
            var apiKey = "api-key";
            var project = "project";
            var json = File.ReadAllText("InvalidAsset.json");

            var httpClient = new HttpClient(new HttpMessageHandlerStub(async (req, cancellationToken) =>
            {
                request = req;

                var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json)
                };

                return await Task.FromResult(responseMessage);
            }));

            var client =
                Client.Create()
                .SetAppId("test")
                .SetHttpClient(httpClient)
                .AddHeader("api-key", apiKey)
                .SetProject(project);

            // Act/Assert
            await Assert.ThrowsAsync<ResponseException>(() => client.GetAssetAsync(42L));
        }

        [Fact]
        public async Task TestCreateAssets()
        {
            // Arrange
            HttpRequestMessage request = null;
            var apiKey = "api-key";
            var project = "project";
            var json = File.ReadAllText("Assets.json");

            var httpClient = new HttpClient(new HttpMessageHandlerStub(async (req, cancellationToken) =>
            {
                request = req;

                var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json)
                };

                return await Task.FromResult(responseMessage);
            }));

            var client =
                Client.Create()
                .SetAppId("test")
                .SetHttpClient(httpClient)
                .AddHeader("api-key", apiKey)
                .SetProject(project);

            var assets = new List<AssetWritePoco> {
                new AssetWritePoco () { Name = "name1", Description = "description1" },
                new AssetWritePoco () { Name = "name2", Description = "description2" },
                new AssetWritePoco () {
                    Name = "name3",
                    Description = "description3",
                    Source = "source",
                    ParentId = 42L,
                    ParentExternalId = "parentExtenralId",
                    ExternalId = "uuid",
                    MetaData = new Dictionary<string, string> { {"data1", "value"} }
                }
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
            HttpRequestMessage request = null;
            var apiKey = "api-key";
            var project = "project";
            var json = File.ReadAllText("Assets.json");

            var httpClient = new HttpClient(new HttpMessageHandlerStub(async (req, cancellationToken) =>
            {
                request = req;

                var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json)
                };

                return await Task.FromResult(responseMessage);
            }));

            var client =
                Client.Create()
                .SetAppId("test")
                .SetHttpClient(httpClient)
                .AddHeader("api-key", apiKey)
                .SetProject(project);

            var options = new List<UpdateAssets.Option> {
                UpdateAssets.Option.SetName("test")
            };
            var assets = new List<ValueTuple<Identity, IEnumerable<UpdateAssets.Option>>> {
                (Identity.Id(42L), options)
            };

            // Act
            await client.UpdateAssetsAsync(assets);
        }

        [Fact]
        public async Task TestGetByIdsMissingAssets()
        {
            // Arrange
            HttpRequestMessage request = null;
            var apiKey = "api-key";
            var project = "project";
            var json = File.ReadAllText("MissingAsset.json");

            var httpClient = new HttpClient(new HttpMessageHandlerStub(async (req, cancellationToken) =>
            {
                request = req;

                var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(json)
                };

                return await Task.FromResult(responseMessage);
            }));

            var client =
                Client.Create()
                .SetAppId("test")
                .SetHttpClient(httpClient)
                .AddHeader("api-key", apiKey)
                .SetProject(project);

            var assets = new List<Identity> {
                Identity.Id(42L)
            };

            // Act
            try {
                var result = await client.GetAssetsByIdsAsync(assets);
            } catch (ResponseException ex) {
                Assert.True(ex.Code == 400);

                var error = ex.Missing.First()["id"];

                Assert.True(error is IntegerValue);
                var value = error as IntegerValue;

                Assert.True(value.Integer == 4234);
            }
        }
        [Fact]
        public async Task TestSearchAssets()
        {
            HttpRequestMessage request = null;
            var apiKey = "api-key";
            var project = "project";
            var json = File.ReadAllText("Assets.json");

            var httpClient = new HttpClient(new HttpMessageHandlerStub(async (req, cancellationToken) =>
            {
                request = req;

                var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json)
                };

                return await Task.FromResult(responseMessage);
            }));

            var client =
                Client.Create()
                .SetAppId("test")
                .SetHttpClient(httpClient)
                .AddHeader("api-key", apiKey)
                .SetProject(project);

            var options = new List<SearchAssets.Option> {
                SearchAssets.Option.Name("str")
            };
            var filters = new List<AssetFilter> {
                AssetFilter.CreatedTime(new TimeRange(DateTime.Now.Subtract(TimeSpan.FromHours(1)), DateTime.Now.Subtract(TimeSpan.FromHours(1)))),
                AssetFilter.Name("string")
            };

            var result = await client.SearchAssetsAsync(100, options, filters);

            Assert.NotEmpty(result);
        }
        [Fact]
        public async Task TestFilterAssets()
        {
            HttpRequestMessage request = null;
            var apiKey = "api-key";
            var project = "project";
            var json = File.ReadAllText("Assets.json");

            var httpClient = new HttpClient(new HttpMessageHandlerStub(async (req, cancellationToken) =>
            {
                request = req;

                var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json)
                };

                return await Task.FromResult(responseMessage);
            }));

            var client =
                Client.Create()
                .SetAppId("test")
                .SetHttpClient(httpClient)
                .AddHeader("api-key", apiKey)
                .SetProject(project);

            var options = new List<FilterAssets.Option> {
                FilterAssets.Option.Limit(100)
            };
            var filters = new List<AssetFilter> {
                AssetFilter.CreatedTime(new TimeRange(DateTime.Now.Subtract(TimeSpan.FromHours(1)), DateTime.Now.Subtract(TimeSpan.FromHours(1)))),
                AssetFilter.Name("string")
            };

            var result = await client.FilterAssetsAsync(options, filters);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Items);
        }
        [Fact]
        public async Task TestStreamContent()
        {
            string requestJson = null;
            var apiKey = "api-key";
            var project = "project";
            var respJson = File.ReadAllText("Assets.json");

            var httpClient = new HttpClient(new HttpMessageHandlerStub(async (req, cancellationToken) =>
            {
                requestJson = await req.Content.ReadAsStringAsync();

                var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(respJson)
                };

                return await Task.FromResult(responseMessage);
            }));

            var client =
                Client.Create()
                .SetAppId("test")
                .SetHttpClient(httpClient)
                .AddHeader("api-key", apiKey)
                .SetProject(project);

            var options = new List<FilterAssets.Option> {
                FilterAssets.Option.Limit(100),
                FilterAssets.Option.Cursor("cursor")
            };

            var filters = new List<AssetFilter> {
                AssetFilter.MetaData(new Dictionary<string, string> { { "key1", "val1" }, { "key2", "val2" } }),
                AssetFilter.Name("Name"),
                AssetFilter.ParentIds(new List<long> { 1234, 12345 })
            };

            var result = await client.FilterAssetsAsync(options, filters);
            var refRequest = Encode.toString(0, new FilterAssets.FilterAssetsRequest(filters, options).Encoder);
            Assert.Equal(refRequest, requestJson);
        }
        [Fact]
        public async Task TestStreamLargeContent()
        {
            string requestJson = null;
            var apiKey = "api-key";
            var project = "project";
            var respJson = File.ReadAllText("Assets.json");

            var httpClient = new HttpClient(new HttpMessageHandlerStub(async (req, cancellationToken) =>
            {
                requestJson = await req.Content.ReadAsStringAsync();

                var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(respJson)
                };

                return await Task.FromResult(responseMessage);
            }));

            var client =
                Client.Create()
                .SetAppId("test")
                .SetHttpClient(httpClient)
                .AddHeader("api-key", apiKey)
                .SetProject(project);

            var createAssets = new List<AssetWritePoco>();
            for (int i = 0; i < 1000; i++) // 1000 is the maximum number of assets per request
            {
                createAssets.Add(new AssetWritePoco
                {
                    Description = "Long description which takes a lot of memory to store " + i,
                    ExternalId = "ExternalIdsCanAlsoBeQuiteLongSometimes" + i,
                    Name = "Names are also fairly unrestricted " + i,
                    ParentExternalId = "With256CharactersForExternalIdsTheLimitCanGetQuiteHigh"
                });
            }

            var result = await client.CreateAssetsAsync(createAssets);
            var refRequest = Encode.toString(0, new CreateAssets.AssetsCreateRequest(createAssets.Select(AssetWriteDto.FromPoco)).Encoder);
            Assert.Equal(refRequest, requestJson);
        }
        [Fact]
        public async Task TestCancellationToken()
        {
            HttpRequestMessage request = null;
            var apiKey = "api-key";
            var project = "project";
            var json = File.ReadAllText("Assets.json");

            var httpClient = new HttpClient(new HttpMessageHandlerStub(async (req, cancellationToken) =>
            {
                request = req;

                var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(json)
                };

                return await Task.FromResult(responseMessage);
            }));

            var client =
                Client.Create()
                .SetHttpClient(httpClient)
                .AddHeader("api-key", apiKey)
                .SetProject(project);

            var options = new List<FilterAssets.Option> {
                FilterAssets.Option.Limit(100)
            };
            var filters = new List<AssetFilter> {
                AssetFilter.CreatedTime(new TimeRange(DateTime.Now.Subtract(TimeSpan.FromHours(1)), DateTime.Now.Subtract(TimeSpan.FromHours(1)))),
                AssetFilter.Name("string")
            };
            using (var src = new CancellationTokenSource())
            {
                src.Cancel();
                try
                {
                    var result = await client.FilterAssetsAsync(options, filters, src.Token);
                    Assert.NotNull(result);
                    Assert.NotEmpty(result.Items);
                }
                catch (TaskCanceledException e)
                {
                    Assert.IsType<TaskCanceledException>(e);
                    return;
                }
                Assert.False(true, "Expected task to fail");
            }
        }
    }
}