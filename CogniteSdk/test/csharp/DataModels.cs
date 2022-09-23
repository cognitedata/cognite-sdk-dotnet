﻿using CogniteSdk.Beta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Test.CSharp.Integration;
using Xunit;

namespace CogniteSdk.Test
{
    public class DataModelsFixture : TestFixture, IAsyncLifetime
    {
        public Client Read => ReadClient;
        public Client Write => WriteClient;

        public string TestSpace { get; private set; }
        public string TestModel { get; private set; }
        public string TestEdgeModel { get; private set; }
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

            var extid = "TestSpace";

            var testSpace = new Space { ExternalId = extid };

            await Write.Beta.DataModels.CreateSpaces(new[] { testSpace });

            var testModel = new ModelCreate
            {
                AllowEdge = false,
                AllowNode = true,
                ExternalId = "TestModel",
                Properties = new Dictionary<string, ModelProperty>
                {
                    { "prop", new ModelProperty
                    {
                        Type = "text",
                        Nullable = true
                    } },
                    { "refProp", new ModelProperty
                    {
                        Type = "direct_relation",
                        Nullable = true,
                        TargetModel = ModelIdentifier.Node
                    } },
                    { "intProp", new ModelProperty
                    {
                        Type = "int64",
                        Nullable = false
                    } }
                }
            };

            var testEdgeModel = new ModelCreate
            {
                AllowEdge = true,
                AllowNode = false,
                ExternalId = "TestEdgeModel",
                Properties = new Dictionary<string, ModelProperty>
                {
                    { "prop", new ModelProperty
                    {
                        Type = "text",
                        Nullable = true
                    } },
                    { "refProp", new ModelProperty
                    {
                        Type = "direct_relation",
                        Nullable = true,
                        TargetModel = ModelIdentifier.Node
                    } },
                    { "intProp", new ModelProperty
                    {
                        Type = "int64",
                        Nullable = false
                    } }
                }
            };

            await Write.Beta.DataModels.ApplyModels(new[] { testModel, testEdgeModel }, extid);

            TestSpace = extid;
            TestModel = "TestModel";
            TestEdgeModel = "TestEdgeModel";
        }

        public override Task DisposeAsync()
        {
            return Task.CompletedTask;
        }
    }
    public class DataModelsTest : IClassFixture<DataModelsFixture>
    {
        private readonly DataModelsFixture tester;
        public DataModelsTest(DataModelsFixture tester)
        {
            this.tester = tester;
        }

        /* Currently cannot delete spaces
        [Fact]
        public async Task TestCreateRetrieveDeleteSpaces()
        {
            var extid = Guid.NewGuid().ToString();
            var space = new Space { ExternalId = extid };

            var created = await tester.Write.Beta.DataModels.CreateSpaces(new[] { space });
            Assert.Single(created);

            var retrieved = await tester.Write.Beta.DataModels.RetrieveSpaces(new[] { extid });
            Assert.Single(retrieved);
            Assert.Equal(extid, retrieved.First().ExternalId);

            var listed = await tester.Write.Beta.DataModels.ListSpaces();
            Assert.True(listed.Count() >= 2);

            await tester.Write.Beta.DataModels.DeleteSpaces(new[] { extid });
        }
        */

        /* Also cannot delete models yet...
         * [Fact]
        public async Task TestCreateRetrieveDeleteModels()
        {
            var extid = tester.Prefix + "model1";

            var model = new ModelCreate
            {
                AllowEdge = false,
                AllowNode = true,
                ExternalId = extid,
                Properties = new Dictionary<string, ModelProperty>
                {
                    { "prop1", new ModelProperty
                    {
                        Type = "text",
                        Nullable = true
                    } },
                    { "refProp", new ModelProperty
                    {
                        Type = "direct_relation",
                        Nullable = true,
                        TargetModel = ModelIdentifier.Node
                    } }
                }
            };

            var created = await tester.Write.Beta.DataModels.ApplyModels(new[] { model }, tester.TestSpace);
            Assert.True(created.Any());

            // Does not work?
            // var retrieved = await tester.Write.Beta.DataModels.RetrieveModels(new[] { extid }, tester.TestSpace);
            // Assert.True(retrieved.Any());

            var listed = await tester.Write.Beta.DataModels.ListModels(tester.TestSpace);
            Assert.True(listed.Count() >= 1);

            await tester.Write.Beta.DataModels.DeleteModels(new[] { extid }, tester.TestSpace);
        } */

        private class TestNodeType : BaseNode
        {
            public string Prop { get; set; }
            public DirectRelationIdentifier RefProp { get; set; }
            public long IntProp { get; set; }
        }

        [Fact]
        public async Task TestCreateRetrieveDeleteNodes()
        {
            var model = new ModelIdentifier(tester.TestSpace, tester.TestModel);
            var id = $"{tester.Prefix}node1";
            var node = new TestNodeType
            {
                ExternalId = id,
                IntProp = 123,
                Prop = "Some property",
            };

            var created = await tester.Write.Beta.DataModels.IngestNodes(new NodeIngestRequest<TestNodeType>
            {
                Items = new[] { node },
                Model = model,
                SpaceExternalId = tester.TestSpace
            });
            Assert.Single(created);

            var retrieved = await tester.Write.Beta.DataModels.RetrieveNodes<TestNodeType>(new RetrieveNodesRequest
            {
                Items = new[] { new CogniteExternalId(id) },
                Model = model,
                SpaceExternalId = tester.TestSpace
            });
            Assert.Single(retrieved.Items);
            Assert.Equal("Some property", retrieved.Items.First().Prop);

            var filtered = await tester.Write.Beta.DataModels.FilterNodes<TestNodeType>(new NodeFilterQuery
            {
                Model = model,
                SpaceExternalId = tester.TestSpace,
                Filter = new OrFilter
                {
                    Or = new[]
                    {
                        new EqualsFilter
                        {
                            Property = new PropertyIdentifier(model, "externalId"),
                            Value = JsonDocument.Parse($"\"{id}\"").RootElement
                        },
                        new EqualsFilter
                        {
                            Property = new PropertyIdentifier(model, "prop"),
                            Value = JsonDocument.Parse("\"Some other value\"").RootElement
                        }
                    }
                }
            });
            Assert.Single(filtered.Items);

            await tester.Write.Beta.DataModels.DeleteNodes(new[] { id }, tester.TestSpace);
        }

        private class TestEdgeType : BaseEdge
        {
            public string Prop { get; set; }
            public DirectRelationIdentifier RefProp { get; set; }
            public long IntProp { get; set; }
        }

        [Fact]
        public async Task TestCreateRetrieveDeleteEdges()
        {
            var model = new ModelIdentifier(tester.TestSpace, tester.TestEdgeModel);
            var id = $"{tester.Prefix}edge1";
            var edge = new TestEdgeType
            {
                ExternalId = id,
                StartNode = new DirectRelationIdentifier(tester.TestSpace, "Source"),
                EndNode = new DirectRelationIdentifier(tester.TestSpace, "Target"),
                IntProp = 123,
                Prop = "Prop",
                Type = new DirectRelationIdentifier(tester.TestSpace, "Type")
            };

            var created = await tester.Write.Beta.DataModels.IngestEdges(new EdgeIngestRequest<TestEdgeType>
            {
                Items = new[] { edge },
                AutoCreateEndNodes = true,
                AutoCreateStartNodes = true,
                SpaceExternalId = tester.TestSpace,
                Model = model
            });

            var retrieved = await tester.Write.Beta.DataModels.RetrieveEdges<TestEdgeType>(new RetrieveNodesRequest
            {
                Items = new[] { new CogniteExternalId(id) },
                Model = model,
                SpaceExternalId = tester.TestSpace
            });
            Assert.Single(retrieved.Items);
            Assert.Equal("Prop", retrieved.Items.First().Prop);
            var filter = new AndFilter
            {
                And = new[]
                {
                    new EqualsFilter
                    {
                        Property = new PropertyIdentifier(model, "prop"),
                        Value = JsonDocument.Parse("\"Prop\"").RootElement
                    },
                    new EqualsFilter
                    {
                        Property = new PropertyIdentifier(ModelIdentifier.Edge, "externalId"),
                        Value = JsonDocument.Parse($"\"{id}\"").RootElement
                    }
                }
            };
            var request = new NodeFilterQuery
            {
                Filter = filter,
                SpaceExternalId = tester.TestSpace,
                Model = model,
            };
            // throw new Exception(JsonSerializer.Serialize(request, Oryx.Cognite.Common.jsonOptions));

            var filtered = await tester.Write.Beta.DataModels.FilterEdges<TestEdgeType>(request);
            Assert.Single(filtered.Items);
            

            await tester.Write.Beta.DataModels.DeleteEdges(new[] { id }, tester.TestSpace);
        }
    }
}
