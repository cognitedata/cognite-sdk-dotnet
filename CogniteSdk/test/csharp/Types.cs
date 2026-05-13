using System.Collections.Generic;
using CogniteSdk.DataModels;
using Xunit;

namespace Test.CSharp
{
    public class TypesTests
    {
        [Fact]
        public void TestViewIdentifierCustomOpertators()
        {
            var viewA = new ViewIdentifier("a", "b", "c");
            var view_duplicate = new ViewIdentifier("a", "b", "c");
            var viewAA = new ViewIdentifier("aa", "b", "c");
            var viewBB = new ViewIdentifier("a", "bb", "c");
            var viewCC = new ViewIdentifier("a", "b", "cc");
            Assert.Equal(viewA, view_duplicate, ViewIdentifier.ValueTypeEqualityComparer);
            Assert.False(ReferenceEquals(viewA, view_duplicate));
            Assert.NotEqual(viewA, viewAA, ViewIdentifier.ValueTypeEqualityComparer);
            Assert.NotEqual(viewA, viewBB, ViewIdentifier.ValueTypeEqualityComparer);
            Assert.NotEqual(viewA, viewCC, ViewIdentifier.ValueTypeEqualityComparer);
            Assert.False(ViewIdentifier.ValueTypeEqualityComparer.Equals(viewA, null));
            Assert.False(ViewIdentifier.ValueTypeEqualityComparer.Equals(null, viewA));
            Assert.Equal((ViewIdentifier)null, null, ViewIdentifier.ValueTypeEqualityComparer);
            Assert.False(ViewIdentifier.ValueTypeEqualityComparer.Equals(null, new ViewIdentifier()));

            var hashset = new HashSet<ViewIdentifier>([viewA, view_duplicate, viewAA, viewBB, viewCC], ViewIdentifier.ValueTypeEqualityComparer);
            Assert.Equal(4, hashset.Count);
            Assert.Contains(viewAA, hashset);
            Assert.Contains(viewBB, hashset);
            Assert.Contains(viewCC, hashset);
            Assert.Contains(viewA, hashset);
            Assert.Equal(ViewIdentifier.ValueTypeEqualityComparer.GetHashCode(viewA), ViewIdentifier.ValueTypeEqualityComparer.GetHashCode(view_duplicate));
            Assert.NotEqual(ViewIdentifier.ValueTypeEqualityComparer.GetHashCode(viewA), ViewIdentifier.ValueTypeEqualityComparer.GetHashCode(viewAA));
            Assert.NotEqual(ViewIdentifier.ValueTypeEqualityComparer.GetHashCode(viewA), ViewIdentifier.ValueTypeEqualityComparer.GetHashCode(new ViewIdentifier()));

            Assert.Equal("a.b.c", viewA.ToString());
        }
    }
}
