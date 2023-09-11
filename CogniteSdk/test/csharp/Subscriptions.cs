using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CogniteSdk;
using CogniteSdk.Beta;

using Com.Cognite.V1.Timeseries.Proto;

using Xunit;

namespace Test.CSharp.Integration
{
    public class SubscriptionsFixture : TestFixture, IAsyncLifetime
    {
        public Client Read => ReadClient;
        public Client Write => WriteClient;

        public IEnumerable<string> TimeSeriesIds { get; private set; }

        public override async Task InitializeAsync()
        {
            var ts = Enumerable.Range(0, 5).Select(r => new TimeSeriesCreate
            {
                ExternalId = $"{Prefix}-subtest-{r}",
                Name = $"Subscription Test {r}",
            });
            TimeSeriesIds = ts.Select(t => t.ExternalId).ToList();
            await Write.TimeSeries.CreateAsync(ts);
        }

        public override async Task DisposeAsync()
        {
            await Write.TimeSeries.DeleteAsync(new TimeSeriesDelete
            {
                IgnoreUnknownIds = true,
                Items = TimeSeriesIds.Select(Identity.Create)
            });

            var subscriptions = await Write.Beta.Subscriptions.ListAsync(new ListSubscriptions
            {
                Limit = 100
            });

            var filtered = subscriptions.Items.Where(sub => sub.ExternalId.StartsWith(Prefix)).ToList();
            if (filtered.Any())
            {
                foreach (var f in filtered)
                {
                    await Write.Beta.Subscriptions.DeleteAsync(
                        new[] { f.ExternalId },
                        true);
                }
            }
        }
    }

    public class SubscriptionsTest : IClassFixture<SubscriptionsFixture>
    {
        private readonly SubscriptionsFixture tester;
        public SubscriptionsTest(SubscriptionsFixture tester)
        {
            this.tester = tester;
        }

        [Fact]
        public async Task TestCreateUpdateRetrieveSubscription()
        {
            var sub = new SubscriptionCreate
            {
                Description = "Desc",
                ExternalId = $"{tester.Prefix}-sub-1",
                Name = "Test-sub",
                TimeSeriesIds = new[]
                {
                    tester.TimeSeriesIds.First(),
                    tester.TimeSeriesIds.Last()
                },
                PartitionCount = 2
            };

            var update = new SubscriptionUpdate
            {
                TimeSeriesIds = new UpdateEnumerable<string>(
                    new[] { tester.TimeSeriesIds.ElementAt(1) },
                    new[] { tester.TimeSeriesIds.First() }),
                Description =new UpdateNullable<string>("Desc 2"),
                Name = new UpdateNullable<string>("Test-sub-2"),
            };

            // Act
            var res = await tester.Write.Beta.Subscriptions.CreateAsync(new[] { sub });
            Assert.Single(res);

            var updRes = await tester.Write.Beta.Subscriptions.UpdateAsync(
                new[]
                {
                    new UpdateItem<SubscriptionUpdate>(sub.ExternalId)
                    {
                        Update = update
                    }
                });
            Assert.Single(updRes);

            var retrieved = await tester.Write.Beta.Subscriptions.RetrieveAsync(
                new[]
                {
                    sub.ExternalId
                });
            Assert.Single(retrieved);

            await tester.Write.Beta.Subscriptions.DeleteAsync(new[] { sub.ExternalId });

            // Assert
            var ret = retrieved.First();
            Assert.Equal("Desc 2", ret.Description);
            Assert.Equal($"{tester.Prefix}-sub-1", ret.ExternalId);
            Assert.Equal("Test-sub-2", ret.Name);
            Assert.Equal(2, ret.PartitionCount);
        }

        [Fact]
        public async Task TestListSubscriptionMembers()
        {
            var sub = new SubscriptionCreate
            {
                ExternalId = $"{tester.Prefix}-sub-2",
                TimeSeriesIds = new[]
                {
                    tester.TimeSeriesIds.First(),
                    tester.TimeSeriesIds.Last()
                },
                PartitionCount = 2
            };

            // Act
            await tester.Write.Beta.Subscriptions.CreateAsync(new[] { sub });

            var members = await tester.Write.Beta.Subscriptions.ListMembersAsync(new ListSubscriptionMembers
            {
                ExternalId = sub.ExternalId,
                Limit = 100
            });
            Assert.Equal(2, members.Items.Count());
            Assert.Contains(members.Items, m => m.ExternalId == tester.TimeSeriesIds.First());
            Assert.Contains(members.Items, m => m.ExternalId == tester.TimeSeriesIds.Last());

            await tester.Write.Beta.Subscriptions.DeleteAsync(new[] { sub.ExternalId });
        }

        [Fact]
        public async Task TestCaptureSubscriptionUpdates()
        {
            var sub = new SubscriptionCreate
            {
                ExternalId = $"{tester.Prefix}-sub-3",
                TimeSeriesIds = new[]
                {
                    tester.TimeSeriesIds.ElementAt(2),
                },
                PartitionCount = 1
            };

            var dpReq = new DataPointInsertionRequest();
            var item = new DataPointInsertionItem();
            item.ExternalId = tester.TimeSeriesIds.ElementAt(2);
            item.NumericDatapoints = new NumericDatapoints();
            item.NumericDatapoints.Datapoints.Add(new NumericDatapoint
            {
                Timestamp = 12345,
                Value = 1.0
            });
            dpReq.Items.Add(item);

            // Act
            await tester.Write.Beta.Subscriptions.CreateAsync(new[] { sub });

            await tester.Write.DataPoints.CreateAsync(dpReq);

            var results = await tester.Write.Beta.Subscriptions.ListDataAsync(
                new ListSubscriptionData
                {
                    ExternalId = sub.ExternalId,
                    Partitions = new[] { new SubscriptionPartitionRequest
                    {
                        Index = 0
                    } }
                });
            Assert.Single(results.Partitions);
            Assert.Contains(results.Updates, upd => upd.Upserts.Any(ups => ups.Timestamp == 12345));

            await tester.Write.Beta.Subscriptions.DeleteAsync(new[] { sub.ExternalId });
        }
    }
}
