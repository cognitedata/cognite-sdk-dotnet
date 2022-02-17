using CogniteSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Test.CSharp.Integration
{
    public class TransformationsFixture : TestFixture, IAsyncLifetime
    {
        public string[] TransformationIds { get; private set; }

        public Client Read => ReadClient;
        public Client Write => WriteClient;
        public long DataSetId { get; private set; }
        public string Prefix { get; private set; }

        protected override void Dispose(bool disposing)
        {
        }

        public override async Task InitializeAsync()
        {
            Prefix = Guid.NewGuid().ToString();

            var t1 = new TransformationCreate
            {
                ConflictMode = TransformationConflictMode.upsert,
                Destination = new TransformationDestination
                {
                    Type = TransformationDestinationType.assets
                },
                ExternalId = $"{Prefix}-t1",
                Name = "test transformation 1",
                Query = "SELECT 1;",
                IsPublic = true
            };

            var t2 = new TransformationCreate
            {
                ConflictMode = TransformationConflictMode.delete,
                Destination = new TransformationDestination
                {
                    Type = TransformationDestinationType.timeseries
                },
                ExternalId = $"{Prefix}-t2",
                Name = "test transformation 2",
                Query = "SELECT 1;",
                IsPublic = true
            };

            TransformationIds = new[] { t1.ExternalId, t2.ExternalId };

            await Write.Transformations.CreateAsync(new[] { t1, t2 });
        }

        public override async Task DisposeAsync()
        {
            await Write.Transformations.DeleteAsync(new TransformationDelete {
                Items = TransformationIds.Select(Identity.Create), IgnoreUnknownIds = true
            });
        }
    }

    public class TransformationsTest : IClassFixture<TransformationsFixture>
    {
        private TransformationsFixture _tester;
        public TransformationsTest(TransformationsFixture tester)
        {
            _tester = tester;
        }

        [Fact]
        public async Task RetreiveTransformations()
        {
            var configs = await _tester.Write.Transformations.RetrieveAsync(new TransformationRetrieve
            {
                WithJobDetails = true,
                IgnoreUnknownIds = true,
                Items = new[]
                {
                    Identity.Create(123),
                    Identity.Create(_tester.TransformationIds[0]),
                    Identity.Create(_tester.TransformationIds[1])
                }
            });

            Assert.Equal(2, configs.Count());
            Assert.Equal("test transformation 1", configs.First().Name);
        }

        [Fact]
        public async Task FilterTransformations()
        {
            var configs = await _tester.Write.Transformations.FilterAsync(new TransformationFilterQuery
            {
                Filter = new TransformationFilter
                {
                    NameRegex = "test.*"
                },
                Limit = 5
            });
            Assert.True(configs.Items.Count() > 1);
        }

        [Fact]
        public async Task CreateUpdateDeleteTransformation()
        {
            var config = new TransformationCreate
            {
                ExternalId = $"{_tester.Prefix}-t3",
                Name = "Test create update delete",
                ConflictMode = TransformationConflictMode.abort,
                Destination = new TransformationDestination
                {
                    Type = TransformationDestinationType.labels
                },
            };

            var created = await _tester.Write.Transformations.CreateAsync(new[] { config });
            Assert.Single(created);

            var update = new UpdateItem<TransformationUpdate>(created.First().Id);
            update.Update = new TransformationUpdate
            {
                Name = new Update<string> { Set = "Test updated" }
            };

            var updated = await _tester.Write.Transformations.UpdateAsync(new[] { update });
            Assert.Single(updated);

            await _tester.Write.Transformations.DeleteAsync(new TransformationDelete
            {
                IgnoreUnknownIds = true,
                Items = new[] { Identity.Create(config.ExternalId) }
            });
        }
        
        [Fact]
        public async Task CreateListDeleteNotifications()
        {
            var notif = new TransformationNotificationCreate(_tester.TransformationIds[0])
            {
                Destination = "test@test.test"
            };

            var created = await _tester.Write.Transformations.SubscribeAsync(new[] { notif });
            Assert.Single(created);

            var listed = await _tester.Write.Transformations.ListNotificationsAsync(new TransformationNotificationQuery
            {
                TransformationExternalId = _tester.TransformationIds[0]
            });
            Assert.Single(listed.Items);

            await _tester.Write.Transformations.UnsubscribeAsync(new[] { listed.Items.First().Id });
        }

        [Fact]
        public async Task RunPreview()
        {
            var result = await _tester.Write.Transformations.PreviewAsync(new TransformationPreview
            {
                Limit = 10,
                SourceLimit = 10,
                Query = "SELECT 1 as field",
                ConvertToString = true
            });

            Assert.Equal("1", result.Results.Items.First()["field"]);
            Assert.Single(result.Schema.Items);
        }

        [Fact]
        public async Task GetSchema()
        {
            var result = await _tester.Write.Transformations.GetSchemaAsync(TransformationDestinationType.assets, TransformationConflictMode.upsert);
            Assert.Equal(10, result.Count());
        }
    }
}
