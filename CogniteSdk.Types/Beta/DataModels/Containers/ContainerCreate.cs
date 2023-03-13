// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Beta.DataModels
{
    /// <summary>
    /// Create a flexible data models container.
    /// </summary>
    public class ContainerCreate
    {
        /// <summary>
        /// Space the container belongs to.
        /// </summary>
        public string Space { get; set; }
        /// <summary>
        /// ExternalId of the container.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// Human readable name of the container.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Description of the container.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Valid uses for the container.
        /// </summary>
        public UsedFor UsedFor { get; set; }
        /// <summary>
        /// Properties indexed by a local unique identifier.
        /// </summary>
        public Dictionary<string, ContainerPropertyDefinition> Properties { get; set; }
    }
}
