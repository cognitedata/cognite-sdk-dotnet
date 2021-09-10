using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

using CogniteSdk;

using Xunit;

namespace Test.CSharp.Integration
{
    public class TestFixture : IDisposable
    {

        protected static Client ReadClient;
        protected static Client WriteClient;
        protected static Event TestEvent;

        public TestFixture()
        {
            ReadClient = CreateClient(Environment.GetEnvironmentVariable("TEST_API_KEY_READ"), "publicdata", "https://api.cognitedata.com");
            WriteClient = CreateOAuth2Client(Environment.GetEnvironmentVariable("TEST_TOKEN_WRITE"), "fusiondotnet-tests", "https://greenfield.cognitedata.com");
            PopulateDataAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private static Client CreateClient(string apiKey, string project, string url)
        {
            var httpClient = new HttpClient();
            return Client.Builder.Create(httpClient)
                .SetAppId("TestApp")
                .AddHeader("api-key", apiKey)
                .SetProject(project)
                .SetBaseUrl(new Uri(url))
                .Build();
        }

        private static Client CreateOAuth2Client(string accessToken, string project, string url)
        {
            var httpClient = new HttpClient();
            return Client.Builder.Create(httpClient)
                .SetAppId("TestApp")
                .AddHeader("Authorization", $"Bearer {accessToken}")
                .SetProject(project)
                .SetBaseUrl(new Uri(url))
                .Build();
        }

        private void PopulateDataAsync()
        {
            try {
                TestEvent = WriteClient.Events.RetrieveAsync(new List<string>() { "TestEvent" }).Result.FirstOrDefault();
            } catch (AggregateException) {
                TestEvent = CreateTestEventAsync();
            }
        }

        private Event CreateTestEventAsync()
        {
            var items = new List<EventCreate> {
                new EventCreate
                {
                    ExternalId = "TestEvent",
                    StartTime = 1565941329,
                    EndTime = 1565941341,
                    Type = "DotNet Test",
                    Subtype = "Dummy Event",
                    Description = "To be use for dotnet Test testing"
                }
            };

            return WriteClient.Events.CreateAsync(items).Result.FirstOrDefault();
        }
    }

    [CollectionDefinition("TestBase")]
    public class TestBase : ICollectionFixture<TestFixture> { }
}
