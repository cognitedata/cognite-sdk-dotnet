using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;

using Xunit;

using CogniteSdk;
using CogniteSdk.Raw;
using CogniteSdk.Sequences;
using CogniteSdk.Sequences.Rows;

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

            var column = new SequenceColumnDto {
                ExternalId = columnExternalIdString,
                Name = "Create column C# test",
                ValueType = CogniteSdk.Sequences.ValueType.DOUBLE
            };
            var sequence = new SequenceWriteDto {
                ExternalId = externalIdString,
                Name = "Create Sequences c# sdk test",
                Description = "Just a test",
                Columns = new List<SequenceColumnDto> { column }
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

            var column = new SequenceColumnDto {
                ExternalId = columnExternalIdString,
                Name = "Create column C# test",
                ValueType = CogniteSdk.Sequences.ValueType.DOUBLE
            };

            var sequence = new SequenceWriteDto {
                ExternalId = externalIdString,
                Name = "Create Sequences c# sdk test",
                Description = "Just a test",
                Columns = new List<SequenceColumnDto> { column }
            };

            var data = new SequenceDataWriteEntity() {
                Columns = new List<string> { columnExternalIdString },
                Rows = new List<RowEntity> { new RowEntity() { RowNumber=1, Values=new List<RowValue>() { RowValue.Double(42.0) } }},
                Id = Identity.ExternalId(externalIdString)
            };
            var delete = new SequenceDataDeleteEntity() { Id = Identity.ExternalId(externalIdString), Rows = new List<Int64> { 1L } };

            // Act
            var res = await WriteClient.Sequences.CreateAsync(new List<SequenceEntity>() { sequence });

            await WriteClient.Sequences.InsertRowsAsync(new List<SequenceDataWriteEntity> { data });
            await WriteClient.Sequences.DeleteRowsAsync(new List<SequenceDataDeleteEntity> { delete });
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
            var rowQuery = new List<RowQueryDto>
            {
                new RowQueryDto { Limit = 10 }
            };
            var externalId = Identity.ExternalId("sdk-test");

            // Act
            var res = await WriteClient.Sequences.ListRowsAsync(externalId, rowQuery);

            // Assert
            Assert.True(res.Columns.Count() > 0);
            Assert.Equal("sdk-test", res.ExternalId);
            Assert.Equal("sdk-test-column", res.Columns.First().Name);
        }
    }
}