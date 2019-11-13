using Xunit;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Thoth.Json.Net;

using CogniteSdk;
using CogniteSdk.Assets;

namespace Tests
{
    public class AssetTests
    {
        [Fact]
        public async Task TestListAssets()
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

            var filter = new List<AssetFilter> {
                AssetFilter.Name("string3"),
                AssetFilter.ExternalIdPrefix("prefix"),
                AssetFilter.Root(false),
                AssetFilter.Source("source"),
                AssetFilter.ParentIds(new List<long> {42L, 43L })
            };

            // Act
            var result = await client.Assets.ListAsync(new List<AssetQuery>(), filter);

            // Assert
            Assert.Single(result.Items);
            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal("api.cognitedata.com", request.RequestUri.Host);
            Assert.Equal("/api/v1/projects/project/assets/list", request.RequestUri.AbsolutePath);
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

            var filters = new List<AssetFilter> {
                AssetFilter.Name("string3")
            };

            // Act/Assert
            await Assert.ThrowsAsync<JsonDecodeException>(() => client.Assets.ListAsync(filters));
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

            var filter = new List<AssetFilter> {
                AssetFilter.Name("string3"),
                AssetFilter.Source("source"),
                AssetFilter.Root(true)
            };

            // Act/Assert
            await Assert.ThrowsAsync<JsonDecodeException>(() => client.Assets.ListAsync(filter));
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
            var result = await client.Assets.GetAsync(42L);

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
            await Assert.ThrowsAsync<JsonDecodeException>(() => client.Assets.GetAsync(42L));
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

            var assets = new List<AssetEntity> {
                new AssetEntity () { Name = "name1", Description = "description1" },
                new AssetEntity () { Name = "name2", Description = "description2" },
                new AssetEntity () {
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
            var result = await client.Assets.CreateAsync (assets);

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

            var updates = new List<AssetUpdate> {
                AssetUpdate.SetName("test")
            };
            var assets = new List<ValueTuple<Identity, IEnumerable<AssetUpdate>>> {
                (Identity.Id(42L), updates)
            };

            // Act
            await client.Assets.UpdateAsync(assets);
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
                Identity.Id(4L)
            };

            // Act
            try {
                var result = await client.Assets.GetByIdsAsync(assets);
            } catch (ResponseException ex) {
                Assert.True(ex.Code == 400);

                var error = ex.Missing.First()["id"];

                Assert.IsType<IntegerValue>(error);
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

            var options = new List<AssetSearch> {
                AssetSearch.Name("str")
            };
            var filters = new List<AssetFilter> {
                AssetFilter.CreatedTime(new TimeRange(DateTime.Now.Subtract(TimeSpan.FromHours(1)), DateTime.Now.Subtract(TimeSpan.FromHours(1)))),
                AssetFilter.Name("string")
            };

            var result = await client.Assets.SearchAsync(100, options, filters);

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

            var options = new List<AssetQuery> {
                AssetQuery.Limit(100)
            };
            var filters = new List<AssetFilter> {
                AssetFilter.CreatedTime(new TimeRange(DateTime.Now.Subtract(TimeSpan.FromHours(1)), DateTime.Now.Subtract(TimeSpan.FromHours(1)))),
                AssetFilter.Name("string")
            };

            var result = await client.Assets.ListAsync(options, filters);

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

            var options = new List<AssetQuery> {
                AssetQuery.Limit(100),
                AssetQuery.Cursor("cursor")
            };

            var filters = new List<AssetFilter> {
                AssetFilter.MetaData(new Dictionary<string, string> { { "key1", "val1" }, { "key2", "val2" } }),
                AssetFilter.Name("Name"),
                AssetFilter.ParentIds(new List<long> { 1234, 12345 })
            };

            var result = await client.Assets.ListAsync(options, filters);
            var refRequest = Encode.toString(0, new Items.Request(filters, options).Encoder);
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

            var createAssets = new List<AssetEntity>();
            for (int i = 0; i < 1000; i++) // 1000 is the maximum number of assets per request
            {
                createAssets.Add(new AssetEntity
                {
                    Description = "Long description which takes a lot of memory to store " + i,
                    ExternalId = "ExternalIdsCanAlsoBeQuiteLongSometimes" + i,
                    Name = "Names are also fairly unrestricted " + i,
                    ParentExternalId = "With256CharactersForExternalIdsTheLimitCanGetQuiteHigh"
                });
            }

            var result = await client.Assets.CreateAsync(createAssets);
            var refRequest = Encode.toString(0, new Create.Request(createAssets.Select(AssetWriteDto.FromAssetEntity)).Encoder);
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
                cancellationToken.ThrowIfCancellationRequested();
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

            var query = new List<AssetQuery> {
                AssetQuery.Limit(100)
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
                    var result = await client.Assets.ListAsync(query, filters, src.Token);
                    Assert.NotNull(result);
                    Assert.NotEmpty(result.Items);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                Assert.False(true, "Expected task to fail");
            }
        }
    }
}