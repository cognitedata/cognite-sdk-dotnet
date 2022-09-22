using CogniteSdk.Beta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        protected override void Dispose(bool disposing)
        {
        }

        public override async Task InitializeAsync()
        {
            // var extid = Guid.NewGuid().ToString();
            var extid = "TestSpace";

            var testSpace = new Space { ExternalId = extid };

            await Write.Beta.DataModels.CreateSpaces(new[] { testSpace });

            TestSpace = extid;
        }

        public override async Task DisposeAsync()
        {
            if (TestSpace == null) return;
            await Write.Beta.DataModels.DeleteSpaces(new[] { TestSpace });
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

        [Fact]
        public async Task TestCreateRetrieveDeleteModels()
        {
            var extid = Guid.NewGuid().ToString();

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
                        Nullable = false,
                        TargetModel = ModelIdentifier.Node
                    } }
                }
            };
            var created = await tester.Write.Beta.DataModels.ApplyModels(new[] { model }, tester.TestSpace);
            Assert.Single(created);

            var retrieved = await tester.Write.Beta.DataModels.RetrieveModels(new[] { extid }, tester.TestSpace);
            Assert.Single(retrieved);

            var listed = await tester.Write.Beta.DataModels.ListModels(tester.TestSpace);
            Assert.True(listed.Count() >= 1);

            await tester.Write.Beta.DataModels.DeleteModels(new[] { extid }, tester.TestSpace);
        }
    }
}
