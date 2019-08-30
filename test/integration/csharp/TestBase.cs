using CogniteSdk;
using System;
using System.Net.Http;
using Xunit;

namespace Test.CSharp.Integration { 

    public class TestFixture : IDisposable {

        protected static Client ReadClient;
        protected static Client WriteClient;

        public TestFixture() {
            ReadClient = CreateClient(Environment.GetEnvironmentVariable("TEST_API_KEY_READ"), "publicdata", "https://api.cognitedata.com");
            WriteClient = CreateClient(Environment.GetEnvironmentVariable("TEST_API_KEY_WRITE"), "fusiondotnet-tests", "https://greenfield.cognitedata.com");

        }

        public void Dispose() { }

        private static Client CreateClient(string apiKey, string project, string url) {
            var httpClient = new HttpClient();
            return Client.Create()
                .SetAppId("TestApp")
                .SetHttpClient(httpClient)
                .AddHeader("api-key", apiKey)
                .SetProject(project)
                .SetServiceUrl(url);
        }
    }

    [CollectionDefinition("TestBase")]
    public class TestBase : ICollectionFixture<TestFixture> { }
}
