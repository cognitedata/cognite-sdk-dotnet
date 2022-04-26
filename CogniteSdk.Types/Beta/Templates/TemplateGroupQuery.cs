using System;
using System.Collections.Generic;
using System.Text;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Query for fetching template groups
    /// </summary>
    public class TemplateGroupQuery : CursorQueryBase
    {
        /// <summary>
        /// Optional filter
        /// </summary>
        public TemplateGroupFilter Filter { get; set; }
    }
}
