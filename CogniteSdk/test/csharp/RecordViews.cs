// Copyright 2026 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using CogniteSdk.DataModels;
using Oryx.Cognite;
using Xunit;

namespace Test.CSharp
{
    public class RecordViewsTests
    {
        private readonly JsonSerializerOptions _options = Common.jsonOptions;

        [Fact]
        public void TestRegularViewCreateSerializationAndDeserialization()
        {
            var viewCreate = new ViewCreate
            {
                ExternalId = "my_view",
                Space = "my_space",
                Version = "v1",
                Properties = new Dictionary<string, ICreateViewProperty>()
            };

            var json = JsonSerializer.Serialize(viewCreate, _options);
            Assert.DoesNotContain("streamId", json);

            var view = JsonSerializer.Deserialize<ViewCreate>(json, _options);
            Assert.NotNull(view);
            Assert.Equal("my_view", view.ExternalId);
            Assert.Equal("my_space", view.Space);
            Assert.Equal("v1", view.Version);
            Assert.Null(view.StreamId);
        }

        [Fact]
        public void TestRegularViewSerializationAndDeserialization()
        {
            var view = new View
            {
                ExternalId = "my_view",
                Space = "my_space",
                Version = "v1",
                Queryable = true,
                UsedFor = UsedFor.node,
                Properties = new Dictionary<string, IViewProperty>()
            };

            var json = JsonSerializer.Serialize(view, _options);
            Assert.DoesNotContain("streamId", json);

            var regularView = JsonSerializer.Deserialize<View>(json, _options);
            Assert.NotNull(regularView);
            Assert.Equal("my_view", regularView.ExternalId);
            Assert.True(regularView.Queryable);
            Assert.Equal(UsedFor.node, regularView.UsedFor);
            Assert.Null(regularView.StreamId);
        }

        [Fact]
        public void TestRecordViewCreateSerializationAndDeserialization()
        {
            var recordViewCreate = new ViewCreate
            {
                ExternalId = "my_record_view",
                Space = "my_space",
                Version = "v1",
                Name = "My Record View",
                Description = "A test record view",
                StreamId = new[] { "test_stream_1" },
                Filter = new EqualsFilter
                {
                    Property = new[] { "my_space", "my_container", "my_prop" },
                    Value = new RawPropertyValue<string>("test_value")
                },
                Properties = new Dictionary<string, ICreateViewProperty>
                {
                    {
                        "my_prop",
                        new ViewPropertyCreate
                        {
                            Container = new ContainerIdentifier("my_space", "my_container"),
                            ContainerPropertyIdentifier = "my_prop",
                            Name = "Property Name",
                            Description = "Property Description"
                        }
                    }
                }
            };

            // Serialize
            var json = JsonSerializer.Serialize(recordViewCreate, _options);
            Assert.Contains("\"streamId\":[\"test_stream_1\"]", json);
            Assert.Contains("\"externalId\":\"my_record_view\"", json);
            Assert.Contains("\"space\":\"my_space\"", json);
            Assert.Contains("\"version\":\"v1\"", json);

            var collectionJson = JsonSerializer.Serialize<IEnumerable<ViewCreate>>(
                new[] { recordViewCreate }, _options);
            Assert.Contains("\"streamId\":[\"test_stream_1\"]", collectionJson);

            // Deserialize back
            var rv = JsonSerializer.Deserialize<ViewCreate>(json, _options);
            Assert.NotNull(rv);
            Assert.Equal("my_record_view", rv.ExternalId);
            Assert.Equal("my_space", rv.Space);
            Assert.Equal("v1", rv.Version);
            Assert.Equal("My Record View", rv.Name);
            Assert.Equal("A test record view", rv.Description);
            Assert.NotNull(rv.StreamId);
            Assert.Equal("test_stream_1", Assert.Single(rv.StreamId));
            Assert.NotNull(rv.Properties);
            Assert.True(rv.Properties.ContainsKey("my_prop"));
        }

        [Fact]
        public void TestRecordViewDeserialization()
        {
            var json = @"{
                ""externalId"": ""test_record_view"",
                ""space"": ""test_space"",
                ""version"": ""v1"",
                ""name"": ""Test Record View"",
                ""description"": ""Record view description"",
                ""createdTime"": 1700000000000,
                ""lastUpdatedTime"": 1700000001000,
                ""writable"": true,
                ""queryable"": true,
                ""usedFor"": ""record"",
                ""isGlobal"": false,
                ""streamId"": [""stream_abc""],
                ""mappedContainers"": [
                    {
                        ""type"": ""container"",
                        ""space"": ""test_space"",
                        ""externalId"": ""test_container""
                    }
                ],
                ""properties"": {
                    ""mappedProp"": {
                        ""container"": {
                            ""type"": ""container"",
                            ""space"": ""test_space"",
                            ""externalId"": ""test_container""
                        },
                        ""containerPropertyIdentifier"": ""raw_prop"",
                        ""name"": ""Mapped Property"",
                        ""type"": {
                            ""type"": ""text"",
                            ""list"": false
                        }
                    }
                }
            }";

            var recordView = JsonSerializer.Deserialize<View>(json, _options);
            Assert.NotNull(recordView);
            Assert.Equal("test_record_view", recordView.ExternalId);
            Assert.Equal("test_space", recordView.Space);
            Assert.Equal("v1", recordView.Version);
            Assert.Equal("Test Record View", recordView.Name);
            Assert.Equal("Record view description", recordView.Description);
            Assert.Equal(1700000000000, recordView.CreatedTime);
            Assert.Equal(1700000001000, recordView.LastUpdatedTime);
            Assert.True(recordView.Writable);
            Assert.True(recordView.Queryable);
            Assert.Equal(UsedFor.record, recordView.UsedFor);
            Assert.False(recordView.IsGlobal);
            Assert.NotNull(recordView.StreamId);
            Assert.Equal("stream_abc", Assert.Single(recordView.StreamId));
            Assert.NotNull(recordView.MappedContainers);
            var containerRef = Assert.Single(recordView.MappedContainers);
            Assert.Equal("test_space", containerRef.Space);
            Assert.Equal("test_container", containerRef.ExternalId);
            Assert.NotNull(recordView.Properties);
            Assert.True(recordView.Properties.ContainsKey("mappedProp"));
            var prop = Assert.IsType<ViewPropertyDefinition>(recordView.Properties["mappedProp"]);
            Assert.Equal("Mapped Property", prop.Name);
            Assert.Equal("raw_prop", prop.ContainerPropertyIdentifier);
        }

        [Fact]
        public void TestContainerGlobalFieldDeserialization()
        {
            const string json = "{\"space\":\"my_space\",\"externalId\":\"my_container\",\"usedFor\":\"record\",\"isGlobal\":true}";

            var container = JsonSerializer.Deserialize<Container>(json, _options);

            Assert.NotNull(container);
            Assert.True(container.IsGlobal);
        }

        [Fact]
        public void TestMixedViewsCollectionDeserialization()
        {
            var regularViewJson = @"{
                ""externalId"": ""normal_view"",
                ""space"": ""test_space"",
                ""version"": ""v1"",
                ""createdTime"": 1000,
                ""lastUpdatedTime"": 2000,
                ""writable"": true,
                ""usedFor"": ""node"",
                ""properties"": {}
            }";

            var recordViewJson = @"{
                ""externalId"": ""record_view"",
                ""space"": ""test_space"",
                ""version"": ""v1"",
                ""createdTime"": 3000,
                ""lastUpdatedTime"": 4000,
                ""writable"": true,
                ""usedFor"": ""record"",
                ""streamId"": [""stream_1""],
                ""properties"": {}
            }";

            var collectionJson = $"[{regularViewJson},{recordViewJson}]";

            var views = JsonSerializer.Deserialize<List<View>>(collectionJson, _options);
            Assert.NotNull(views);
            Assert.Equal(2, views.Count);

            // Regular view has no stream
            Assert.Equal("normal_view", views[0].ExternalId);
            Assert.Equal(UsedFor.node, views[0].UsedFor);
            Assert.Null(views[0].StreamId);

            // Record view carries its stream
            Assert.Equal("record_view", views[1].ExternalId);
            Assert.Equal(UsedFor.record, views[1].UsedFor);
            Assert.Equal("stream_1", Assert.Single(views[1].StreamId));
        }

        [Fact]
        public void TestViewQueryToQueryParams()
        {
            var query = new ViewQuery
            {
                Space = "test_space",
                AllVersions = true,
                IncludeInheritedProperties = false,
                IncludeGlobal = true,
                UsedFor = new[] { UsedFor.record }
            };

            var parms = query.ToQueryParams();
            Assert.Contains(("space", "test_space"), parms);
            Assert.Contains(("allVersions", "true"), parms);
            Assert.Contains(("includeInheritedProperties", "false"), parms);
            Assert.Contains(("includeGlobal", "true"), parms);
            Assert.Contains(("usedFor", "record"), parms);

            // Test multiple UsedFor values
            var multiQuery = new ViewQuery
            {
                UsedFor = new[] { UsedFor.node, UsedFor.edge, UsedFor.record }
            };
            var multiParms = multiQuery.ToQueryParams();
            Assert.Equal(3, multiParms.Count(p => p.Item1 == "usedFor"));
            Assert.Contains(("usedFor", "node"), multiParms);
            Assert.Contains(("usedFor", "edge"), multiParms);
            Assert.Contains(("usedFor", "record"), multiParms);
        }

        [Fact]
        public void TestContainersQueryToQueryParams()
        {
            var query = new ContainersQuery
            {
                Space = "container_space",
                IncludeGlobal = true,
                UsedFor = new[] { UsedFor.record, UsedFor.all }
            };

            var parms = query.ToQueryParams();
            Assert.Contains(("space", "container_space"), parms);
            Assert.Contains(("includeGlobal", "true"), parms);
            Assert.Contains(("usedFor", "record"), parms);
            Assert.Contains(("usedFor", "all"), parms);
        }
    }
}
