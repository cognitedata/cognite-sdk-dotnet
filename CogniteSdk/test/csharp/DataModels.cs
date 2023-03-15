// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk;
using CogniteSdk.Beta.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
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
        public string Prefix { get; private set; }

        protected override void Dispose(bool disposing)
        {
        }

        public override async Task InitializeAsync()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Random random = new Random();
            Prefix = "sdkTest" + new string(Enumerable.Repeat(chars, 5)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            TestSpace = $"{Prefix}Space";

            var testSpace = new SpaceCreate { Space = TestSpace };

            await Write.Beta.DataModels.UpsertSpaces(new[] { testSpace });

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
                Properties = new Dictionary<string, ViewPropertyCreate>
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

            await Write.Beta.DataModels.UpsertContainers(new[] { testContainer });
            await Write.Beta.DataModels.UpsertViews(new[] { testView });

            TestContainer = testContainerIdt;
            TestView = new ViewIdentifier(TestSpace, "TestView", "1");
        }

        public override async Task DisposeAsync()
        {
            if (TestContainer != null) await Write.Beta.DataModels.DeleteContainers(new[] { TestContainer.ContainerId() });
            if (TestView != null) await Write.Beta.DataModels.DeleteViews(new[] { TestView.FDMExternalId() });
            if (TestSpace != null) await Write.Beta.DataModels.DeleteSpaces(new[] { TestSpace });
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
            var created = await tester.Write.Beta.DataModels.UpsertSpaces(new[] { space });
            Assert.Single(created);

            try
            {
                // Update space name
                space.Name = "Test Space Updated";
                var updated = await tester.Write.Beta.DataModels.UpsertSpaces(new[] { space });
                Assert.Single(updated);
                Assert.Equal("Test Space Updated", updated.First().Name);

                // Retrieve space by id
                var retrieved = await tester.Write.Beta.DataModels.RetrieveSpaces(new[] { extid });
                Assert.Single(retrieved);
            }
            finally
            {
                // Delete space
                var deleted = await tester.Write.Beta.DataModels.DeleteSpaces(new[] { extid });
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
                        Properties = new Dictionary<string, ViewPropertyCreate>()
                    }
                }
            };
            var id = new FDMExternalId(extid, tester.TestSpace, "1");

            // Create a data model
            var created = await tester.Write.Beta.DataModels.UpsertDataModels(new[] { model });
            Assert.Single(created);

            try
            {
                // Retrieve a data model
                var retrieved = await tester.Write.Beta.DataModels.RetrieveDataModels(new[] { id }, true);
                Assert.Single(retrieved);
                var retModel = retrieved.First();
                Assert.Equal(2, retModel.Views.Count());
                Assert.True(retModel.Views.Any(view => (view as View).ExternalId == "TestView"));

                // Update a data model
                model.Description = "Test description";
                var updated = await tester.Write.Beta.DataModels.UpsertDataModels(new[] { model });
                Assert.Single(updated);
                Assert.Equal("Test description", updated.First().Description);
            }
            finally
            {
                // Delete the data model
                var deleted = await tester.Write.Beta.DataModels.DeleteDataModels(new[]
                {
                    id
                });
                // Delete the implicitly created view
                await tester.Write.Beta.DataModels.DeleteViews(new[]
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
                Properties = new Dictionary<string, ViewPropertyCreate>
                {
                    { "prop", new ViewPropertyCreate
                    {
                        Container = tester.TestContainer,
                        Name = "Property",
                        ContainerPropertyIdentifier = "prop"
                    } }
                }
            };

            var created = await tester.Write.Beta.DataModels.UpsertViews(new[] { view });
            Assert.Single(created);

            var id = new FDMExternalId(extid, tester.TestSpace, "1");

            try
            {
                // Retrieve a vuew
                var retrieved = await tester.Write.Beta.DataModels.RetrieveViews(new[] { id });
                Assert.Single(retrieved);

                // Update a view
                view.Description = "Test description";
                var updated = await tester.Write.Beta.DataModels.UpsertViews(new[] { view });
                Assert.Single(updated);
                Assert.Equal("Test description", updated.First().Description);
            }
            finally
            {
                // Delete the view
                var deleted = await tester.Write.Beta.DataModels.DeleteViews(new[] { id });
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

            var created = await tester.Write.Beta.DataModels.UpsertContainers(new[] { container });
            Assert.Single(created);

            var id = new ContainerId(extid, tester.TestSpace);

            try
            {
                // Retrieve a container
                var retrieved = await tester.Write.Beta.DataModels.RetrieveContainers(new[] { id });
                Assert.Single(retrieved);

                // Update a container
                container.Description = "Test description";
                var updated = await tester.Write.Beta.DataModels.UpsertContainers(new[] { container });
                Assert.Single(updated);
                Assert.Equal("Test description", updated.First().Description);
            }
            finally
            {
                // Delete the container
                var deleted = await tester.Write.Beta.DataModels.DeleteContainers(new[] { id });
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

            var created = await tester.Write.Beta.DataModels.UpsertInstances(req);
            Assert.Equal(2, created.Count());

            var ids = new[] {
                new InstanceIdentifier(InstanceType.node, tester.TestSpace, "node1"),
                new InstanceIdentifier(InstanceType.node, tester.TestSpace, "node2")
            };

            try
            {
                var retrieved = await tester.Write.Beta.DataModels.RetrieveInstances<StandardInstanceData>(new InstancesRetrieve
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
                var deleted = await tester.Write.Beta.DataModels.DeleteInstances(ids);
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

            var created = await tester.Write.Beta.DataModels.UpsertInstances(req);
            Assert.Equal(2, created.Count());

            var ids = new[] {
                new InstanceIdentifier(InstanceType.edge, tester.TestSpace, "edge1"),
                new InstanceIdentifier(InstanceType.edge, tester.TestSpace, "edge2")
            };

            try
            {
                var retrieved = await tester.Write.Beta.DataModels.RetrieveInstances<StandardInstanceData>(new InstancesRetrieve
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
                var deleted = await tester.Write.Beta.DataModels.DeleteInstances(ids);
                Assert.Equal(2, deleted.Count());
                var deletedNodes = await tester.Write.Beta.DataModels.DeleteInstances(new[]
                {
                    new InstanceIdentifier(InstanceType.node, tester.TestSpace, "node3"),
                    new InstanceIdentifier(InstanceType.node, tester.TestSpace, "node4"),
                    new InstanceIdentifier(InstanceType.node, tester.TestSpace, "node5"),
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

            var created = await tester.Write.Beta.DataModels.UpsertInstances(req);
            Assert.Equal(2, created.Count());

            var ids = new[] {
                new InstanceIdentifier(InstanceType.node, tester.TestSpace, "node6"),
                new InstanceIdentifier(InstanceType.node, tester.TestSpace, "node7")
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
                var queryResult = await tester.Write.Beta.DataModels.QueryInstances<StandardInstanceData>(q);

                Assert.True(queryResult.Items.Any(), JsonSerializer.Serialize(q, Oryx.Cognite.Common.jsonOptions));
                Assert.Equal("res1", queryResult.Items.First().Key);
                Assert.Single(queryResult.Items["res1"]);
                var node = queryResult.Items["res1"].First();
                Assert.Equal("node6", node.ExternalId);
                Assert.Equal("Test value 2", (node.Properties[tester.TestSpace][tester.TestView.ExternalId + "/1"]["prop"] as RawPropertyValue<string>).Value);
            }
            finally
            {
                await tester.Write.Beta.DataModels.DeleteInstances(ids);
            }
            
        }
    }
}
