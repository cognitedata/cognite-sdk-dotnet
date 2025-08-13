// Copyright 2025 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// Defines model dependency field for a simulator.
    /// </summary>
    public class SimulatorModelDependencyField
    {
        /// <summary>
        /// Name of the field
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Label of the field. Used to render the field label in the GUI.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Info of the field. Used to render a tooltip for the field in the GUI.
        /// </summary>
        public string Info { get; set; }
    }

    /// <summary>
    /// Defines a model dependency for a simulator.
    /// </summary>
    public class SimulatorModelDependency
    {
        /// <summary>
        /// File extension types allowed as a model dependency.
        /// </summary>
        public IEnumerable<string> FileExtensionTypes { get; set; }

        /// <summary>
        /// List of simulator specific fields for the dependency. Used to link the file to a simulator object in the model.
        /// </summary>
        public IEnumerable<SimulatorModelDependencyField> Fields { get; set; }
    }

    /// <summary>
    /// Represents a file dependency field for referencing a file in CDF.
    /// </summary>
    public class SimulatorFileDependencyReference
    {
        /// <summary>
        /// File id of the file dependency. This is used to reference a file in CDF.
        /// </summary>
        public long Id { get; set; }
    }

    /// <summary>
    /// Represents a file dependency in a simulator model.
    /// </summary>
    public class SimulatorFileDependency
    {
        /// <summary>
        /// Reference to a file in CDF.
        /// </summary>
        public SimulatorFileDependencyReference File { get; set; }

        /// <summary>
        /// Simulator-specific arguments for the file dependency. These are used to link the file 
        /// to a specific object in the target model.
        /// </summary>
        public Dictionary<string, string> Arguments { get; set; }
    }
}
