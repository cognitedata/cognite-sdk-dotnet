// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;

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
        public Uri UploadUrl { get; set; }
    }
}

