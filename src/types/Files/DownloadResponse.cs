// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CogniteSdk.Types.Files
{
    public class DownloadResponse
    {
        public long Id { get; set; }
        public string ExternalId { get; set; }
        public string DownloadUrl { get; set; }
    }
}
