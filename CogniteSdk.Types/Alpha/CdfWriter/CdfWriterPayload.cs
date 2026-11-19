// Copyright 2026 Cognite AS
// SPDX-License-Identifier: Apache-2.0

#nullable enable

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// Request to send items to the CDF writer, grouped by task, emitted resource and operation.
    /// The CDF writer resolves the mapping bound to each (task, resource), applies it, and forwards
    /// the result to CDF.
    /// </summary>
    public class CdfWriterPayloadRequest
    {
        /// <summary>
        /// External ID of the integration.
        /// </summary>
        public string? IntegrationId { get; set; }
        /// <summary>
        /// One entry per task. Task IDs must be unique within the request.
        /// </summary>
        public IEnumerable<CdfWriterTaskData>? Data { get; set; }
    }

    /// <summary>
    /// Items emitted by one task.
    /// </summary>
    public class CdfWriterTaskData
    {
        /// <summary>
        /// Creates an empty task data item.
        /// </summary>
        public CdfWriterTaskData()
        {
        }

        /// <summary>
        /// Creates a task data item with the given operations. The dictionary is copied.
        /// </summary>
        /// <param name="resources">Map from emitted resource to its operations</param>
        public CdfWriterTaskData(Dictionary<string, CdfWriterResourceOperations>? resources)
        {
            Resources = resources is null ? null : new Dictionary<string, CdfWriterResourceOperations>(resources);
        }

        /// <summary>
        /// Name of a task declared in <see cref="CdfWriterSetupRequest.Tasks"/>.
        /// </summary>
        public string? TaskId { get; set; }
        /// <summary>
        /// Map from emitted resource, for example "CogniteTimeseries", to its operations.
        /// </summary>
        public Dictionary<string, CdfWriterResourceOperations>? Resources { get; private set; }

        /// <summary>
        /// Adds the operations for an emitted resource.
        /// </summary>
        /// <param name="resource">Emitted resource, for example "CogniteTimeseries"</param>
        /// <param name="operations">Operations to send for the resource</param>
        /// <exception cref="ArgumentNullException"><paramref name="resource"/> or <paramref name="operations"/> is null</exception>
        public void AddResource(string resource, CdfWriterResourceOperations operations)
        {
            if (resource is null) throw new ArgumentNullException(nameof(resource));
            if (operations is null) throw new ArgumentNullException(nameof(operations));
            Resources ??= new Dictionary<string, CdfWriterResourceOperations>();
            Resources[resource] = operations;
        }
    }

    /// <summary>
    /// Operations on one emitted resource. Leave operations without items as null.
    /// Which operations a resource accepts is decided by the CDF writer: <c>datapoints</c> only exists
    /// on "CogniteTimeseries", and "CogniteAsset" and "edges" only accept <c>delete</c>.
    /// </summary>
    public class CdfWriterResourceOperations
    {
        /// <summary>
        /// Node upserts.
        /// </summary>
        public IEnumerable<CdfWriterUpsertItem>? Upsert { get; set; }
        /// <summary>
        /// Data points, grouped by series.
        /// </summary>
        public IEnumerable<CdfWriterDatapointsItem>? Datapoints { get; set; }
        /// <summary>
        /// Hard deletes.
        /// </summary>
        public IEnumerable<CdfWriterDeleteItem>? Delete { get; set; }
    }

    /// <summary>
    /// A node upsert, emitted without a space. The CDF writer routes it to a space.
    /// </summary>
    public class CdfWriterUpsertItem
    {
        /// <summary>
        /// External ID of the node.
        /// </summary>
        public string? ExternalId { get; set; }
        /// <summary>
        /// Optional node type, a direct relation. Either a bare external ID as a string, which the CDF writer
        /// expands to a full reference, or an <see cref="DataModels.InstanceIdentifier"/>, which is passed through unchanged.
        /// </summary>
        public object? Type { get; set; }
        /// <summary>
        /// Properties in the resource's emitted view. Direct relations may be bare external IDs.
        /// Serialized as-is.
        /// </summary>
        public object? Properties { get; set; }
    }

    /// <summary>
    /// Data points for one series.
    /// </summary>
    public class CdfWriterDatapointsItem
    {
        /// <summary>
        /// External ID of the series. The CDF writer routes it to an instance ID.
        /// </summary>
        public string? ExternalId { get; set; }
        /// <summary>
        /// Type of every data point in this item: <c>numeric</c>, <c>string</c> or <c>state</c>.
        /// State requires a mapping with the beta API version.
        /// </summary>
        public string? Type { get; set; }
        /// <summary>
        /// Data points. Must not be empty.
        /// </summary>
        public IEnumerable<CdfWriterDatapoint>? Datapoints { get; set; }
    }

    /// <summary>
    /// A single data point.
    /// </summary>
    public class CdfWriterDatapoint
    {
        /// <summary>
        /// Milliseconds since the Unix epoch.
        /// </summary>
        public long Timestamp { get; set; }
        /// <summary>
        /// Value, matching the containing item's type: a double for numeric, a string for string,
        /// and a long for state. Always serialized, a missing or <see cref="MultiValue.Null"/> value is sent as null,
        /// which requires a status that permits missing values.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public MultiValue? Value { get; set; }
        /// <summary>
        /// State items only. Optional string value, independent of <see cref="Value"/>.
        /// </summary>
        public string? StringValue { get; set; }
        /// <summary>
        /// Status code. Omitted means Good.
        /// </summary>
        public StatusCode? Status { get; set; }
    }

    /// <summary>
    /// A hard delete, emitted without a space.
    /// </summary>
    public class CdfWriterDeleteItem
    {
        /// <summary>
        /// External ID of the instance.
        /// </summary>
        public string? ExternalId { get; set; }
        /// <summary>
        /// <c>node</c> or <c>edge</c>. Omitted means node. Required for deletes under "edges".
        /// </summary>
        public string? InstanceType { get; set; }
    }

    /// <summary>
    /// Response from the CDF writer payload endpoint. Returned both when every CDF call succeeded,
    /// and when at least one failed (HTTP 207). Check <see cref="CdfWriterOperationResult.Status"/> of each operation.
    /// </summary>
    public class CdfWriterPayloadResponse
    {
        /// <summary>
        /// Results per task, in the same order as the request.
        /// </summary>
        public IEnumerable<CdfWriterTaskDataResult>? Data { get; set; }
    }

    /// <summary>
    /// Results for one task.
    /// </summary>
    public class CdfWriterTaskDataResult
    {
        /// <summary>
        /// Task ID from the request.
        /// </summary>
        public string? TaskId { get; set; }
        /// <summary>
        /// Map from emitted resource to operation to result, with the same keys as the request.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Setter is required so System.Text.Json can populate the response")]
        public Dictionary<string, Dictionary<string, CdfWriterOperationResult>>? Resources { get; set; }
    }

    /// <summary>
    /// Result of the CDF call made for one (task, resource, operation).
    /// </summary>
    public class CdfWriterOperationResult
    {
        /// <summary>
        /// Mapping that was applied.
        /// </summary>
        public string? MappingId { get; set; }
        /// <summary>
        /// HTTP status from CDF. 504 on timeout, 502 on connection error.
        /// Null when every item was skipped and no call was made.
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// CDF response body, unchanged. Null when no call was made.
        /// </summary>
        public JsonElement? Body { get; set; }
        /// <summary>
        /// Items that did not exist before the write. Only with merge mode <c>preserve</c>.
        /// </summary>
        public int? Created { get; set; }
        /// <summary>
        /// Items that already existed. Only with merge mode <c>preserve</c>.
        /// </summary>
        public int? Updated { get; set; }
        /// <summary>
        /// Items whose properties expression returned null.
        /// </summary>
        public int? Skipped { get; set; }
        /// <summary>
        /// Time series upserts whose series already existed with a different type. These were written
        /// with the stored type, and data points for them must use it.
        /// </summary>
        public IEnumerable<CdfWriterTypeMismatch>? TypeMismatches { get; set; }
        /// <summary>
        /// Upserts whose routing rules now select a different space than the recorded placement.
        /// These were written to the recorded space.
        /// </summary>
        public IEnumerable<CdfWriterPlacementConflict>? PlacementConflicts { get; set; }
        /// <summary>
        /// Immutable properties that differ from what was sent. Only with merge mode <c>preserve</c>.
        /// </summary>
        public IEnumerable<CdfWriterMismatch>? Mismatched { get; set; }
    }

    /// <summary>
    /// A time series whose stored type differs from the requested one.
    /// </summary>
    public class CdfWriterTypeMismatch
    {
        /// <summary>
        /// Position in the operation's items.
        /// </summary>
        public int ItemIndex { get; set; }
        /// <summary>
        /// Space of the series.
        /// </summary>
        public string? Space { get; set; }
        /// <summary>
        /// External ID of the series.
        /// </summary>
        public string? ExternalId { get; set; }
        /// <summary>
        /// Type sent by the extractor.
        /// </summary>
        public string? RequestedType { get; set; }
        /// <summary>
        /// Type stored in CDF, which the write kept.
        /// </summary>
        public string? StoredType { get; set; }
    }

    /// <summary>
    /// An upsert whose routing rules disagree with the recorded placement.
    /// </summary>
    public class CdfWriterPlacementConflict
    {
        /// <summary>
        /// Position in the operation's items.
        /// </summary>
        public int ItemIndex { get; set; }
        /// <summary>
        /// External ID of the item.
        /// </summary>
        public string? ExternalId { get; set; }
        /// <summary>
        /// Space the item was written to.
        /// </summary>
        public string? RecordedSpace { get; set; }
        /// <summary>
        /// Space the rules selected for this upsert.
        /// </summary>
        public string? EvaluatedSpace { get; set; }
    }

    /// <summary>
    /// An immutable property whose stored value differs from what was sent.
    /// </summary>
    public class CdfWriterMismatch
    {
        /// <summary>
        /// Space of the node.
        /// </summary>
        public string? Space { get; set; }
        /// <summary>
        /// External ID of the node.
        /// </summary>
        public string? ExternalId { get; set; }
        /// <summary>
        /// Name of the property.
        /// </summary>
        public string? Property { get; set; }
        /// <summary>
        /// Value that was sent.
        /// </summary>
        public JsonElement? Sent { get; set; }
        /// <summary>
        /// Value stored in CDF.
        /// </summary>
        public JsonElement? Found { get; set; }
    }
}
