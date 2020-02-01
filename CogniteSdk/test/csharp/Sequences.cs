using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;

using CogniteSdk;
using CogniteSdk.Sequences;

namespace Test.CSharp.Integration
{
    [Collection("TestBase")]
    public class SequencesTests : TestFixture
    {
        [Fact]
        [Trait("Description", "List Sequences with limit is Ok")]
        public async Task ListSequenceWithLimit()
        {
            // Arrange
            var query = new SequenceQueryDto
            {
                Limit = 10
            };

            // Act
            var res = await WriteClient.Sequences.ListAsync(query);

            // Assert
            Assert.True(res.Items.Any(), "Expected at least one sequence");
        }
        [Fact]
        [Trait("Description", "List Sequences with limit and filter is Ok")]
        public async Task ListSequenceWithLimitAndFilter()
        {
            // Arrange
            var query = new SequenceQueryDto
            {
                Limit = 10,
                Filter = new SequenceFilterDto()
                {
                    ExternalIdPrefix = "sdk-test"
                }
            };

            // Act
            var res = await WriteClient.Sequences.ListAsync(query);

            // Assert
            Assert.True(res.Items.Any(), "Expected at least one sequence");
        }
        [Fact]
        [Trait("Description", "Retrieve Sequences by id is Ok")]
        public async Task RetrieveSequenceById()
        {
            // Arrange
            var ids = new List<long>() { 5702374195409554L };

            // Act
            var res = await WriteClient.Sequences.RetrieveAsync(ids);
            var returnedIds = res.Select(sequence => sequence.Id);

            // Assert
            Assert.True(res.Count() == 1, "Expected one sequence");
            Assert.True(returnedIds.Intersect(ids).Count() == returnedIds.Count(), "One of the received Sequence dont match the requested IDs");
        }

        [Fact]
        [Trait("Description", "Create and delete sequence is Ok")]
        public async Task CreateAndDeleteSequenceAsync()
        {
            // Arrange
            var externalIdString = Guid.NewGuid().ToString();
            var columnExternalIdString = Guid.NewGuid().ToString();

            var column = new SequenceColumnWriteDto {
                ExternalId = columnExternalIdString,
                Name = "Create column C# test",
                ValueType = SequenceValueType.DOUBLE
            };
            var sequence = new SequenceWriteDto {
                ExternalId = externalIdString,
                Name = "Create Sequences c# sdk test",
                Description = "Just a test",
                Columns = new List<SequenceColumnWriteDto> { column }
            };
            // Act
            var res = await WriteClient.Sequences.CreateAsync(new List<SequenceWriteDto> { sequence });
            await WriteClient.Sequences.DeleteAsync(new List<string> { externalIdString });

            // Assert
            var resCount = res.Count();
            Assert.True(1 == resCount, $"Expected 1 created sequence but got {resCount}");
            Assert.True(externalIdString == res.First().ExternalId, "Created externalId doesnt match expected");
        }

        [Fact]
        [Trait("Description", "Create and delete rows in sequence is Ok")]
        public async Task CreateAndDeleteRowsInSequenceAsync()
        {
            // Arrange
            var externalIdString = Guid.NewGuid().ToString();
            var columnExternalIdString = Guid.NewGuid().ToString();

            var column = new SequenceColumnWriteDto {
                ExternalId = columnExternalIdString,
                Name = "Create column C# test",
                ValueType = SequenceValueType.DOUBLE
            };

            var sequence = new SequenceWriteDto {
                ExternalId = externalIdString,
                Name = "Create Sequences c# sdk test",
                Description = "Just a test",
                Columns = new List<SequenceColumnWriteDto> { column }
            };

            var data = new SequenceDataWriteDto() {
                Columns = new List<string> { columnExternalIdString },
                Rows = new List<SequenceRowDto>
                {
                    new SequenceRowDto() { RowNumber=1, Values=new List<MultiValue>() { MultiValue.Create(42.0) } }
                },
                ExternalId = externalIdString
            };
            var delete = new SequenceRowDeleteDto()
            {
                ExternalId = externalIdString,
                Rows = new List<long> { 1L }
            };

            // Act
            var res = await WriteClient.Sequences.CreateAsync(new List<SequenceWriteDto> { sequence });

            await WriteClient.Sequences.CreateRowsAsync(new List<SequenceDataWriteDto> { data });
            await WriteClient.Sequences.DeleteRowsAsync(new List<SequenceRowDeleteDto> { delete });
            await WriteClient.Sequences.DeleteAsync(new List<string>() { externalIdString });

            // Assert
            var resCount = res.Count();
            Assert.True(1 == resCount, $"Expected 1 created sequence but got {resCount}");
            Assert.True(externalIdString == res.First().ExternalId, "Created externalId doesnt match expected");
        }

        [Fact]
        [Trait("Description", "List sequence rows")]
        public async Task ListRowsAsync()
        {
            // Arrange
            var rowQuery = new SequenceRowQueryDto
            {
                Limit = 10,
                ExternalId = "sdk-test"
            };

            // Act
            var res = await WriteClient.Sequences.ListRowsAsync(rowQuery);

            // Assert
            Assert.True(res.Columns.Any());
            Assert.Equal("sdk-test", res.ExternalId);
            Assert.Equal("sdk-test-column", res.Columns.First().Name);
        }
    }
}