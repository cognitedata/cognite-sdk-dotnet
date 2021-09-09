using System;
using System.Collections.Generic;
using System.Text;

namespace CogniteSdk
{
    /// <summary>
    /// Query for filtering extraction pipeline runs
    /// </summary>
    public class ExtPipeRunQuery : CursorQueryBase
    {
        /// <summary>
        /// Required filter
        /// </summary>
        public ExtPipeRunFilter Filter { get; set; }
    }
}
