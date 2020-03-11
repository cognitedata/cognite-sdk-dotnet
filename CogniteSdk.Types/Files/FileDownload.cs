// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// File download response class with Url.
    /// </summary>
    public class FileDownload
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

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
