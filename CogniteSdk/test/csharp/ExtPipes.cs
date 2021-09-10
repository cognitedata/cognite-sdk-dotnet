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
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Write.ExtPipes.DeleteAsync(new ExtPipeDelete { Items = new[] { Identity.Create(TestPipeline.ExternalId) } }).Wait();
            }
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

        [Fact]
        public async Task CreateUpdateDeleteExtPipes()
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
            var extId2 = Guid.NewGuid().ToString();
            var update = new ExtPipeUpdate
            {
                Contacts = new UpdateEnumerable<ExtPipeContact>(new[] { new ExtPipeContact
                {
                    Email = "test2@test.test",
                    Name = "test2",
                    Role = "test2",
                    SendNotification = false
                } }, new[] { create.Contacts.First() }),
                Description = new Update<string>("Test description 2"),
                Documentation = new Update<string>("Test documentation 2"),
                ExternalId = new Update<string>(extId2),
                Metadata = new UpdateDictionary<string>(new Dictionary<string, string> { { "testKey2", "testValue2" } },
                    new[] { "testKey" }),
                Name = new Update<string>("test 2"),
                Schedule = new Update<string>("Continuous"),
                Source = new Update<string>("Some other source")
            };

            // Act
            await tester.Write.ExtPipes.CreateAsync(new[] { create });
            var result = await tester.Write.ExtPipes.UpdateAsync(new[] { new UpdateItem<ExtPipeUpdate>(extId) { Update = update } });
            await tester.Write.ExtPipes.DeleteAsync(new ExtPipeDelete { Items = new[] { Identity.Create(extId2) } });

            // Assert
            Assert.Single(result);
            var pipe = result.First();
            Assert.Single(pipe.Metadata);
            Assert.Single(pipe.Contacts);
            Assert.Equal("Continuous", pipe.Schedule);
        }
        [Fact]
        public async Task CreateListExtPipeRuns()
        {
            var runs = new[]
            {
                new ExtPipeRunCreate
                {
                    ExternalId = tester.TestPipeline.ExternalId,
                    Message = "test seen",
                    Status = ExtPipeRunStatus.seen
                },
                new ExtPipeRunCreate
                {
                    ExternalId = tester.TestPipeline.ExternalId,
                    Message = "test fail",
                    Status = ExtPipeRunStatus.failure
                },
                new ExtPipeRunCreate
                {
                    ExternalId = tester.TestPipeline.ExternalId,
                    Message = "test success",
                    Status = ExtPipeRunStatus.success
                }
            };

            // Act
            foreach (var run in runs)
            {
                await tester.Write.ExtPipes.CreateRunsAsync(new[] { run });
            }
            ItemsWithCursor<ExtPipeRun> read = null;
            for (int i = 0; i < 10; i++)
            {
                read = await tester.Write.ExtPipes.ListRunsAsync(new ExtPipeRunQuery
                {
                    Filter = new ExtPipeRunFilter { ExternalId = tester.TestPipeline.ExternalId }
                });
                if (read.Items.Count() >= 3) break;
                await Task.Delay(1000);
            }

            Assert.True(read.Items.Count() >= 3);
        }
    }
}
