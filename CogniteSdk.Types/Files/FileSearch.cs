// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk.Files
{
    /// <summary>
    /// File search query DTO.
    /// </summary>
    public class FileSearch
    {
        /// <summary>
        /// Prefix and fuzzy search on name.
        /// </summary>
        public string Name { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}