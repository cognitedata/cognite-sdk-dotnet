// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
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

            // Try to create the stream, ignore errors. Streams cannot currently
            // be deleted, and there is a limit on the count per project, so we can't create
            // one per test run.
            try
            {
                await Write.Beta.StreamRecords.CreateStreamAsync(new StreamWrite
                {
                    ExternalId = TestStream,
                    Settings = new StreamSettings
                    {
                        Template = new StreamTemplateSettings
                        {
                            Name = StreamTemplateName.ImmutableTestStream
                        }
                    }
                });
            }
            catch (ResponseException ex) when (ex.Code == 409) { }
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

        [Fact(Skip = "Stream Records tests are disabled due to breaking changes in the API")]
        public async Task TestListRetrieveStreams()
        {
            var streams = await tester.Write.Beta.StreamRecords.ListStreamsAsync();
            Assert.Contains(streams, s => s.ExternalId == tester.TestStream);

            var retrieved = await tester.Write.Beta.StreamRecords.RetrieveStreamAsync(tester.TestStream);
            Assert.Equal(tester.TestStream, retrieved.ExternalId);
        }

        [Fact(Skip = "Stream Records tests are disabled due to breaking changes in the API")]
        public async Task TestIngestRecords()
        {
            // Create some records
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

            await tester.Write.Beta.StreamRecords.IngestAsync(tester.TestStream, req);
        }

        [Fact(Skip = "Stream Records tests are disabled due to breaking changes in the API")]
        public async Task TestRetrieveRecords()
        {
            // The stream records API is so eventually consistent that this test would take
            // way too long to run. Just test that we can send a request without failing.
            var start = DateTimeOffset.UtcNow.AddMinutes(-1).ToUnixTimeMilliseconds();
            await tester.Write.Beta.StreamRecords.RetrieveAsync<StandardInstanceData>(
                tester.TestStream,
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
}
