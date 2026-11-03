// Copyright 2026 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// Request to send a batch of items for a registered mapping task to the CDF writer.
    /// The CDF writer applies the mapping and forwards the items to CDF.
    /// </summary>
    /// <typeparam name="T">Type of the items payload. The shape depends on the mapping task, for
    /// datapoints this is a map from external ID to a list of <see cref="DataPoint"/>.</typeparam>
    public class CdfWriterPayloadRequest<T>
    {
        /// <summary>
        /// External ID of the integration the mapping belongs to.
        /// </summary>
        public string IntegrationId { get; set; }
        /// <summary>
        /// Name of the mapping task the items belong to, as registered with <see cref="CdfWriterSetupItem.TaskName"/>.
        /// </summary>
        public string TaskId { get; set; }
        /// <summary>
        /// Items to send. Serialized as-is.
        /// </summary>
        public T Items { get; set; }
    }
}
