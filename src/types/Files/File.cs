// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Types.Files
{
    public class File
    {
        public string ExternalId { get; set; }

        public string MimeType { get; set; }
        public IDictionary<string, string> MetaData { get; set; }
        public IEnumerable<long> AssetIds { get; set; }
        public string Source { get; set; }
        public long SourceCreatedTime { get; set; }
        public long SourceModifiedTime { get; set; }
        public long UploadedTime { get; set; }

        public string Name { get; set; }
        public long Id { get; set; }
        public long CreatedTime { get; set; }
        public long LastUpdatedTime { get; set; }
        public bool Uploaded { get; set; }
        public string UploadUrl { get; set; }

    }
}

