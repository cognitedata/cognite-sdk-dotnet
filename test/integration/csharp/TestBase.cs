using CogniteSdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;

namespace Test.CSharp.Integration
{
    [TestClass]
    public class TestBase {

        protected static Client ReadClient;
        protected static Client WriteClient;

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context) {
            ReadClient = CreateClient(Environment.GetEnvironmentVariable("TEST_API_KEY_READ"), "publicdata", "https://api.cognitedata.com");
            WriteClient = CreateClient(Environment.GetEnvironmentVariable("TEST_API_KEY_WRITE"), "fusiondotnet-tests", "https://greenfield.cognitedata.com");
        }

        private static Client CreateClient(string apiKey, string project, string url) {
            var httpClient = new HttpClient();
            return Client.Create()
                .SetAppId("TestApp")
                .SetHttpClient(httpClient)
                .AddHeader("api-key", apiKey)
                .SetProject(project)
                .SetServiceUrl(url);
        }

        public class Category {
            public const string Asset = "Asset";
            public const string Event = "Event";
            public const string Timeseries = "Timeseries";
            public const string DataPoints = "Data Points";
        }
    }
}
