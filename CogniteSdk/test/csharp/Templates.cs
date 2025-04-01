// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CogniteSdk;
using CogniteSdk.Beta;
using Xunit;

namespace Test.CSharp.Integration
{
    public class TemplatesFixture : TestFixture, IAsyncLifetime
    {
        public Client Read => ReadClient;
        public Client Write => WriteClient;
        public TemplateGroup TestGroup { get; private set; }
        public TemplateVersion TestVersion { get; private set; }

        protected override void Dispose(bool disposing)
        {
        }

        public override async Task InitializeAsync()
        {
            var extid = Guid.NewGuid().ToString();

            var testGroup = new TemplateGroupCreate
            {
                Description = "Test group",
                ExternalId = extid
            };
            var result = await Write.Beta.Templates.CreateAsync(new[] { testGroup });
            TestGroup = result.First();

            var testVersion = new TemplateVersionCreate
            {
                ConflictMode = TemplateGroupConflictMode.Force,
                Schema = @"
type MyType @template {
  myStringField: String
  myIntField: Int
  myFloatField: Float
  myBooleanField: Boolean
  child: NonTemplateType
}

type NonTemplateType { # Non template type
  textField: String
}"
            };
            TestVersion = await Write.Beta.Templates.UpsertVersionAsync(extid, testVersion);
        }

        public override async Task DisposeAsync()
        {
            if (TestGroup == null) return;
            await Write.Beta.Templates.DeleteAsync(new[] { TestGroup.ExternalId }, true);
        }
    }


    public class TemplatesTest : IClassFixture<TemplatesFixture>
    {
        private TemplatesFixture tester;
        public TemplatesTest(TemplatesFixture tester)
        {
            this.tester = tester;
        }

        [Fact]
        public async Task TestCreateUpsertRetrieveDeleteGroup()
        {
            // Arrange
            var extid = Guid.NewGuid().ToString();
            var testGroup = new TemplateGroupCreate
            {
                Description = "Test group",
                ExternalId = extid
            };

            // Act
            var createResult = await tester.Write.Beta.Templates.CreateAsync(new[] { testGroup });
            testGroup.Description = "Test group 2";
            var upsertResult = await tester.Write.Beta.Templates.UpsertAsync(new[] { testGroup });
            var retrieveResult = await tester.Write.Beta.Templates.RetrieveAsync(new[] { extid }, true);
            await tester.Write.Beta.Templates.DeleteAsync(new[] { extid }, true);

            // Assert
            Assert.Equal("Test group", createResult.First().Description);
            Assert.Equal("Test group 2", upsertResult.First().Description);
            Assert.Equal("Test group 2", retrieveResult.First().Description);
        }

        [Fact]
        public async Task InsertDeleteVersion()
        {
            // Arrange
            var version = new TemplateVersionCreate
            {
                ConflictMode = TemplateGroupConflictMode.Update,
                Schema = @"
type MyType @template {
  myStringField: String
}
"
            };

            // Act
            var insertResult = await tester.Write.Beta.Templates.UpsertVersionAsync(tester.TestGroup.ExternalId, version);
            await tester.Write.Beta.Templates.DeleteVersionsAsync(tester.TestGroup.ExternalId, insertResult.Version);

            // Assert
            Assert.True(insertResult.Version > 1);
            Assert.Equal(insertResult.Schema, version.Schema);
        }

        class QueryResult
        {
            public string MyStringField { get; set; }
            public int MyIntField { get; set; }
        }

        class QueryResultWrapper
        {
            public ItemsWithCursor<QueryResult> MyTypeQuery { get; set; }
        }

        [Fact]
        public async Task CreateUpsertRetrieveQueryDeleteInstance()
        {
            // Arrange
            var extid = Guid.NewGuid().ToString();
            var instance = new TemplateInstanceCreate
            {
                ExternalId = extid,
                TemplateName = "MyType",
                FieldResolvers = new Dictionary<string, BaseFieldResolver>
                {
                    { "myStringField", new ConstantFieldResolver { Value = JsonDocument.Parse("\"some-value\"").RootElement } }
                }
            };
            var groupId = tester.TestGroup.ExternalId;
            var version = tester.TestVersion.Version;

            // Act
            var createResult = await tester.Write.Beta.Templates.CreateInstancesAsync(groupId, version, new[] { instance });
            instance.FieldResolvers["myIntField"] = new ConstantFieldResolver { Value = JsonDocument.Parse("123").RootElement };
            var upsertResult = await tester.Write.Beta.Templates.UpsertInstancesAsync(groupId, version, new[] { instance });
            var retrieveResult = await tester.Write.Beta.Templates.RetrieveInstancesAsync(groupId, version, new[] { extid }, true);

            try
            {
                var queryResult = await tester.Write.Beta.Templates.QueryAsync<QueryResultWrapper>(groupId, version, new GraphQlQuery
                {
                    Query = @"
{
  myTypeQuery {
    items {
      myStringField
      myIntField
    }
  }
}
"
                });
                Assert.Equal(123, queryResult.Data.MyTypeQuery.Items.First().MyIntField);
                Assert.Equal("some-value", queryResult.Data.MyTypeQuery.Items.First().MyStringField);
            }
            finally
            {
                await tester.Write.Beta.Templates.DeleteInstancesAsync(groupId, version, new[] { extid }, true);
            }


            // Assert
            Assert.Single(createResult.First().FieldResolvers, kvp => kvp.Value != null);
            Assert.Equal(2, upsertResult.First().FieldResolvers.Where(kvp => kvp.Value != null).Count());
            Assert.Equal(2, retrieveResult.First().FieldResolvers.Where(kvp => kvp.Value != null).Count());
        }

        [Fact]
        public async Task CreateUpsertDeleteView()
        {
            // Arrange
            var extid = Guid.NewGuid().ToString();
            var view = new TemplateViewCreate<object>
            {
                ExternalId = extid,
                Source = new TemplateViewSource<object>
                {
                    Filter = new object(),
                    Type = "assets"
                }
            };
            var groupId = tester.TestGroup.ExternalId;
            var version = tester.TestVersion.Version;

            // Act
            var createResult = await tester.Write.Beta.Templates.CreateViewsAsync(groupId, version, new[] { view });
            view.Source.Type = "events";
            var upsertResult = await tester.Write.Beta.Templates.UpsertViewsAsync(groupId, version, new[] { view });
            await tester.Write.Beta.Templates.DeleteAsync(new[] { extid }, true);

            // Assert
            Assert.Equal("assets", createResult.First().Source.Type);
            Assert.Equal("events", upsertResult.First().Source.Type);
        }
    }
}
