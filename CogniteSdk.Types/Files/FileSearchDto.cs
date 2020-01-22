// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Files
{
    /// <summary>
    /// File search query DTO.
    /// </summary>
    public class FileSearchDto
    {
        /// <summary>
        /// Prefix and fuzzy search on name.
        /// </summary>
        public string Name { get; set; }
    }
}