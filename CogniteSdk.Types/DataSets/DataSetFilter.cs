using System;
using System.Collections.Generic;
using System.Text;

namespace CogniteSdk
{
    /// <summary>
    /// The data set filter class.
    /// </summary>
    public class DataSetFilter
    {
        /// <summary>
        /// Custom, application specific metadata. String key -> String value.
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; }
        /// <summary>
        /// Range between two timestamps.
        /// </summary>
        public TimeRange CreatedTime { get; set; }
        /// <summary>
        /// Range between two timestamps.
        /// </summary>
        public TimeRange LastUpdatedTime { get; set; }
        /// <summary>
        /// Filter by this (case-sensitive) prefix for the external ID.
        /// </summary>
        public string ExternalIdPrefix { get; set; }
        /// <summary>
        /// Whether the data set is write protected.
        /// </summary>
        public bool WriteProtected { get; set; }
    }
}
