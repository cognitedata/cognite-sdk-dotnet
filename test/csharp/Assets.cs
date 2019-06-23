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
using System.Net.Http;
using System.Net;

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

            var assetArgs =
                AssetArgs.Empty()
                .Name("string3")
                .ExternalIdPrefix("prefix")
                .Root(false)
                .Source("source")
                .ParentIds(new List<long> {42L, 43L });
                //.MetaData(new Dictionary<string, string> {{ "option1", "value1"}});

            // Act
            var result = await client.GetAssetsAsync(assetArgs);

            // Assert
            Assert.Single(result.Items);
            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.Equal("api.cognitedata.com", request.RequestUri.Host);
            Assert.Equal("?parentIds=%5b42%2c43%5d&source=source&root=false&externalIdPrefix=prefix&name=string3", request.RequestUri.Query);
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

            var assetArgs =
                AssetArgs.Empty()
                .Name("string3")
                .Source("source")
                .Root(true);

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

            var assets = new List<AssetCreateDto> {
                Asset.Create("name1")
                    .SetDescription("description1"),
                Asset.Create("name2")
                    .SetDescription("description2"),
                Asset.Create("name3")
                    .SetDescription("description3")
                    .SetSource("source")
                    .SetParentId(42L)
                    .SetParentExternalId("parentExtenralId")
                    .SetExternalId("uuid")
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

            var assets = new List<AssetUpdate> {
                new AssetUpdate(42L)
            };

            // Act
            var result = await client.UpdateAssetsAsync (assets);

            // Assert
            Assert.True(result);
        }
    }
}