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
        public void ListSequenceWithLimit()
        {
            // Arrange
            var limit = 10;
            var option = SequenceQuery.Limit(limit);

            // Act
            var res = WriteClient.Sequences.ListAsync(new List<SequenceQuery> { option }, new List<SequenceFilter>());

            // Assert
            Assert.True(res.Result.Items.Count() > 0, "Expected atleast one sequence");
        }
        [Fact]
        [Trait("Description", "List Sequences with limit and filter is Ok")]
        public void ListSequenceWithLimitAndFilter()
        {
            // Arrange
            var limit = 10;
            var option = SequenceQuery.Limit(limit);
            var filter = SequenceFilter.ExternalIdPrefix("sdk-test");

            // Act
            var res = WriteClient.Sequences.ListAsync(new List<SequenceQuery> { option }, new List<SequenceFilter> { filter });

            // Assert
            Assert.True(res.Result.Items.Count() > 0, "Expected atleast one sequence");
        }
    }
}