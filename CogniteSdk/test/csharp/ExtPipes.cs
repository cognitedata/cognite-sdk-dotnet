using CogniteSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Test.CSharp.Integration
{
    public class ExtPipesFixture : TestFixture
    {
        public Client Read => ReadClient;
        public Client Write => WriteClient;
        public ExtPipe TestPipeline { get; private set; }
        public long DataSetId { get; private set; }
        public ExtPipesFixture()
        {
            PopulateData().Wait();
        }

        private async Task PopulateData()
        {
            var datasets = await Write.DataSets.RetrieveAsync(new[] { "test-dataset" }, true);
            if (!datasets.Any())
            {
                datasets = await Write.DataSets.CreateAsync(new[]
                {
                    new DataSetCreate
                    {
                        ExternalId = "test-dataset",
                        Name = "test-dataset"
                    }
                });
            }
            var dataset = datasets.First();
            DataSetId = dataset.Id;

            var extid = Guid.NewGuid().ToString();
            var testPipe = new ExtPipeCreate
            {
                DataSetId = DataSetId,
                ExternalId = extid,
                Name = "test extpipe"
            };
            var result = await Write.ExtPipes.CreateAsync(new[] { testPipe });
            TestPipeline = result.First();
        }
        public new void Dispose()
        {
            if (TestPipeline == null) return;
            Write.ExtPipes.DeleteAsync(new ExtPipeDelete { IgnoreUnknownIds = true, Items = new[] { Identity.Create(TestPipeline.ExternalId) } }).Wait();
        }
    }


    public class ExtPipesTest : IClassFixture<ExtPipesFixture>
    {
        private readonly ExtPipesFixture tester;
        public ExtPipesTest(ExtPipesFixture tester)
        {
            this.tester = tester;
        }

        [Fact]
        public async Task ListExtPipes()
        {
            // Arrange
            var query = new ExtPipeQuery
            {
                Limit = 10
            };

            // Act
            // Listing may take some time to resolve.
            ItemsWithCursor<ExtPipe> result = null;
            for (int i = 0; i < 10; i++)
            {
                result = await tester.Write.ExtPipes.ListAsync(query);
                if (result.Items.Any()) break;
                await Task.Delay(1000);
            }

            // Assert
            Assert.True(result.Items.Any());
        }

        [Fact]
        public async Task CreateAndDeleteExtPipes()
        {
            // Arrange
            var extId = Guid.NewGuid().ToString();
            var create = new ExtPipeCreate
            {
                Contacts = new[]
                {
                    new ExtPipeContact
                    {
                        Email = "test@test.test",
                        Name = "test",
                        Role = "test",
                        SendNotification = false
                    }
                },
                DataSetId = tester.DataSetId,
                Description = "Test description",
                Documentation = "Test documentation",
                ExternalId = extId,
                Metadata = new Dictionary<string, string> { { "testKey", "testValue" } },
                Name = "test",
                Schedule = "On trigger",
                Source = "Some source"
            };

            // Act
            var created = await tester.Write.ExtPipes.CreateAsync(new[] { create });

            await tester.Write.ExtPipes.DeleteAsync(new[] { extId });

            // Assert
            Assert.Single(created);
        }

        [Fact]
        public async Task RetrieveExtPipes()
        {
            // Act
            var result = await tester.Write.ExtPipes.RetrieveAsync(new[] { tester.TestPipeline.ExternalId }, false);

            // Assert
            Assert.Single(result);
        }
    }
}
