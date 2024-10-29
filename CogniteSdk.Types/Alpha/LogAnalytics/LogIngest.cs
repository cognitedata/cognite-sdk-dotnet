using System.Collections.Generic;
using CogniteSdk.DataModels;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// ILA Log item to ingest.
    /// </summary>
    public class LogItem
    {
        /// <summary>
        /// Id of the space the log belongs to.
        /// </summary>
        public string Space { get; set; }
        /// <summary>
        /// List of source properties to write. The properties are from the container(s) making up this log.
        /// Note that `InstanceData` is abstract, you should generally use `InstanceData[T]`
        /// to assign types to the log item, but since log sources currently only write to
        /// containers, it is usually impossible to assign only a single type to the logs.
        /// 
        /// As a fallback, you can use <see cref="StandardInstanceWriteData"/>.
        /// </summary>
        public IEnumerable<InstanceData> Sources { get; set; }
    }

    /// <summary>
    /// Insertion request for ILA logs.
    /// </summary>
    public class LogIngest : ItemsWithoutCursor<LogItem>
    {
    }
}