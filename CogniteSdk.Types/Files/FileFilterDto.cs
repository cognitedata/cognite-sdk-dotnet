// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;
using System.Collections.Generic;

namespace CogniteSdk.Files
{
    /// <summary>
    /// The File query DTO.
    /// </summary>
    public class FileFilterDto
    {
        /// <summary>
        /// Name of the file.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// File type. E.g. text/plain, application/pdf, ..
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Custom, application specific metadata. String key -> String value. Limits: Maximum length of key is 32
        /// bytes, value 512 bytes, up to 16 key-value pairs.
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// Only include files that reference these specific asset IDs.
        /// </summary>
        public IEnumerable<long> AssetIds { get; set; }

        /// <summary>
        /// Only include files that have a related asset in a tree rooted at any of these root assetIds.
        /// </summary>
        public IEnumerable<Identity> RootAssetIds { get; set; }

        /// <summary>
        /// Only include files that have a related asset in a subtree rooted at any of these assetIds (including the
        /// roots given). If the total size of the given subtrees exceeds 100,000 assets, an error will be returned.
        /// </summary>
        /// <value></value>
        public IEnumerable<Identity> AssetSubtreeIds { get; set; }

        /// <summary>
        /// The source of the file.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Range between two timestamps.
        /// </summary>
        public TimeRange CreatedTime { get; set; }

        /// <summary>
        /// Range between two timestamps.
        /// </summary>
        public TimeRange LastUpdatedTime { get; set; }

        /// <summary>
        /// Range between two timestamps.
        /// </summary>
        public TimeRange UploadedTime { get; set; }

        /// <summary>
        /// Range between two timestamps. When the source was created.
        /// </summary>
        /// <value></value>
        public TimeRange SourceCreatedTime { get; set; }

        /// <summary>
        /// Range between two timestamps.
        /// </summary>
        public TimeRange SourceModifiedTime { get; set; }

        /// <summary>
        /// Filter by this (case-sensitive) prefix for the external ID.
        /// </summary>
        public string ExternalIdPrefix { get; set; }

        /// <summary>
        /// Whether or not the actual file is uploaded. This field is returned only by the API, it has no effect in a
        /// post body.
        /// </summary>
        /// <value></value>
        public bool? Uploaded { get; set; }

        /// <summary>
        /// Only include assets that belong to these datasets.
        /// </summary>
        public IEnumerable<long> DataSetIds { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<FileFilterDto>(this);
    }
}