// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk.Raw
{
    /// <summary>
    /// Raw table object.
    /// </summary>
    public class TableDto : Stringable
    {
        /// <summary>
        /// Name of the table. Unique in database.
        /// </summary>
        public string Name { get; set; }
    }
}
