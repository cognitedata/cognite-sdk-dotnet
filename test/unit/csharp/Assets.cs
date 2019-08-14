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

using Fusion;

using Assets = Fusion.Assets;

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

            var options = new List<Assets.List.Option> {
                Assets.List.Option.Name("string3"),
                Assets.List.Option.ExternalIdPrefix("prefix"),
                Assets.List.Option.Root(false),
                Assets.List.Option.Source("source"),
                Assets.List.Option.ParentIds(new List<long> {42L, 43L })
                //.MetaData(new Dictionary<string, string> {{ "option1", "value1"}});
            };

            // Act
            var result = await client.Assets.ListAsync(options);

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

            var options = new List<Assets.List.Option> {
                Assets.List.Option.Name("string3")
            };

            // Act/Assert
            await Assert.ThrowsAsync<ResponseException>(() => client.Assets.ListAsync(options));
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

            var assetArgs = new List<Assets.List.Option> {
                Assets.List.Option.Name("string3"),
                Assets.List.Option.Source("source"),
                Assets.List.Option.Root(true)
            };

            // Act/Assert
            await Assert.ThrowsAsync<ResponseException>(() => client.Assets.ListAsync(assetArgs));
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
            await Assert.ThrowsAsync<ResponseException>(() => client.Assets.GetAsync(42L));
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

            var assets = new List<Assets.Asset> {
                new Assets.Asset () { Name = "name1", Description = "description1" },
                new Assets.Asset () { Name = "name2", Description = "description2" },
                new Assets.Asset () {
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

            var options = new List<Assets.Update.Option> {
                Assets.Update.Option.SetName("test")
            };
            var assets = new List<ValueTuple<Identity, IEnumerable<Assets.Update.Option>>> {
                (Identity.Id(42L), options)
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
                Identity.Id(42L)
            };

            // Act
            try {
                var result = await client.Assets.GetByIdsAsync(assets);
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

            var options = new List<Assets.Search.Option> {
                Assets.Search.Option.Name("str")
            };
            var filters = new List<Assets.FilterOption> {
                Assets.FilterOption.CreatedTime(new TimeRange(DateTime.Now.Subtract(TimeSpan.FromHours(1)), DateTime.Now.Subtract(TimeSpan.FromHours(1)))),
                Assets.FilterOption.Name("string")
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

            var options = new List<Assets.Filter.Option> {
                Assets.Filter.Option.Limit(100)
            };
            var filters = new List<Assets.FilterOption> {
                Assets.FilterOption.CreatedTime(new TimeRange(DateTime.Now.Subtract(TimeSpan.FromHours(1)), DateTime.Now.Subtract(TimeSpan.FromHours(1)))),
                Assets.FilterOption.Name("string")
            };

            var result = await client.Assets.FilterAsync(options, filters);

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

            var options = new List<Assets.Filter.Option> {
                Assets.Filter.Option.Limit(100),
                Assets.Filter.Option.Cursor("cursor")
            };

            var filters = new List<Assets.FilterOption> {
                Assets.FilterOption.MetaData(new Dictionary<string, string> { { "key1", "val1" }, { "key2", "val2" } }),
                Assets.FilterOption.Name("Name"),
                Assets.FilterOption.ParentIds(new List<long> { 1234, 12345 })
            };

            var result = await client.Assets.FilterAsync(options, filters);
            var refRequest = Encode.toString(0, new Assets.Filter.Request(filters, options).Encoder);
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

            var createAssets = new List<Assets.Asset>();
            for (int i = 0; i < 1000; i++) // 1000 is the maximum number of assets per request
            {
                createAssets.Add(new Assets.Asset
                {
                    Description = "Long description which takes a lot of memory to store " + i,
                    ExternalId = "ExternalIdsCanAlsoBeQuiteLongSometimes" + i,
                    Name = "Names are also fairly unrestricted " + i,
                    ParentExternalId = "With256CharactersForExternalIdsTheLimitCanGetQuiteHigh"
                });
            }

            var result = await client.Assets.CreateAsync(createAssets);
            var refRequest = Encode.toString(0, new Assets.Create.Request(createAssets.Select(Assets.WriteDto.FromAsset)).Encoder);
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

            var options = new List<Assets.Filter.Option> {
                Assets.Filter.Option.Limit(100)
            };
            var filters = new List<Assets.FilterOption> {
                Assets.FilterOption.CreatedTime(new TimeRange(DateTime.Now.Subtract(TimeSpan.FromHours(1)), DateTime.Now.Subtract(TimeSpan.FromHours(1)))),
                Assets.FilterOption.Name("string")
            };
            using (var src = new CancellationTokenSource())
            {
                src.Cancel();
                try
                {
                    var result = await client.Assets.FilterAsync(options, filters, src.Token);
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