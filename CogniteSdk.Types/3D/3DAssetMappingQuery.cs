// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The Asset mapping query class.
    /// </summary>
    public class ThreeDAssetMappingQuery : CursorQueryBase
    {
        /// <summary>
        /// Filter on asset mappings.
        /// </summary>
        public ThreeDAssetMappingFilter Filter { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<ThreeDAssetMappingQuery>(this);
    }
}