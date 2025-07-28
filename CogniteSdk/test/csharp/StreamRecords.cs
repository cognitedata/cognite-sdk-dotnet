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
        public string TestStream { get; private set; }
        public Dictionary<StreamTemplateName, string> TestStreams { get; private set; }

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
                ExternalId = "TestContainer",
                Space = TestSpace,
                Name = "Test",
                UsedFor = UsedFor.all,
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
            var testContainerIdt = new ContainerIdentifier(TestSpace, "TestContainer");

            await Write.DataModels.UpsertContainers(new[] { testContainer });
            TestContainer = testContainerIdt;

            TestStream = "dotnet-sdk-test-stream";
            TestStreams = new Dictionary<StreamTemplateName, string>();

            // Create test streams for all template types to exercise the converter
            var allTemplateTypes = new[]
            {
                StreamTemplateName.ImmutableTestStream,
                StreamTemplateName.ImmutableDataStaging,
                StreamTemplateName.ImmutableNormalizedData,
                StreamTemplateName.ImmutableArchive,
                StreamTemplateName.MutableTestStream,
                StreamTemplateName.MutableLiveData
            };

            foreach (var templateType in allTemplateTypes)
            {
                var streamId = $"dotnet-sdk-test-{templateType.ToString().ToLowerInvariant()}";

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

        public override async Task DisposeAsync()
        {
            // Clean up all created test streams with throttling to avoid rate limits
            if (TestStreams != null)
            {
                foreach (var streamId in TestStreams.Values)
                {
                    if (!string.IsNullOrEmpty(streamId))
                    {
                        try
                        {
                            await Write.Beta.StreamRecords.DeleteStreamAsync(streamId);
                            // Add small delay to avoid overwhelming the API
                            await Task.Delay(100);
                        }
                        catch (ResponseException)
                        {
                            // Ignore errors during cleanup - stream might not exist or already be deleted
                        }
                    }
                }
            }
            
            try
            {
                await base.DisposeAsync();
            }
            catch (ResponseException)
            {
                // Ignore other cleanup errors
            }
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
            Assert.True(tester.TestStreams.ContainsKey(StreamTemplateName.MutableTestStream));

            var targetStream = tester.TestStreams[StreamTemplateName.MutableTestStream];
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
            Assert.True(tester.TestStreams.ContainsKey(StreamTemplateName.ImmutableDataStaging));

            var targetStream = tester.TestStreams[StreamTemplateName.ImmutableDataStaging];
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

    }

    /// <summary>
    /// Unit tests for StreamTemplateNameConverter that don't require API access
    /// </summary>
    public class StreamTemplateNameConverterTest
    {
        [Fact]
        public void TestStreamTemplateNameConverter_AllEnumValues()
        {
            // Test that all StreamTemplateName enum values can be serialized and deserialized correctly
            // This ensures the converter handles all cases and improves coverage
            var allTemplateTypes = new[]
            {
                StreamTemplateName.ImmutableTestStream,
                StreamTemplateName.ImmutableDataStaging,
                StreamTemplateName.ImmutableNormalizedData,
                StreamTemplateName.ImmutableArchive,
                StreamTemplateName.MutableTestStream,
                StreamTemplateName.MutableLiveData
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
