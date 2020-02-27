// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using CogniteSdk.Types.Common;

namespace CogniteSdk.Files
{
    /// <summary>
    /// File upload read response resource.
    /// </summary>
    public class FileUploadRead : File
    {
        /// <summary>
        /// The URL where the file contents should be uploaded.
        /// </summary>
        public Uri UploadUrl { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}

