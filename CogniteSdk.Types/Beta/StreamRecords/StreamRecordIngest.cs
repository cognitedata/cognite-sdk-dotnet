// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using CogniteSdk.DataModels;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Stream record to ingest.
    /// </summary>
    public class StreamRecordWrite
    {
        /// <summary>
        /// Id of the space the record belongs to.
        /// </summary>
        public string Space { get; set; }
        /// <summary>
        /// List of source properties to write. The properties are from the container(s) making up this record.
        /// Note that `InstanceData` is abstract, you should generally use `InstanceData[T]`
        /// to assign types to the record item, but since record sources currently only write to
        /// containers, it is usually impossible to assign only a single type to the records.
        /// 
        /// As a fallback, you can use <see cref="StandardInstanceWriteData"/>.
        /// </summary>
        public IEnumerable<InstanceData> Sources { get; set; }
    }

    /// <summary>
    /// Insertion request for records.
    /// </summary>
    public class StreamRecordIngest : ItemsWithoutCursor<StreamRecordWrite>
    {
    }
}