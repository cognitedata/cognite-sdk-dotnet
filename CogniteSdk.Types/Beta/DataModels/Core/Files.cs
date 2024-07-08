// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Beta.DataModels.Core
{
    /// <summary>
    /// Core data model representation of a file.
    /// </summary>
    public class FileBase : CoreInstanceBase
    {
        /// <summary>
        /// List of assets this file relates to.
        /// </summary>
        public IEnumerable<DirectRelationIdentifier> Assets { get; set; }
        /// <summary>
        /// MIME type of the file.
        /// </summary>
        public string MimeType { get; set; }
        /// <summary>
        /// Contains the path elements from the source
        /// (for when the source system has a file system hierarchy or similar)
        /// </summary>
        public string Directory { get; set; }
        /// <summary>
        /// Whether the file content has been uploaded to Cognite Data Fusion.
        /// </summary>
        /// <value></value>
        public bool? IsUploaded { get; set; }
        /// <summary>
        /// Class of this file, direct relation to `FileClass`.
        /// </summary>
        public DirectRelationIdentifier FileClass { get; set; }
    }

    /// <summary>
    /// Core data model representation of the class of a file,
    /// containing common properties.
    /// </summary>
    public class FileClass : IDescribable
    {
        /// <inheritdoc />
        public string Name { get; set; }
        /// <inheritdoc />
        public string Description { get; set; }
        /// <inheritdoc />
        public IEnumerable<string> Tags { get; set; }
        /// <inheritdoc />
        public IEnumerable<string> Aliases { get; set; }
    }
}