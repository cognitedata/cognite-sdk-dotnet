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
            Assert.Equal(viewA, view_duplicate);
            Assert.False(ReferenceEquals(viewA, view_duplicate));
            Assert.NotEqual(viewA, viewAA);
            Assert.NotEqual(viewA, viewBB);
            Assert.NotEqual(viewA, viewCC);
            Assert.False(viewA == null);
            Assert.False(null == viewA);
            Assert.False(new ViewIdentifier() == null);

            var hashset = new HashSet<ViewIdentifier>([viewA, view_duplicate, viewAA, viewBB, viewCC]);
            Assert.Equal(4, hashset.Count);
            Assert.Contains(viewAA, hashset);
            Assert.Contains(viewBB, hashset);
            Assert.Contains(viewCC, hashset);
            Assert.Contains(viewA, hashset);
            Assert.Equal(viewA.GetHashCode(), view_duplicate.GetHashCode());
            Assert.NotEqual(viewA.GetHashCode(), viewAA.GetHashCode());
            Assert.NotEqual(viewA.GetHashCode(), new ViewIdentifier().GetHashCode());
        }
    }
}
