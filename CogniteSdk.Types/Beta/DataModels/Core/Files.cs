// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;

namespace CogniteSdk.Beta.DataModels.Core
{
    /// <summary>
    /// Core data model representation of a file.
    /// </summary>
    public class CogniteFile : CogniteCoreInstanceBase
    {
        /// <summary>
        /// List of assets this file relates to.
        /// </summary>
        public IEnumerable<DirectRelationIdentifier> Assets { get; set; }
        /// <summary>
        /// List of equipment this file relates to.
        /// </summary>
        public IEnumerable<DirectRelationIdentifier> Equipment { get; set; }
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
        /// Point in time when the file upload was completed and the file was made available.
        /// </summary>
        /// <value></value>
        public DateTime? UploadedTime { get; set; }
        /// <summary>
        /// Category of this file, direct relation to `CogniteFileCategory`.
        /// </summary>
        public DirectRelationIdentifier Category { get; set; }
    }

    /// <summary>
    /// This identifies the category of an CogniteFile.
    /// </summary>
    public class CogniteFileCategory : CogniteDescribable
    {
        /// <summary>
        /// Identified category code, such as “AA” for Accounting (from Norsok). The full name, “Accounting” would be written to the inherited name field.
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Name of the standard the category originates from, such as “Norsok”.
        /// </summary>
        public string Standard { get; set; }
        /// <summary>
        /// Reference to where to find the standard used (preferably a URL).
        /// </summary>
        public string StandardReference { get; set; }
    }
}