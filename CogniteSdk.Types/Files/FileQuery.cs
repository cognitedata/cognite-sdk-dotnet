// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk.Files
{
    /// <summary>
    /// The File query DTO.
    /// </summary>
    public class FileQuery : CursorQueryBase
    {
        /// <summary>
        /// File filter.
        /// </summary>
        public FileFilter Filter { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}