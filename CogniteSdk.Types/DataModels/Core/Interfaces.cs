// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;

namespace CogniteSdk.DataModels.Core
{
    /// <summary>
    /// Core data model interface for instances.
    /// 
    /// The describable core concept is used as a standard way of holding
    /// the bare minimum of information about the instance. Used for nodes.
    /// /// </summary>
    public interface ICogniteDescribable
    {
        /// <summary>
        /// Name of the instance.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Description of the instance.
        /// </summary>
        /// <value></value>
        public string Description { get; set; }
        /// <summary>
        /// Text based labels for generic use, limited to 1000.
        /// </summary>
        public IEnumerable<string> Tags { get; set; }
        /// <summary>
        /// Alternative names for the instance.
        /// </summary>
        public IEnumerable<string> Aliases { get; set; }
    }

    /// <summary>
    /// Core data model interface for instances.
    /// 
    /// The sourceable core concept is used as way to describe the origin of
    /// an instance, and providing a link back to the source system.
    /// </summary>
    public interface ICogniteSourceable
    {
        /// <summary>
        /// Identifier in the source system.
        /// </summary>
        public string SourceId { get; set; }
        /// <summary>
        /// Context of the source id. For systems where the sourceId is globally unique, the sourceContext
        /// is expected to not be set.
        /// </summary>
        public string SourceContext { get; set; }
        /// <summary>
        /// Direct relation to a source system.
        /// </summary>
        public DirectRelationIdentifier Source { get; set; }
        /// <summary>
        /// When the instance was created in the source system (if available).
        /// </summary>
        public DateTime? SourceCreatedTime { get; set; }
        /// <summary>
        /// When the instance was last updated in the source system (if available)
        /// </summary>
        public DateTime? SourceUpdatedTime { get; set; }
        /// <summary>
        /// User identifier from the source system on who created the source data.
        /// This identifier is not guaranteed to match the user identifiers in CDF.
        /// </summary>
        public string SourceCreatedUser { get; set; }
        /// <summary>
        /// User identifier from the source system on who last updated the source data.
        /// This identifier is not guaranteed to match the user identifiers in CDF.
        /// </summary>
        public string SourceUpdatedUser { get; set; }
    }

    /// <summary>
    /// Core data model interface for instances.
    /// 
    /// Represents the source system the data comes from
    /// </summary>
    public interface ICogniteSourceSystem : ICogniteDescribable
    {
        /// <summary>
        /// Version identifier for the source system.
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// Manufacturer of the source system.
        /// </summary>
        public string Manufacturer { get; set; }
    }

    /// <summary>
    /// Core data model interface for instances.
    /// 
    /// Schedulable represents the metadata about when an activity (or similar) starts and ends.
    /// </summary>
    public interface ICogniteSchedulable
    {
        /// <summary>
        /// The actual start time of an activity (or similar that extends this).
        /// </summary>
        /// <value></value>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// The actual end time of an activity (or similar that extends this).
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// The planned start time of an activity (or similar that extends this).
        /// </summary>
        public DateTime? ScheduledStartTime { get; set; }
        /// <summary>
        /// The planned end time of an activity (or similar that extends this).
        /// </summary>
        public DateTime? ScheduledEndTime { get; set; }
    }

    /// <summary>
    /// Core data model interface for instances.
    /// 
    /// Visualizable represents the standard way to reference a related 3D resource.
    /// </summary>
    public interface ICogniteVisualizable
    {
        /// <summary>
        /// Direct relation to a 3D object.
        /// </summary>
        public DirectRelationIdentifier Object3D { get; set; }
    }

    /// <summary>
    /// Interface for types representing 3D objects.
    /// </summary>
    public interface ICogniteObject3D : ICogniteSourceable, ICogniteDescribable
    {

    }
}