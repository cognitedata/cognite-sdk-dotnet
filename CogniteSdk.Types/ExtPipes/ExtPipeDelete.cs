using System;
using System.Collections.Generic;
using System.Text;

namespace CogniteSdk
{
    /// <summary>
    /// Delete extraction pipelines
    /// </summary>
    public class ExtPipeDelete : ItemsWithoutCursor<Identity>
    {
        /// <summary>
        /// Ignore IDs and external IDs that are not found. Default: false
        /// </summary>
        public bool? IgnoreUnknownIds { get; set; }
    }
}
