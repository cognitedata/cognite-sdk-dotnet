// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The ThreeDThumbnail query class.
    /// </summary>
    public class ThreeDUpdateThumbnailQuery : CursorQueryBase
    {
        /// <summary>
        /// Id of file to use as thumbnail. JPEG or PNG.
        /// </summary>
        public long FileId { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<ThreeDUpdateThumbnailQuery>(this);
    }
}