// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;

namespace CogniteSdk.DataModels.Core
{
    /// <summary>
    /// Abstract class implementing describable concept.
    /// /// </summary>
    public abstract class CogniteDescribable : ICogniteDescribable
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
    /// Abstract class implementing sourcable concept.
    /// </summary>
    public abstract class CogniteSourceable : ICogniteSourceable
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
    /// Abstract class implementing source system concept.
    /// </summary>
    public abstract class CogniteSourceSystem : CogniteDescribable, ICogniteSourceSystem
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
    /// Abstract class implementing schedulable system concept.
    /// </summary>
    public abstract class CogniteSchedulable : ICogniteSchedulable
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
    /// Abstract class implementing visualizable concept.
    /// </summary>
    public abstract class CogniteVisualizable : ICogniteVisualizable
    {
        /// <summary>
        /// Direct relation to a 3D object.
        /// </summary>
        public DirectRelationIdentifier Object3D { get; set; }
    }

    /// <summary>
    /// Does not correspond to a core data model view,
    /// this is a convenience class implementing Describable and Sourcable.
    /// </summary>
    public abstract class CogniteCoreInstanceBase : CogniteSourceable, ICogniteDescribable
    {
        /// <inheritdoc />
        public string Name { get; set; }
        /// <inheritdoc />
        public string Description { get; set; }
        /// <inheritdoc />
        public IEnumerable<string> Tags { get; set; }
        /// <inheritdoc />
        public IEnumerable<string> Aliases { get; set; }
    }

    /// <summary>
    /// Abstract class implementing object 3D concept.
    /// </summary>
    public abstract class CogniteObject3DBase : CogniteCoreInstanceBase
    {

    }
}
