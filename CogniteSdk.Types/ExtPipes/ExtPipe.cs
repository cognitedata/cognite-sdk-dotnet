using System;
using System.Collections.Generic;
using System.Text;

namespace CogniteSdk
{
    /// <summary>
    /// Extraction pipeline.
    /// </summary>
    public class ExtPipe
    {
        /// <summary>
        /// External Id provided by client.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// A server generated ID for the extraction pipeline.
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Name of extraction pipeline.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Description of extraction pipeline.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// DataSet Id.
        /// </summary>
        public long DataSetId { get; set; }
        /// <summary>
        /// Optional list of associated raw tables.
        /// </summary>
        public IEnumerable<ExtPipeRawTable> RawTables { get; set; }
        /// <summary>
        /// Schedule of pipeline. Possible values: "On trigger", "Continuous", a cron expression, or empty.
        /// </summary>
        public string Schedule { get; set; }
        /// <summary>
        /// Optional list of contacts.
        /// </summary>
        public IEnumerable<ExtPipeContact> Contacts { get; set; }
        /// <summary>
        /// Custom, application specific metadata.
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; }
        /// <summary>
        /// Source for extraction pipeline.
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// Long documentation text field.
        /// </summary>
        public string Documentation { get; set; }
        /// <summary>
        /// Time of last successful run. The number of milliseconds since 00:00:00 Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus leap seconds.
        /// </summary>
        public long LastSuccess { get; set; }
        /// <summary>
        /// Time of last failed run. The number of milliseconds since 00:00:00 Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus leap seconds.
        /// </summary>
        public long LastFailure { get; set; }
        /// <summary>
        /// Last failure message.
        /// </summary>
        public string LastMessage { get; set; }
        /// <summary>
        /// Last seen time. The number of milliseconds since 00:00:00 Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus leap seconds.
        /// </summary>
        public long LastSeen { get; set; }
        /// <summary>
        /// Last updated time. The number of milliseconds since 00:00:00 Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus leap seconds.
        /// </summary>
        public long LastUpdatedTime { get; set; }
        /// <summary>
        /// Extraction pipeline creater. Usually user email.
        /// </summary>
        public string CreatedBy { get; set; }
    }
}
