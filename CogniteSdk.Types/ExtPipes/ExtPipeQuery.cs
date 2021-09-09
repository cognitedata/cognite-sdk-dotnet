using System;
using System.Collections.Generic;
using System.Text;

namespace CogniteSdk
{
    /// <summary>
    /// Query for filtering extraction pipelines
    /// </summary>
    public class ExtPipeQuery : CursorQueryBase
    {
        /// <summary>
        /// Extraction pipeline filter
        /// </summary>
        public ExtPipeFilter Filter { get; set; }
    }
}
