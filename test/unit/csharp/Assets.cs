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
                Client.Create(httpClient)
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
                Client.Create(httpClient)
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
                Client.Create(httpClient)
                .AddHeader("api-key", apiKey)
                .SetProject(project);

            var assetArgs = new List<GetAssets.Option> {
                GetAssets.Option.Name("string3"),
                GetAssets.Option.Source("source"),
                GetAssets.Option.Root(true)
            };

            // Act/Assert
            await Assert.ThrowsAsync<DecodeException>(() => client.GetAssetsAsync(assetArgs));
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
                Client.Create(httpClient)
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
                Client.Create(httpClient)
                .AddHeader("api-key", apiKey)
                .SetProject(project);

            // Act/Assert
            await Assert.ThrowsAsync<DecodeException>(() => client.GetAssetAsync(42L));
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
                Client.Create(httpClient)
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
                Client.Create(httpClient)
                .AddHeader("api-key", apiKey)
                .SetProject(project);

            var options = new List<UpdateAssets.Option> {
                UpdateAssets.Option.SetName("test")
            };
            var assets = new List<ValueTuple<long, IEnumerable<UpdateAssets.Option>>> {
                (42L, options)
            };

            // Act
            var result = await client.UpdateAssetsAsync(assets);

            // Assert
            Assert.True(result);
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
                Client.Create(httpClient)
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
                Client.Create(httpClient)
                .AddHeader("api-key", apiKey)
                .SetProject(project);

            var options = new List<SearchAssets.Option> {
                SearchAssets.Option.Name("str")
            };
            var filters = new List<SearchAssets.Filter> {
                SearchAssets.Filter.CreatedTime(new SearchAssets.TimeRange(DateTime.Now.Subtract(TimeSpan.FromHours(1)), DateTime.Now.Subtract(TimeSpan.FromHours(1)))),
                SearchAssets.Filter.Name("string")
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
                Client.Create(httpClient)
                .AddHeader("api-key", apiKey)
                .SetProject(project);

            var options = new List<FilterAssets.Option> {
                FilterAssets.Option.Limit(100)
            };
            var filters = new List<SearchAssets.Filter> {
                SearchAssets.Filter.CreatedTime(new SearchAssets.TimeRange(DateTime.Now.Subtract(TimeSpan.FromHours(1)), DateTime.Now.Subtract(TimeSpan.FromHours(1)))),
                SearchAssets.Filter.Name("string")
            };

            var result = await client.FilterAssetsAsync(options, filters);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Items);
        }
    }
}