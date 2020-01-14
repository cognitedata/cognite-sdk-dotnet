// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Text.Json;

namespace CogniteSdk.Raw
{
    /// <summary>
    /// Raw database object.
    /// </summary>
    public class DatabaseDto
    {
        /// <summary>
        /// Unique name of a database.
        /// </summary>
        public string Name { get; set; }
    }
}