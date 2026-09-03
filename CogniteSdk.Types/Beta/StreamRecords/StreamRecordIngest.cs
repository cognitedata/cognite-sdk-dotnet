// Copyright 2025 Cognite AS
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
        /// External ID of the record, required.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// Id of the space the record belongs to.
        /// </summary>
        public string Space { get; set; }
        /// <summary>
        /// List of source properties to write. Each source is either a container, in which case the
        /// properties are keyed by container property names, or a record view (a view with
        /// <see cref="UsedFor.record"/>), in which case the properties are keyed by the view's property
        /// names and a single source writes to all of the containers mapped by the view.
        /// Note that `InstanceData` is abstract, you should generally use `InstanceData[T]`
        /// to assign types to the record item, but since sources may span several containers,
        /// it is usually impossible to assign only a single type to the records.
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

    /// <summary>
    /// Delete request for records.
    /// </summary>
    public class StreamRecordDelete : ItemsWithoutCursor<InstanceIdentifier>
    {
    }
}
