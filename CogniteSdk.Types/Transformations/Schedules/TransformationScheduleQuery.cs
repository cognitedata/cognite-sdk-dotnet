using System;
using System.Collections.Generic;
using System.Text;

namespace CogniteSdk
{
    /// <summary>
    /// Query for listing transformation schedules.
    /// </summary>
    public class TransformationScheduleQuery : CursorQueryBase
    {
        /// <summary>
        /// Whether public transformations should be included in the results. Default is true.
        /// </summary>
        public bool? IncludePublic { get; set; }

        /// <inheritdoc />
        public override List<(string, string)> ToQueryParams()
        {
            var list = base.ToQueryParams();
            if (IncludePublic.HasValue)
                list.Add(("includePublic", IncludePublic.ToString()));

            return list;
        }
    }
}
