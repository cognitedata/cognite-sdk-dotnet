using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;

using Xunit;

using CogniteSdk;
using CogniteSdk.Sequences;

namespace Test.CSharp.Integration
{
    [Collection("TestBase")]
    public class Sequences : TestFixture
    {
        [Fact]
        [Trait("Description", "List Sequences with limit is Ok")]
        public async Task ListSequenceWithLimit()
        {
            // Arrange
            var limit = 10;
            var option = SequenceQuery.Limit(limit);

            // Act
            var res = await WriteClient.Sequences.ListAsync(new List<SequenceQuery> { option }, new List<SequenceFilter>());

            // Assert
            Assert.True(res.Items.Count() > 0, "Expected atleast one sequence");
        }
        [Fact]
        [Trait("Description", "List Sequences with limit and filter is Ok")]
        public async Task ListSequenceWithLimitAndFilter()
        {
            // Arrange
            var limit = 10;
            var option = SequenceQuery.Limit(limit);
            var filter = SequenceFilter.ExternalIdPrefix("sdk-test");

            // Act
            var res = await WriteClient.Sequences.ListAsync(new List<SequenceQuery> { option }, new List<SequenceFilter> { filter });

            // Assert
            Assert.True(res.Items.Count() > 0, "Expected atleast one sequence");
        }
        [Fact]
        [Trait("Description", "Retrieve Sequences by id is Ok")]
        public async Task RetrieveSequenceById()
        {
            // Arrange
            var id = new List<long>() { 5702374195409554L };

            // Act
            var res = await WriteClient.Sequences.GetByIdsAsync(id);
            var returnedIds = res.Select(sequence => sequence.Id);

            // Assert
            Assert.True(res.Count() == 1, "Expected one sequence");
            Assert.True(returnedIds.Intersect(id).Count() == returnedIds.Count(), "One of the received Sequence dont match the requested IDs");
        }
        [Fact]
        [Trait("Description", "Create and delete sequence sequence is Ok")]
        public async Task CreateAndDeleteSequenceAsync() {
            // Arrange
            var externalIdString = Guid.NewGuid().ToString();
            var columnExternalIdString = Guid.NewGuid().ToString();
            var column = new ColumnEntity();
            column.ExternalId = columnExternalIdString;
            column.Name = "Create column C# test";
            column.ValueType = CogniteSdk.Sequences.ValueType.Double;
            var sequence = new SequenceEntity();
            sequence.ExternalId = externalIdString;
            sequence.Name = "Create Sequences c# sdk test";
            sequence.Description = "Just a test";
            sequence.Columns = new List<ColumnEntity>() { column };


            // Act
            var res = await WriteClient.Sequences.CreateAsync(new List<SequenceEntity>() { sequence });
            await WriteClient.Sequences.DeleteAsync(new List<string>() { externalIdString });

            // Assert
            var resCount = res.Count();
            Assert.True(1 == resCount, $"Expected 1 created sequence but got {resCount}");
            Assert.True(externalIdString == res.First().ExternalId, "Created externalId doesnt match expected");
        }
    }
}