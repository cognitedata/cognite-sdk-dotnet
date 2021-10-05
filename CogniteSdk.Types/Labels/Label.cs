// Copyright 2021 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk
{
    /// <summary>
    /// Label read class
    /// </summary>
    public class Label
    {
        /// <summary>
        /// The external ID provided by the client. Must be unique for the resource type.
        /// At most 255 characters.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// Name of the label. Max 140 characters.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Optional description of the label. Max 500 characters.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Optional internalid of DataSet this label belongs to.
        /// </summary>
        public long? DataSetId { get; set; }
        /// <summary>
        /// The number of milliseconds since 00:00:00 Thursday, 1 January 1970, Coordinated Universal Time (UTC),
        /// minus leap seconds when this label was created.
        /// </summary>
        public long CreatedTime { get; set; }
    }
}
