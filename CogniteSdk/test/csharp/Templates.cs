using CogniteSdk;
using CogniteSdk.Beta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
