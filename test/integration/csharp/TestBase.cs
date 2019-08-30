using CogniteSdk;
using CogniteSdk.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Test.CSharp.Integration { 

    public class TestFixture : IDisposable {

        protected static Client ReadClient;
        protected static Client WriteClient;
        protected static EventEntity TestEvent;

        public TestFixture() {
            ReadClient = CreateClient(Environment.GetEnvironmentVariable("TEST_API_KEY_READ"), "publicdata", "https://api.cognitedata.com");
            WriteClient = CreateClient(Environment.GetEnvironmentVariable("TEST_API_KEY_WRITE"), "fusiondotnet-tests", "https://greenfield.cognitedata.com");

            PopulateDataAsync();

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

        private void PopulateDataAsync() {
            try {
                TestEvent = WriteClient.Events.GetByIdsAsync(new List<string>() { "TestEvent" }).Result.FirstOrDefault();
            } catch (AggregateException) {
                TestEvent = CreateTestEventAsync();
            }
        }

        private EventEntity CreateTestEventAsync() {
            var newEvent = new EventEntity();
            newEvent.ExternalId = "TestEvent";
            newEvent.StartTime = 1565941329;
            newEvent.EndTime = 1565941341;
            newEvent.Type = "DotNet Test";
            newEvent.SubType = "Dummy Event";
            newEvent.Description = "To be use for dotnet Test testing";

            return WriteClient.Events.CreateAsync(new List<EventEntity>() { newEvent }).Result.FirstOrDefault();
        }
    }

    [CollectionDefinition("TestBase")]
    public class TestBase : ICollectionFixture<TestFixture> { }
}
