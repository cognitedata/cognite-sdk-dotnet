// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CogniteSdk;
using CogniteSdk.Beta;
using CogniteSdk.DataModels;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Test.CSharp.Integration
{
    public class StreamRecordsFixture : TestFixture, IAsyncLifetime
    {
        public Client Write => WriteClient;

        public string TestSpace { get; private set; }
        public Dictionary<string, string> TestStreams { get; private set; }

        public ContainerIdentifier TestContainer { get; private set; }

        public override async Task InitializeAsync()
        {
            // Share space cross-tests because space existence is eventually consistent in
            // stream records...
            TestSpace = "SdkTestStreamRecordsSpace";

            var testSpace = new SpaceCreate { Space = TestSpace };

            await Write.DataModels.UpsertSpaces(new[] { testSpace });

            var testContainer = new ContainerCreate
            {
                ExternalId = "StreamTestContainer",
                Space = TestSpace,
                Name = "Test",
                UsedFor = UsedFor.record,
                Properties = new Dictionary<string, ContainerPropertyDefinition>
                {
                    { "prop", new ContainerPropertyDefinition
                    {
                        Type = BasePropertyType.Text(),
                        Nullable = true,
                    } },
                    { "intProp", new ContainerPropertyDefinition
                    {
                        Type = BasePropertyType.Create(PropertyTypeVariant.int64),
                        Nullable = true,
                    } },
                    { "boolArrayProp", new ContainerPropertyDefinition
                    {
                        Type = BasePropertyType.Create(PropertyTypeVariant.boolean, true),
                        Nullable = true,
                    }}
                }
            };
            var testContainerIdt = new ContainerIdentifier(TestSpace, "StreamTestContainer");

            await Write.DataModels.UpsertContainers(new[] { testContainer });
            TestContainer = testContainerIdt;

            TestStreams = new Dictionary<string, string>();

            // Create test streams for all test stream templates
            var allTemplateTypes = new[]
            {
                "ImmutableTestStream",
                "BasicLiveData",
            };

            foreach (var templateType in allTemplateTypes)
            {
                var streamId = $"dotnet-sdk-test-stream-{templateType.ToLowerInvariant()}";

                TestStreams[templateType] = streamId;

                try
                {
                    var streamWrite = new StreamWrite
                    {
                        ExternalId = streamId,
                        Settings = new StreamSettings
                        {
                            Template = new StreamTemplateSettings
                            {
                                Name = templateType
                            }
                        }
                    };

                    await Write.Beta.StreamRecords.CreateStreamAsync(streamWrite);
                }
                catch (ResponseException ex) when (ex.Code == 409)
                {
                    // Stream already exists, which is fine
                }
            }
        }

        public override Task DisposeAsync()
        {
            // Note: We don't call base.DisposeAsync() because we don't call base.InitializeAsync()
            // The TestEvent is not used by StreamRecords tests
            return Task.CompletedTask;
        }
    }

    public class StreamRecordsTest : IClassFixture<StreamRecordsFixture>
    {
        private readonly StreamRecordsFixture tester;
        private readonly int perTestUniqueInt;

        private static int testCounter = 0;
        public StreamRecordsTest(StreamRecordsFixture tester)
        {
            this.tester = tester;
            perTestUniqueInt = Interlocked.Increment(ref testCounter);
        }

        [Fact]
        public async Task TestListRetrieveStreams()
        {
            // Ensure TestStreams was initialized
            Assert.NotNull(tester.TestStreams);
            Assert.NotEmpty(tester.TestStreams);

            var streams = await tester.Write.Beta.StreamRecords.ListStreamsAsync();
            Assert.NotNull(streams);

            foreach (var kvp in tester.TestStreams)
            {
                var templateType = kvp.Key;
                var streamId = kvp.Value;

                Assert.NotNull(streamId);
                Assert.Contains(streams, s => s.ExternalId == streamId);

                var retrieved = await tester.Write.Beta.StreamRecords.RetrieveStreamAsync(streamId);
                Assert.NotNull(retrieved);
                Assert.Equal(streamId, retrieved.ExternalId);

            }
        }

        [Fact]
        public async Task TestIngestRecords()
        {
            // Ensure TestStreams was initialized
            Assert.NotNull(tester.TestStreams);
            Assert.True(tester.TestStreams.ContainsKey("BasicLiveData"));

            var targetStream = tester.TestStreams["BasicLiveData"];
            Assert.NotNull(targetStream);

            var req = new[] {
                new StreamRecordWrite {
                    ExternalId = $"{tester.Prefix}test-record-1",
                    Space = tester.TestSpace,
                    Sources = new[] {
                        new InstanceData<StandardInstanceWriteData>
                        {
                            Source = tester.TestContainer,
                            Properties = new StandardInstanceWriteData
                            {
                                { "prop", new RawPropertyValue<string>("Test value") },
                                { "intProp", new RawPropertyValue<long>(perTestUniqueInt) },
                                { "boolArrayProp", new RawPropertyValue<bool[]>(new[] {
                                    true, false, true
                                })}
                            }
                        }
                    }
                },
                new StreamRecordWrite {
                    ExternalId = $"{tester.Prefix}test-record-2",
                    Space = tester.TestSpace,
                    Sources = new[] {
                        new InstanceData<StandardInstanceWriteData>
                        {
                            Source = tester.TestContainer,
                            Properties = new StandardInstanceWriteData
                            {
                                { "prop", new RawPropertyValue<string>("Test value 2") },
                                { "intProp", new RawPropertyValue<long>(perTestUniqueInt) },
                                { "boolArrayProp", new RawPropertyValue<bool[]>(new[] {
                                    false, true, false
                                })}
                            }
                        }
                    }
                }
            };

            await tester.Write.Beta.StreamRecords.IngestAsync(targetStream, req);
        }

        [Fact]
        public async Task TestRetrieveRecords()
        {
            Assert.NotNull(tester.TestStreams);
            Assert.True(tester.TestStreams.ContainsKey("ImmutableTestStream"));

            var targetStream = tester.TestStreams["ImmutableTestStream"];
            Assert.NotNull(targetStream);

            // The stream records API is so eventually consistent that this test would take
            // way too long to run. Just test that we can send a request without failing.
            var start = DateTimeOffset.UtcNow.AddMinutes(-1).ToUnixTimeMilliseconds();
            await tester.Write.Beta.StreamRecords.RetrieveAsync<StandardInstanceData>(
                targetStream,
                new StreamRecordsRetrieve
                {
                    LastUpdatedTime = new LastUpdatedTimeFilter
                    {
                        GreaterThan = new RawPropertyValue<long>(start),
                        LessThan = new RawPropertyValue<long>(start + 1000 * 60 * 5),
                    },
                    Filter = new EqualsFilter
                    {
                        Property = new[] { tester.TestSpace, tester.TestContainer.ExternalId, "intProp" },
                        Value = new RawPropertyValue<long>(perTestUniqueInt),
                    },
                    Limit = 123,
                    Sort = new[] { new StreamRecordsSort {
                        Property = new[] { tester.TestSpace, tester.TestContainer.ExternalId, "intProp"}
                    }}
                }
            );
        }

        [Fact]
        public async Task TestUpsertRecords()
        {
            Assert.NotNull(tester.TestStreams);
            Assert.True(tester.TestStreams.ContainsKey("BasicLiveData"));

            var targetStream = tester.TestStreams["BasicLiveData"];
            Assert.NotNull(targetStream);

            var recordId = $"{tester.Prefix}test-upsert-record-{perTestUniqueInt}";

            // First upsert (create)
            var createReq = new[] {
                new StreamRecordWrite {
                    ExternalId = recordId,
                    Space = tester.TestSpace,
                    Sources = new[] {
                        new InstanceData<StandardInstanceWriteData>
                        {
                            Source = tester.TestContainer,
                            Properties = new StandardInstanceWriteData
                            {
                                { "prop", new RawPropertyValue<string>("Initial value") },
                                { "intProp", new RawPropertyValue<long>(100) }
                            }
                        }
                    }
                }
            };

            await tester.Write.Beta.StreamRecords.UpsertAsync(targetStream, createReq);

            // Second upsert (update)
            var updateReq = new[] {
                new StreamRecordWrite {
                    ExternalId = recordId,
                    Space = tester.TestSpace,
                    Sources = new[] {
                        new InstanceData<StandardInstanceWriteData>
                        {
                            Source = tester.TestContainer,
                            Properties = new StandardInstanceWriteData
                            {
                                { "prop", new RawPropertyValue<string>("Updated value") },
                                { "intProp", new RawPropertyValue<long>(200) }
                            }
                        }
                    }
                }
            };

            await tester.Write.Beta.StreamRecords.UpsertAsync(targetStream, updateReq);
        }

        [Fact]
        public async Task TestDeleteRecords()
        {
            Assert.NotNull(tester.TestStreams);
            Assert.True(tester.TestStreams.ContainsKey("BasicLiveData"));

            var targetStream = tester.TestStreams["BasicLiveData"];
            Assert.NotNull(targetStream);

            var recordId = $"{tester.Prefix}test-delete-record-{perTestUniqueInt}";

            // First create a record to delete
            var createReq = new[] {
                new StreamRecordWrite {
                    ExternalId = recordId,
                    Space = tester.TestSpace,
                    Sources = new[] {
                        new InstanceData<StandardInstanceWriteData>
                        {
                            Source = tester.TestContainer,
                            Properties = new StandardInstanceWriteData
                            {
                                { "prop", new RawPropertyValue<string>("To be deleted") },
                                { "intProp", new RawPropertyValue<long>(999) }
                            }
                        }
                    }
                }
            };

            await tester.Write.Beta.StreamRecords.UpsertAsync(targetStream, createReq);

            // Now delete it
            var deleteReq = new[] {
                new InstanceIdentifier(tester.TestSpace, recordId)
            };

            await tester.Write.Beta.StreamRecords.DeleteRecordsAsync(targetStream, deleteReq);
        }

        [Fact]
        public async Task TestSyncRecordsWithInitializeCursor()
        {
            Assert.NotNull(tester.TestStreams);
            Assert.True(tester.TestStreams.ContainsKey("BasicLiveData"));

            var targetStream = tester.TestStreams["BasicLiveData"];
            Assert.NotNull(targetStream);

            var syncRequest = new StreamRecordsSync
            {
                InitializeCursor = "7d-ago",
                Limit = 10,
                Sources = new[] {
                    new StreamRecordSource
                    {
                        Source = tester.TestContainer,
                        Properties = new[] { "prop", "intProp" }
                    }
                }
            };

            var response = await tester.Write.Beta.StreamRecords.SyncAsync<StandardInstanceData>(
                targetStream,
                syncRequest
            );

            Assert.NotNull(response);
            Assert.NotNull(response.NextCursor);
            Assert.NotNull(response.Items);

            // Test second call with cursor from first response
            var syncRequest2 = new StreamRecordsSync
            {
                Cursor = response.NextCursor,
                Limit = 10
            };

            var response2 = await tester.Write.Beta.StreamRecords.SyncAsync<StandardInstanceData>(
                targetStream,
                syncRequest2
            );

            Assert.NotNull(response2);
            Assert.NotNull(response2.NextCursor);
        }

        [Fact]
        public async Task TestRetrieveStreamWithStatistics()
        {
            Assert.NotNull(tester.TestStreams);
            Assert.NotEmpty(tester.TestStreams);

            var streamId = tester.TestStreams.Values.First();

            // Test with includeStatistics = false
            var streamWithoutStats = await tester.Write.Beta.StreamRecords.RetrieveStreamAsync(
                streamId,
                includeStatistics: false
            );
            Assert.NotNull(streamWithoutStats);
            Assert.Equal(streamId, streamWithoutStats.ExternalId);

            // Test with includeStatistics = true
            var streamWithStats = await tester.Write.Beta.StreamRecords.RetrieveStreamAsync(
                streamId,
                includeStatistics: true
            );
            Assert.NotNull(streamWithStats);
            Assert.Equal(streamId, streamWithStats.ExternalId);
            Assert.NotNull(streamWithStats.Settings.Limits.MaxRecordsTotal.Consumed);

            // Test without parameter (default behavior)
            var streamDefault = await tester.Write.Beta.StreamRecords.RetrieveStreamAsync(streamId);
            Assert.NotNull(streamDefault);
            Assert.Equal(streamId, streamDefault.ExternalId);
            Assert.Null(streamDefault.Settings.Limits.MaxRecordsTotal.Consumed);
        }

        [Fact]
        public async Task TestDeleteStreamIdempotent()
        {
            var nonExistentStreamId = $"non-existent-stream-{Guid.NewGuid():N}";
            await tester.Write.Beta.StreamRecords.DeleteStreamAsync(nonExistentStreamId);
        }

        [Fact]
        public async Task TestAggregateRecords()
        {
            Assert.NotNull(tester.TestStreams);
            Assert.True(tester.TestStreams.ContainsKey("BasicLiveData"));

            var targetStream = tester.TestStreams["BasicLiveData"];
            Assert.NotNull(targetStream);

            var aggregateRequest = new StreamRecordsAggregate
            {
                Aggregates = new Dictionary<string, IStreamRecordAggregate>
                {
                    {
                        "my_count", new CountStreamRecordAggregate
                        {
                            Property = new[] { tester.TestSpace, tester.TestContainer.ExternalId, "intProp" }
                        }
                    }
                }
            };

            var response = await tester.Write.Beta.StreamRecords.AggregateAsync(
                targetStream,
                aggregateRequest
            );

            Assert.NotNull(response);
            Assert.NotNull(response.Aggregates);
            Assert.True(response.Aggregates.ContainsKey("my_count"));
            Assert.IsType<CountStreamRecordAggregateResult>(response.Aggregates["my_count"]);
        }

        [Fact]
        public async Task TestSyncRecordsWithStatus()
        {
            Assert.NotNull(tester.TestStreams);
            Assert.True(tester.TestStreams.ContainsKey("BasicLiveData"));

            var targetStream = tester.TestStreams["BasicLiveData"];
            Assert.NotNull(targetStream);

            var syncRequest = new StreamRecordsSync
            {
                InitializeCursor = "7d-ago",
                Limit = 100,
                Sources = new[] {
                    new StreamRecordSource
                    {
                        Source = tester.TestContainer,
                        Properties = new[] { "prop", "intProp" }
                    }
                }
            };

            var response = await tester.Write.Beta.StreamRecords.SyncAsync<StandardInstanceData>(
                targetStream,
                syncRequest
            );

            Assert.NotNull(response);
            Assert.NotNull(response.Items);
            Assert.NotNull(response.NextCursor);

            // If there are items, verify that status is populated
            foreach (var item in response.Items)
            {
                Assert.NotNull(item.Status);
                // Status should be one of: created, updated, or deleted
                Assert.True(
                    item.Status == StreamRecordStatus.created ||
                    item.Status == StreamRecordStatus.updated ||
                    item.Status == StreamRecordStatus.deleted,
                    $"Unexpected status value: {item.Status}"
                );

                // For deleted records (tombstones), Properties may be null
                if (item.Status == StreamRecordStatus.deleted)
                {
                    // Properties are omitted for deleted records
                }
            }
        }

    }

    /// <summary>
    /// Unit tests for StreamTemplate serialization that don't require API access
    /// </summary>
    public class StreamTemplateSerializationTest
    {
        [Fact]
        public void TestStreamTemplateString_AllTemplateValues()
        {
            // Test that all stream template string values can be serialized and deserialized correctly
            var allTemplateTypes = new[]
            {
                "ImmutableTestStream",
                "BasicLiveData",
            };

            foreach (var templateType in allTemplateTypes)
            {
                // Create a StreamWrite with the template type
                var streamWrite = new StreamWrite
                {
                    ExternalId = $"test-stream-{templateType}",
                    Settings = new StreamSettings
                    {
                        Template = new StreamTemplateSettings
                        {
                            Name = templateType
                        }
                    }
                };

                // Serialize to JSON
                var json = JsonSerializer.Serialize(streamWrite, Oryx.Cognite.Common.jsonOptions);
                Assert.Contains($"\"{templateType}\"", json);

                // Deserialize back
                var deserialized = JsonSerializer.Deserialize<StreamWrite>(json, Oryx.Cognite.Common.jsonOptions);

                // Verify the template type was correctly serialized/deserialized
                Assert.Equal(templateType, deserialized.Settings.Template.Name);
                Assert.Equal($"test-stream-{templateType}", deserialized.ExternalId);
            }
        }
    }
}
