// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using CogniteSdk.Types.Common;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// Status of a simulation run
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SimulatorSortOrder
    {
        /// <summary>
        /// Ascending sort order.
        /// </summary>
        asc,

        /// <summary>
        /// Descending sort order.
        /// </summary>
        desc,
    }

    /// <summary>
    /// Sort order item.
    /// </summary>
    public class SimulatorSortItem
    {
        /// <summary>
        /// Sort by property. Only 'createdTime' is supported.
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// Sort order. Ascending ("asc") or descending ("desc").
        /// </summary>
        public SimulatorSortOrder Order { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<SimulatorSortItem>(this);
    }

    /// <summary>
    /// The Simulator model revision class.
    /// </summary>
    public class SimulatorModelRevisionQuery
    {
        /// <summary>
        /// Filter on simulators model revisions.
        /// </summary>
        public SimulatorModelRevisionFilter Filter { get; set; }

        /// <summary>
        /// Sort order.
        /// </summary>
        public IEnumerable<SimulatorSortItem> Sort { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<SimulatorModelRevisionQuery>(this);
    }

    /// <summary>
    /// The simulator model revision filter class.
    /// </summary>
    public class SimulatorModelRevisionFilter
    {
        /// <summary>
        /// Filter on model external ids.
        /// </summary>
        public IEnumerable<string> ModelExternalIds { get; set; }

        /// <summary>
        /// Filter by created time.
        /// </summary>
        /// <value>Range between two timestamps.</value>
        public TimeRange CreatedTime { get; set; }

        /// <summary>
        /// Filter by last updated time.
        /// </summary>
        /// <value>Range between two timestamps.</value>
        public TimeRange LastUpdatedTime { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<SimulatorModelRevisionFilter>(this);
    }
}
