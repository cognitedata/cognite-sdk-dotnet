// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;
using System;

namespace CogniteSdk.Files
{
    /// <summary>
    /// File download response with Url.
    /// </summary>
    public class FileDownloadDto : Stringable
    {
        /// <summary>
        /// Id of the file object.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// User provided externalId for the object.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Url from which file can be downloaded.
        /// </summary>
        public Uri DownloadUrl { get; set; }
    }
}
