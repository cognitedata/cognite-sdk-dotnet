using System.Collections.Generic;

namespace CogniteSdk.Types.Sequences
{
    public class Row
    {
        public long RowNumber { get; set; }

        public IEnumerable<RowValue> Values { get; set; }
    }
}
