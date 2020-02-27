// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using CogniteSdk.Types.Common;

namespace CogniteSdk.Files
{
    /// <summary>
    /// File read response resource.
    /// </summary>
    public class File
    {
        /// <summary>
        /// The external ID provided by the client. Must be unique for the resource type.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Javascript friendly internal ID given to the object.
        /// </summary>
        public long? DataSetId { get; set; }

        /// <summary>
        /// Name of the file.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The source of the file.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// File type. E.g. text/plain, application/pdf, ..
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Custom, application specific metadata. String key -> String value.
        /// Limits: Maximum length of key is 32 bytes,
        /// value 512 bytes, up to 16 key-value pairs
        /// </summary>
        [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "System.Text.Json ignores properties that don't have setters")]
        public Dictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// Ids of assets this file is linked to.
        /// </summary>
        public IEnumerable<long> AssetIds { get; set; }

        /// <summary>
        /// Unix timestamp in milliseconds of when the source was created.
        /// </summary>
        public long? SourceCreatedTime { get; set; }

        /// <summary>
        /// Unix timestamp in milliseconds of when the source was last modified.
        /// </summary>
        public long? SourceModifiedTime { get; set; }

        /// <summary>
        /// The server-generated ID of the file.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Whether or not the actual file is uploaded.
        /// </summary>
        public bool Uploaded { get; set; }

        /// <summary>
        /// Unix timestamp in milliseconds of when the file was uploaded.
        /// </summary>
        public long? UploadedTime { get; set; }

        /// <summary>
        /// Unix timestamp in milliseconds of when the file was created.
        /// </summary>
        public long CreatedTime { get; set; }

        /// <summary>
        /// Unix timestamp in milliseconds of when the file was last updated.
        /// </summary>
        public long LastUpdatedTime { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}

