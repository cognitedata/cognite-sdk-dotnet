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

    public class TransformationsTest
    {
    }
}
