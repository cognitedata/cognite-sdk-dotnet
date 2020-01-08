// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CogniteSdk.Types.Files
{
    /// <summary>
    /// File download response with Url.
    /// </summary>
    public class DownloadResponse
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
        public string DownloadUrl { get; set; }
    }
}
