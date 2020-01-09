// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Types.Raw
{
    /// <summary>
    /// Raw table object.
    /// </summary>
    public class Table
    {
        /// <summary>
        /// Name of the table. Unique in database.
        /// </summary>
        public string Name { get; set; }
    }
}
