using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CogniteSdk;

using Xunit;

namespace Test.CSharp.Integration
{
    public class TestFixture : IDisposable, IAsyncLifetime
    {

        protected static Client ReadClient;
        protected static Client WriteClient;
        protected static Event TestEvent;

        public TestFixture()
        {
            ReadClient = CreateOAuth2Client(
                Environment.GetEnvironmentVariable("TEST_TOKEN_READ"),
                Environment.GetEnvironmentVariable("TEST_PROJECT_READ") ?? "publicdata",
                Environment.GetEnvironmentVariable("TEST_HOST_READ") ?? "https://api.cognitedata.com");
            WriteClient = CreateOAuth2Client(
                Environment.GetEnvironmentVariable("TEST_TOKEN_WRITE"),
                Environment.GetEnvironmentVariable("TEST_PROJECT_WRITE") ?? "fusiondotnet-tests",
                Environment.GetEnvironmentVariable("TEST_HOST_WRITE") ?? "https://greenfield.cognitedata.com");
        }


        protected virtual void Dispose(bool disposing)
        {
        }

        public void Dispose()
        {
            Dispose(true);
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

        private async Task PopulateDataAsync()
        {
            try
            {
                var events = await WriteClient.Events.RetrieveAsync(new List<string>() { "TestEvent" });
                TestEvent = events.FirstOrDefault();
            }
            catch (ResponseException)
            {
                TestEvent = await CreateTestEventAsync();
            }
        }

        private async Task<Event> CreateTestEventAsync()
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

            var events = await WriteClient.Events.CreateAsync(items);
            return events.FirstOrDefault();
        }

        public virtual async Task InitializeAsync()
        {
            await PopulateDataAsync();
        }

        public virtual async Task DisposeAsync()
        {
            await WriteClient.Events.DeleteAsync(new EventDelete { IgnoreUnknownIds = true, Items = new[] { Identity.Create("TestEvent") } });
        }
    }

    [CollectionDefinition("TestBase")]
    public class TestBase : ICollectionFixture<TestFixture> { }
}
