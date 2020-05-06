// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The ThreeDRevisionUpdateThumbnail class.
    /// </summary>
    public class ThreeDRevisionUpdateThumbnail
    {
        /// <summary>
        /// File ID of thumbnail file in Files API. Only JPEG and PNG files are supported.
        /// </summary>
        public long FileId { get; set; }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return obj is ThreeDRevisionUpdateThumbnail dto &&
                   FileId == dto.FileId;
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return 2108858624 + FileId.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}

