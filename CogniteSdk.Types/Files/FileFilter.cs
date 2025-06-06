// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The File query class.
    /// </summary>
    public class FileFilter
    {
        /// <summary>
        /// Name of the file.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Filter by this (case-sensitive) prefix for the directory.
        /// </summary>
        public string DirectoryPrefix { get; set; }

        /// <summary>
        /// File type. E.g. text/plain, application/pdf, ..
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Custom, application specific metadata. String key -> String value. Limits: Maximum length of key is 32
        /// bytes, value 512 bytes, up to 16 key-value pairs.
        /// </summary>
        [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "System.Text.Json ignores properties that don't have setters")]
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
        public IEnumerable<Identity> AssetSubtreeIds { get; set; }

        /// <summary>
        /// Asset External IDs of related equipment that this file relates to.
        /// </summary>
        public IEnumerable<string> AssetExternalIds { get; set; }

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
        public IEnumerable<Identity> DataSetIds { get; set; }

        /// <summary>
        /// Label filter.
        /// </summary>
        public LabelFilter Labels { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
