// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0
using System.Collections.Generic;

namespace CogniteSdk
{
    /// <summary>
    /// Class for creating a new data set
    /// </summary>
    public class DataSetCreate
    {
        /// <summary>
        /// The external ID provided by the client. Must be unique within the project.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// The name of the data set.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The description of the data set.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Custom, application specific metadata. String key -> String value.
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; }
        /// <summary>
        /// To write data to a write-protected data set,
        /// you need to be a member of a group that has the "datasets:owner" action for the data set.
        /// </summary>
        public bool WriteProtected { get; set; }
    }
}
