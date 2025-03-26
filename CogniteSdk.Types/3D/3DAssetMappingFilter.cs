// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The 3D AssetMapping asset filter class.
    /// </summary>
    public class ThreeDAssetMappingFilter
    {
        /// <summary>
        /// Ids for assets.
        /// </summary>
        public IEnumerable<long> AssetIds { get; set; }

        /// <summary>
        /// Ids for nodes.
        /// </summary>
        public IEnumerable<long> NodeIds { get; set; }

        /// <summary>
        /// Tree indexes.
        /// </summary>
        public IEnumerable<long> TreeIndexes { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
