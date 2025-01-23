// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0
#nullable enable
using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// Unit quantity for simulator value.
    /// </summary>
    public class SimulatorValueUnitQuantity
    {
        /// <summary>
        /// Name of the unit (e.g., "kg", "m", "Pa").
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// External id of the unit (e.g., "mass:kilogm", "length:m", "pressure:pa").
        /// Optional.
        /// </summary>
        public string? ExternalId { get; set; }

        /// <summary>
        /// Quantity of measure for the unit (e.g., "Mass", "Length", "Pressure").
        /// Optional.
        /// </summary>
        public string? Quantity { get; set; }
    }

    /// <summary>
    /// Connection type for simulator model revision data.
    /// </summary>
    public enum SimulatorModelRevisionDataConnectionType
    {
        /// <summary>
        /// Material connection type.
        /// </summary>
        Material,

        /// <summary>
        /// Energy connection type.
        /// </summary>
        Energy,

        /// <summary>
        /// Information connection type.
        /// </summary>
        Information
    }

    /// <summary>
    /// Position data for simulator model revision.
    /// </summary>
    public class SimulatorModelRevisionDataPosition
    {
        /// <summary>
        /// X coordinate.
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Y coordinate.
        /// </summary>
        public float Y { get; set; }
    }

    /// <summary>
    /// Graphical object data for simulator model revision.
    /// </summary>
    public class SimulatorModelRevisionDataGraphicalObject
    {
        /// <summary>
        /// Position of the graphical object.
        /// </summary>
        public SimulatorModelRevisionDataPosition Position { get; set; }

        /// <summary>
        /// Height of the graphical object.
        /// </summary>
        public float? Height { get; set; }

        /// <summary>
        /// Width of the graphical object.
        /// </summary>
        public float? Width { get; set; }

        /// <summary>
        /// Horizontal scale factor.
        /// </summary>
        public bool? ScaleX { get; set; }

        /// <summary>
        /// Vertical scale factor.
        /// </summary>
        public bool? ScaleY { get; set; }

        /// <summary>
        /// Rotation angle.
        /// </summary>
        public float? Angle { get; set; }

        /// <summary>
        /// Whether the object is active.
        /// </summary>
        public bool? Active { get; set; }
    }

    /// <summary>
    /// Property data for simulator model revision.
    /// </summary>
    public class SimulatorModelRevisionDataProperty
    {
        /// <summary>
        /// Property name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Reference object mapping.
        /// </summary>
        public Dictionary<string, string> ReferenceObject { get; set; }

        /// <summary>
        /// Type of the value.
        /// </summary>
        public SimulatorValueType ValueType { get; set; }

        /// <summary>
        /// Property value.
        /// </summary>
        public SimulatorValue Value { get; set; }

        /// <summary>
        /// Unit of the property.
        /// Optional.
        /// </summary>
        public SimulatorValueUnitQuantity? Unit { get; set; }

        /// <summary>
        /// Whether the property is read-only.
        /// </summary>
        public bool? ReadOnly { get; set; }
    }

    /// <summary>
    /// Node data for simulator model revision.
    /// </summary>
    public class SimulatorModelRevisionDataObjectNode
    {
        /// <summary>
        /// Node identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Node name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Node type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Graphical representation.
        /// </summary>
        public SimulatorModelRevisionDataGraphicalObject? GraphicalObject { get; set; }

        /// <summary>
        /// IEnumerable of node properties.
        /// </summary>
        public IEnumerable<SimulatorModelRevisionDataProperty> Properties { get; set; }
    }

    /// <summary>
    /// Edge data for simulator model revision.
    /// </summary>
    public class SimulatorModelRevisionDataObjectEdge
    {
        /// <summary>
        /// Edge identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Edge name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Source node identifier.
        /// </summary>
        public string SourceId { get; set; }

        /// <summary>
        /// Target node identifier.
        /// </summary>
        public string TargetId { get; set; }

        /// <summary>
        /// Type of connection.
        /// </summary>
        public SimulatorModelRevisionDataConnectionType ConnectionType { get; set; }
    }

    /// <summary>
    /// Thermodynamic data for simulator model revision.
    /// </summary>
    public class SimulatorModelRevisionDataThermodynamic
    {
        /// <summary>
        /// IEnumerable of property packages.
        /// </summary>
        public IEnumerable<string> PropertyPackages { get; set; }

        /// <summary>
        /// IEnumerable of components.
        /// </summary>
        public IEnumerable<string> Components { get; set; }
    }

    /// <summary>
    /// Flowsheet data for simulator model revision.
    /// </summary>
    public class SimulatorModelRevisionDataFlowsheet
    {
        /// <summary>
        /// IEnumerable of flowsheet nodes.
        /// </summary>
        public IEnumerable<SimulatorModelRevisionDataObjectNode> SimulatorObjectNodes { get; set; }

        /// <summary>
        /// IEnumerable of flowsheet edges.
        /// </summary>
        public IEnumerable<SimulatorModelRevisionDataObjectEdge> SimulatorObjectEdges { get; set; }

        /// <summary>
        /// Thermodynamic data.
        /// </summary>
        public SimulatorModelRevisionDataThermodynamic Thermodynamics { get; set; }
    }

    /// <summary>
    /// Base data for simulator model revision.
    /// </summary>
    public class SimulatorModelRevisionData
    {
        /// <summary>
        /// A unique identifier.
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// External id of the model revision.
        /// </summary>
        public string ModelRevisionExternalId { get; set; }

        /// <summary>
        /// Flowsheet of the model revision.
        /// </summary>
        public SimulatorModelRevisionDataFlowsheet? Flowsheet { get; set; }

        /// <summary>
        /// Additional simulator-specific information.
        /// </summary>
        public Dictionary<string, string>? Info { get; set; }

        /// <summary>
        /// The number of milliseconds since epoch.
        /// </summary>
        public long CreatedTime { get; set; }

        /// <summary>
        /// The number of milliseconds since epoch.
        /// </summary>
        public long LastUpdatedTime { get; set; }
    }

}
