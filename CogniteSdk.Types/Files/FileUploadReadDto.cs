// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Files
{
    /// <summary>
    /// File upload read response resource.
    /// </summary>
    public class FileUploadReadDto : FileReadDto
    {
        /// <summary>
        /// The URL where the file contents should be uploaded.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "Class is DTO")]
        public string UploadUrl { get; set; }
    }
}

