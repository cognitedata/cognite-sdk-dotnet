// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Beta.DataModels.Core
{
    /// <summary>
    /// The SourceSystem core concept is used to standardize the way source systems are stored.
    /// </summary>
    public class SourceSystem : IDescribable
    {
        /// <inheritdoc />
        public string Name { get; set; }
        /// <inheritdoc />
        public string Description { get; set; }
        /// <inheritdoc />
        public IEnumerable<string> Tags { get; set; }
        /// <inheritdoc />
        public IEnumerable<string> Aliases { get; set; }

        /// <summary>
        /// Version identifier for the source system.
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// Manufacturer of the source system.
        /// </summary>
        public string Manufacturer { get; set; }
    }
}