﻿// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CogniteSdk;
using CogniteSdk.DataModels;
using CogniteSdk.Resources.DataModels;
using Xunit;

namespace Test.CSharp.Integration
{
    public class DataModelsFixture : TestFixture, IAsyncLifetime
    {
        public Client Read => ReadClient;
        public Client Write => WriteClient;

        public string TestSpace { get; private set; }
        public ContainerIdentifier TestContainer { get; private set; }
        public ViewIdentifier TestView { get; private set; }

        protected override void Dispose(bool disposing)
        {
        }

        public override async Task InitializeAsync()
        {
            TestSpace = $"{Prefix}Space";

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
                    { "refProp", new ContainerPropertyDefinition
                    {
                        Type = BasePropertyType.Direct(),
                        Nullable = true,
                    } },
                    { "intProp", new ContainerPropertyDefinition
                    {
                        Type = BasePropertyType.Create(PropertyTypeVariant.int64),
                        Nullable = true,
                    } }
                }
            };
            var testContainerIdt = new ContainerIdentifier(TestSpace, "TestContainer");

            var testView = new ViewCreate
            {
                ExternalId = "TestView",
                Name = "Test",
                Space = TestSpace,
                Filter = new MatchAllFilter(),
                Properties = new Dictionary<string, ICreateViewProperty>
                {
                    { "prop", new ViewPropertyCreate
                    {
                        Container = testContainerIdt,
                        ContainerPropertyIdentifier = "prop",
                        Name = "Property"
                    } },
                    { "refProp", new ViewPropertyCreate
                    {
                        Container = testContainerIdt,
                        ContainerPropertyIdentifier = "refProp",
                        Name = "Property Reference"
                    } },
                    { "intProp", new ViewPropertyCreate
                    {
                        Container = testContainerIdt,
                        ContainerPropertyIdentifier = "intProp",
                        Name = "Property Integer"
                    } }
                },
                Version = "1"
            };

            await Write.DataModels.UpsertContainers(new[] { testContainer });
            await Write.DataModels.UpsertViews(new[] { testView });

            TestContainer = testContainerIdt;
            TestView = new ViewIdentifier(TestSpace, "TestView", "1");
        }

        public override async Task DisposeAsync()
        {
            if (TestContainer != null) await Write.DataModels.DeleteContainers(new[] { TestContainer.ContainerId() });
            if (TestView != null) await Write.DataModels.DeleteViews(new[] { TestView.FDMExternalId() });
            if (TestSpace != null) await Write.DataModels.DeleteSpaces(new[] { TestSpace });
        }
    }
    public class DataModelsTest : IClassFixture<DataModelsFixture>
    {
        private readonly DataModelsFixture tester;
        public DataModelsTest(DataModelsFixture tester)
        {
            this.tester = tester;
        }

        [Fact]
        public async Task TestCreateUpdateRetrieveDeleteSpaces()
        {
            var extid = $"{tester.Prefix}TestCreateRetrieveSpace";
            var space = new SpaceCreate { Space = extid };

            // Create a space
            var created = await tester.Write.DataModels.UpsertSpaces(new[] { space });
            Assert.Single(created);

            try
            {
                // Update space name
                space.Name = "Test Space Updated";
                var updated = await tester.Write.DataModels.UpsertSpaces(new[] { space });
                Assert.Single(updated);
                Assert.Equal("Test Space Updated", updated.First().Name);

                // Retrieve space by id
                var retrieved = await tester.Write.DataModels.RetrieveSpaces(new[] { extid });
                Assert.Single(retrieved);
            }
            finally
            {
                // Delete space
                var deleted = await tester.Write.DataModels.DeleteSpaces(new[] { extid });
                Assert.Single(deleted);
            }
        }

        [Fact]
        public async Task TestCreateUpdateRetrieveDeleteModel()
        {
            var extid = "TestCreateRetrieveModel";
            var model = new DataModelCreate
            {
                Space = tester.TestSpace,
                ExternalId = extid,
                Version = "1",
                Views = new[]
                {
                    (IViewCreateOrReference)tester.TestView,
                    new ViewCreate
                    {
                        ExternalId = "TestCreateInModel",
                        Filter = new MatchAllFilter(),
                        Implements = new[]
                        {
                            tester.TestView
                        },
                        Space = tester.TestSpace,
                        Version = "1",
                        Properties = new Dictionary<string, ICreateViewProperty>()
                    }
                }
            };
            var id = new FDMExternalId(extid, tester.TestSpace, "1");

            // Create a data model
            var created = await tester.Write.DataModels.UpsertDataModels(new[] { model });
            Assert.Single(created);

            try
            {
                // Retrieve a data model
                var retrieved = await tester.Write.DataModels.RetrieveDataModels(new[] { id }, true);
                Assert.Single(retrieved);
                var retModel = retrieved.First();
                Assert.Equal(2, retModel.Views.Count());
                Assert.Contains(retModel.Views, view => (view as View).ExternalId == "TestView");

                // Update a data model
                model.Description = "Test description";
                var updated = await tester.Write.DataModels.UpsertDataModels(new[] { model });
                Assert.Single(updated);
                Assert.Equal("Test description", updated.First().Description);
            }
            finally
            {
                // Delete the data model
                var deleted = await tester.Write.DataModels.DeleteDataModels(new[]
                {
                    id
                });
                // Delete the implicitly created view
                await tester.Write.DataModels.DeleteViews(new[]
                {
                    new FDMExternalId("TestCreateInModel", tester.TestSpace, "1")
                });
                Assert.Single(deleted);
            }
        }

        [Fact]
        public async Task TestCreateUpdateRetrieveDeleteView()
        {
            var extid = "TestCreateRetrieveView";
            var view = new ViewCreate
            {
                ExternalId = extid,
                Space = tester.TestSpace,
                Name = "Test 2",
                Version = "1",
                Properties = new Dictionary<string, ICreateViewProperty>
                {
                    { "prop", new ViewPropertyCreate
                    {
                        Container = tester.TestContainer,
                        Name = "Property",
                        ContainerPropertyIdentifier = "prop"
                    } }
                }
            };

            var created = await tester.Write.DataModels.UpsertViews(new[] { view });
            Assert.Single(created);

            var id = new FDMExternalId(extid, tester.TestSpace, "1");

            try
            {
                // Retrieve a vuew
                var retrieved = await tester.Write.DataModels.RetrieveViews(new[] { id });
                Assert.Single(retrieved);

                // Update a view
                view.Description = "Test description";
                var updated = await tester.Write.DataModels.UpsertViews(new[] { view });
                Assert.Single(updated);
                Assert.Equal("Test description", updated.First().Description);
            }
            finally
            {
                // Delete the view
                var deleted = await tester.Write.DataModels.DeleteViews(new[] { id });
                Assert.Single(deleted);
            }
        }

        [Fact]
        public async Task TestCreateUpdateRetrieveDeleteContainer()
        {
            var extid = "TestCreateRetrieveContainer";
            var container = new ContainerCreate
            {
                ExternalId = extid,
                Space = tester.TestSpace,
                Name = "Test 2",
                UsedFor = UsedFor.node,
                Properties = new Dictionary<string, ContainerPropertyDefinition>
                {
                    { "prop", new ContainerPropertyDefinition
                    {
                        Nullable = true,
                        Type = BasePropertyType.Text(true),
                        Name = "prop"
                    } }
                }
            };

            var created = await tester.Write.DataModels.UpsertContainers(new[] { container });
            Assert.Single(created);

            var id = new ContainerId(extid, tester.TestSpace);

            try
            {
                // Retrieve a container
                var retrieved = await tester.Write.DataModels.RetrieveContainers(new[] { id });
                Assert.Single(retrieved);

                // Update a container
                container.Description = "Test description";
                var updated = await tester.Write.DataModels.UpsertContainers(new[] { container });
                Assert.Single(updated);
                Assert.Equal("Test description", updated.First().Description);
            }
            finally
            {
                // Delete the container
                var deleted = await tester.Write.DataModels.DeleteContainers(new[] { id });
                Assert.Single(deleted);
            }
        }

        [Fact]
        public async Task TestCreateRetrieveDeleteNodes()
        {
            var req = new InstanceWriteRequest
            {
                Items = new[]
                {
                    new NodeWrite
                    {
                        ExternalId = "node1",
                        Space = tester.TestSpace,
                        Sources = new[]
                        {
                            new InstanceData<StandardInstanceWriteData>
                            {
                                Properties = new StandardInstanceWriteData
                                {
                                    { "prop", new RawPropertyValue<string>("Test value") },
                                    { "intProp", new RawPropertyValue<long>(123) }
                                },
                                Source = tester.TestContainer
                            }
                        }
                    },
                    new NodeWrite
                    {
                        ExternalId = "node2",
                        Space = tester.TestSpace,
                        Sources = new[]
                        {
                            new InstanceData<StandardInstanceWriteData>
                            {
                                Properties = new StandardInstanceWriteData
                                {
                                    { "prop", new RawPropertyValue<string>("Test value") },
                                    { "refProp", new DirectRelationIdentifier(tester.TestSpace, "node1") }
                                },
                                Source = tester.TestContainer
                            }
                        }
                    }
                }
            };

            var created = await tester.Write.DataModels.UpsertInstances(req);
            Assert.Equal(2, created.Count());

            var ids = new[] {
                new InstanceIdentifierWithType(InstanceType.node, tester.TestSpace, "node1"),
                new InstanceIdentifierWithType(InstanceType.node, tester.TestSpace, "node2")
            };

            try
            {
                var retrieved = await tester.Write.DataModels.RetrieveInstances<StandardInstanceData>(new InstancesRetrieve
                {
                    Sources = new[]
                    {
                        new InstanceSource
                        {
                            Source = tester.TestView
                        }
                    },
                    Items = ids,
                    IncludeTyping = true
                });
                Assert.Equal(2, retrieved.Items.Count());
                Assert.NotNull(retrieved.Typing);
                var item = retrieved.Items.First(n => n.ExternalId == "node1");
                Assert.Equal("Test value", (item.Properties[tester.TestSpace][tester.TestView.ExternalId + "/1"]["prop"] as RawPropertyValue<string>).Value);
            }
            finally
            {
                var deleted = await tester.Write.DataModels.DeleteInstances(ids);
                Assert.Equal(2, deleted.Count());
            }
        }

        [Fact]
        public async Task TestCreateRetrieveDeleteEdges()
        {
            var req = new InstanceWriteRequest
            {
                AutoCreateEndNodes = true,
                AutoCreateStartNodes = true,
                Items = new[]
                {
                    new EdgeWrite
                    {
                        ExternalId = "edge1",
                        Space = tester.TestSpace,
                        Sources = new[]
                        {
                            new InstanceData<StandardInstanceWriteData>
                            {
                                Properties = new StandardInstanceWriteData
                                {
                                    { "prop", new RawPropertyValue<string>("Test value") },
                                    { "intProp", new RawPropertyValue<long>(123) }
                                },
                                Source = tester.TestContainer
                            }
                        },
                        StartNode = new DirectRelationIdentifier(tester.TestSpace, "node3"),
                        EndNode = new DirectRelationIdentifier(tester.TestSpace, "node4"),
                        Type = new DirectRelationIdentifier(tester.TestSpace, "node5")
                    },
                    new EdgeWrite
                    {
                        ExternalId = "edge2",
                        Space = tester.TestSpace,
                        Sources = new[]
                        {
                            new InstanceData<StandardInstanceWriteData>
                            {
                                Properties = new StandardInstanceWriteData
                                {
                                    { "prop", new RawPropertyValue<string>("Test value") },
                                    { "refProp", new DirectRelationIdentifier(tester.TestSpace, "node4") }
                                },
                                Source = tester.TestContainer
                            }
                        },
                        StartNode = new DirectRelationIdentifier(tester.TestSpace, "node3"),
                        EndNode = new DirectRelationIdentifier(tester.TestSpace, "node4"),
                        Type = new DirectRelationIdentifier(tester.TestSpace, "node5")
                    }
                }
            };

            var created = await tester.Write.DataModels.UpsertInstances(req);
            Assert.Equal(2, created.Count());

            var ids = new[] {
                new InstanceIdentifierWithType(InstanceType.edge, tester.TestSpace, "edge1"),
                new InstanceIdentifierWithType(InstanceType.edge, tester.TestSpace, "edge2")
            };

            try
            {
                var retrieved = await tester.Write.DataModels.RetrieveInstances<StandardInstanceData>(new InstancesRetrieve
                {
                    Sources = new[]
                    {
                        new InstanceSource
                        {
                            Source = tester.TestView
                        }
                    },
                    Items = ids,
                    IncludeTyping = true
                });
                Assert.Equal(2, retrieved.Items.Count());
                Assert.NotNull(retrieved.Typing);
                var item = retrieved.Items.First(n => n.ExternalId == "edge1");
                Assert.Equal("Test value", (item.Properties[tester.TestSpace][tester.TestView.ExternalId + "/1"]["prop"] as RawPropertyValue<string>).Value);
            }
            finally
            {
                var deleted = await tester.Write.DataModels.DeleteInstances(ids);
                Assert.Equal(2, deleted.Count());
                var deletedNodes = await tester.Write.DataModels.DeleteInstances(new[]
                {
                    new InstanceIdentifierWithType(InstanceType.node, tester.TestSpace, "node3"),
                    new InstanceIdentifierWithType(InstanceType.node, tester.TestSpace, "node4"),
                    new InstanceIdentifierWithType(InstanceType.node, tester.TestSpace, "node5"),
                });
                Assert.Equal(3, deletedNodes.Count());
            }
        }

        [Fact]
        public void TestFilterSerializer()
        {
            var filter = new AndFilter
            {
                And = new IDMSFilter[]
                {
                    new EqualsFilter
                    {
                        Property = new [] { "space", "view", "prop" },
                        Value = new RawPropertyValue<string>("value")
                    },
                    new NotFilter
                    {
                        Not = new PrefixFilter
                        {
                            Property = new [] { "edge", "externalId" },
                            Value = new RawPropertyValue<string>("prefix")
                        }
                    }
                }
            };

            Assert.Equal(@"{""and"":[{""equals"":{""property"":[""space"",""view"",""prop""],""value"":""value""}},{""not"":{""prefix"":{""property"":[""edge"",""externalId""],""value"":""prefix""}}}]}",
                JsonSerializer.Serialize(filter, Oryx.Cognite.Common.jsonOptions));
            var reversed = JsonSerializer.Deserialize<IDMSFilter>(JsonSerializer.Serialize(filter, Oryx.Cognite.Common.jsonOptions), Oryx.Cognite.Common.jsonOptions);
            var andFilter = Assert.IsType<AndFilter>(reversed);
            Assert.Equal(2, andFilter.And.Count());
            var eqFilter = Assert.IsType<EqualsFilter>(andFilter.And.First());
            Assert.Equal("value", (eqFilter.Value as RawPropertyValue<string>).Value);
        }

        [Fact]
        public void TestDeserializeArrayValue()
        {
            {
                var res = JsonSerializer.Deserialize<IDMSValue>("[1, 2, 3]", Oryx.Cognite.Common.jsonOptions);
                var resArr = Assert.IsType<RawPropertyValue<double[]>>(res);
                Assert.Equal(3, resArr.Value.Length);
            }
            {
                var res = JsonSerializer.Deserialize<IDMSValue>(@"[""test"", ""test2"", ""test3""]", Oryx.Cognite.Common.jsonOptions);
                var resArr = Assert.IsType<RawPropertyValue<string[]>>(res);
                Assert.Equal(3, resArr.Value.Length);
            }
            {
                var res = JsonSerializer.Deserialize<IDMSValue>("[true, false, true]", Oryx.Cognite.Common.jsonOptions);
                var resArr = Assert.IsType<RawPropertyValue<bool[]>>(res);
                Assert.Equal(3, resArr.Value.Length);
            }
            {
                var res = JsonSerializer.Deserialize<IDMSValue>("[[1], [2], [3]]", Oryx.Cognite.Common.jsonOptions);
                var resArr = Assert.IsType<RawPropertyValue<double[][]>>(res);
                Assert.Equal(3, resArr.Value.Length);
            }
            {
                var res = JsonSerializer.Deserialize<IDMSValue>("[]", Oryx.Cognite.Common.jsonOptions);
                var resArr = Assert.IsType<RawPropertyValue<object[]>>(res);
                Assert.Empty(resArr.Value);
            }
        }

        [Fact]
        public async Task TestRunQuery()
        {
            var req = new InstanceWriteRequest
            {
                Items = new[]
                {
                    new NodeWrite
                    {
                        ExternalId = "node6",
                        Space = tester.TestSpace,
                        Sources = new[]
                        {
                            new InstanceData<StandardInstanceWriteData>
                            {
                                Properties = new StandardInstanceWriteData
                                {
                                    { "prop", new RawPropertyValue<string>("Test value 2") },
                                    { "intProp", new RawPropertyValue<long>(321) }
                                },
                                Source = tester.TestContainer
                            }
                        }
                    },
                    new NodeWrite
                    {
                        ExternalId = "node7",
                        Space = tester.TestSpace,
                        Sources = new[]
                        {
                            new InstanceData<StandardInstanceWriteData>
                            {
                                Properties = new StandardInstanceWriteData
                                {
                                    { "prop", new RawPropertyValue<string>("Test value 3") },
                                    { "refProp", new DirectRelationIdentifier(tester.TestSpace, "node6") }
                                },
                                Source = tester.TestContainer
                            }
                        }
                    }
                }
            };

            var created = await tester.Write.DataModels.UpsertInstances(req);
            Assert.Equal(2, created.Count());

            var ids = new[] {
                new InstanceIdentifierWithType(InstanceType.node, tester.TestSpace, "node6"),
                new InstanceIdentifierWithType(InstanceType.node, tester.TestSpace, "node7")
            };

            var q = new Query
            {
                Select = new Dictionary<string, SelectExpression>
                {
                    { "res1", new SelectExpression
                    {
                        Sources = new[]
                        {
                            new SelectSource
                            {
                                Source = tester.TestView,
                                Properties = new[] { "prop" }
                            }
                        }
                    } }
                },
                With = new Dictionary<string, IQueryTableExpression>
                {
                    { "res1", new QueryNodeTableExpression
                    {
                        Nodes = new QueryNodes
                        {
                            Filter = new AndFilter
                            {
                                And = new[]
                                {
                                    new EqualsFilter
                                    {
                                        Property = new[] { tester.TestSpace, tester.TestView.ExternalId + "/1", "intProp" },
                                        Value = new RawPropertyValue<long>(321)
                                    },
                                    new EqualsFilter
                                    {
                                        Property = new[] { "node", "space" },
                                        Value = new RawPropertyValue<string>(tester.TestSpace)
                                    }
                                }
                            }
                        }
                    } }
                }
            };

            try
            {
                var queryResult = await tester.Write.DataModels.QueryInstances<StandardInstanceData>(q);

                Assert.Equal("res1", queryResult.Items.First().Key);
                Assert.Single(queryResult.Items["res1"]);
                var node = queryResult.Items["res1"].First();
                Assert.Equal("node6", node.ExternalId);
                Assert.Equal("Test value 2", (node.Properties[tester.TestSpace][tester.TestView.ExternalId + "/1"]["prop"] as RawPropertyValue<string>).Value);
            }
            finally
            {
                await tester.Write.DataModels.DeleteInstances(ids);
            }

        }

        [Fact]
        public async Task TestCustomResource()
        {
            var resource = new TestResource(tester);
            await resource.UpsertAsync(new[] {
                new SourcedNodeWrite<TestItem> {
                    ExternalId = "node9",
                    Space = tester.TestSpace,
                    Properties = new TestItem {
                        Prop = "test",
                        IntProp = 123
                    }
                }
            }, new UpsertOptions());

            var retrieved = await resource.RetrieveAsync(new[] { new InstanceIdentifierWithType {
                InstanceType = InstanceType.node,
                Space = tester.TestSpace,
                ExternalId = "node9"
            }});
            Assert.Single(retrieved);
            var node = retrieved.First();
            Assert.Equal("test", node.Properties.Prop);
            Assert.Equal(123, node.Properties.IntProp);

            await resource.DeleteAsync(new[] { new InstanceIdentifierWithType {
                InstanceType = InstanceType.node,
                Space = tester.TestSpace,
                ExternalId = "node9"
            }});

            var retrieved2 = await resource.RetrieveAsync(new[] { new InstanceIdentifierWithType {
                InstanceType = InstanceType.node,
                Space = tester.TestSpace,
                ExternalId = "node9"
            }});
            Assert.Empty(retrieved2);
        }
    }

    class TestItem
    {
        public string Prop { get; set; }
        public DirectRelationIdentifier RefProp { get; set; }
        public int? IntProp { get; set; }
    }

    class TestResource : BaseDataModelResource<TestItem>
    {
        public override ViewIdentifier View { get; }

        public TestResource(DataModelsFixture fixture) : base(fixture.Write.DataModels)
        {
            View = fixture.TestView;
        }
    }
}
