// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Files
{
    /// <summary>
    /// The File query DTO.
    /// </summary>
    public class FileQueryDto : CursorQueryBase
    {
        /// <summary>
        /// File filter.
        /// </summary>
        public FileFilterDto Filter { get; set; }
    }
}