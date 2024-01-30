// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.Text.Json;
using CogniteSdk.Types.Common;

namespace CogniteSdk.Alpha
{

    /// <summary>
    /// A Simulator routine revision to create.
    /// </summary>
    public class SimulatorRoutineRevisionCreate
    {
        /// <summary>
        /// The external id of the routine revision.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// The external id of the routine to which this revision belongs to.
        /// </summary>
        public string RoutineExternalId { get; set; }

        /// <summary>
        /// List of script configurations.
        /// </summary>
        public IEnumerable<SimulatorRoutineRevisionScriptStage> Script { get; set; }

        /// <summary>
        /// Configuration settings for the simulator routine revision.
        /// </summary>
        public SimulatorRoutineRevisionConfiguration Configuration { get; set; }

        /// <inheritdoc />      
        public override string ToString() => Stringable.ToString(this);

    }

    /// <summary>
    /// Configuration settings for the simulator routine revision.
    /// </summary>
    public class SimulatorRoutineRevisionConfiguration
    {
        /// <summary>
        /// Schedule configuration.
        /// </summary>
        public SimulatorRoutineRevisionSchedule Schedule { get; set; }

        /// <summary>
        /// Data sampling configuration.
        /// </summary>
        public SimulatorRoutineRevisionDataSampling DataSampling { get; set; }

        /// <summary>
        /// Logical check configuration.
        /// </summary>
        public SimulatorRoutineRevisionLogicalCheck LogicalCheck { get; set; }

        /// <summary>
        /// Steady state detection configuration.
        /// </summary>
        public SimulatorRoutineRevisionSteadyStateDetection SteadyStateDetection { get; set; }

        /// <summary>
        /// List of input timeseries configurations.
        /// </summary>
        public IEnumerable<SimulatorRoutineRevisionInputTimeseries> InputTimeseries { get; set; }

        /// <summary>
        /// List of output timeseries configurations.
        /// </summary>
        public IEnumerable<SimulatorRoutineRevisionOutputTimeseries> OutputTimeseries { get; set; }

        /// <summary>
        /// List of output sequences configurations. Used only for the predefined calculations.
        /// </summary>
        public IEnumerable<SimulatorRoutineRevisionOutputSequence> OutputSequences { get; set; }

        /// <summary>
        /// Extra paramethers for the predefined calculations. Used for ChokeDp, IPR, VLP, BhpFromGradientTraverse, and BhpFromGaugeBhp calculation types.
        /// </summary>
        public Dictionary<string, JsonElement> ExtraOptions { get; set; }

        /// <summary>
        /// List of input constants configurations.
        /// </summary>
        public IEnumerable<SimulatorRoutineRevisionInputConstants> InputConstants { get; set; }

        /// <inheritdoc />      
        public override string ToString() => Stringable.ToString(this);
    }

    /// <summary>
    /// Schedule configuration.
    /// </summary>
    public class SimulatorRoutineRevisionSchedule
    {
        /// <summary>
        /// Indicates whether the schedule is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Start time for the schedule.
        /// </summary>
        public long? StartTime { get; set; }

        /// <summary>
        /// Repeat interval for the schedule.
        /// </summary>
        public string Repeat { get; set; }

        /// <inheritdoc />      
        public override string ToString() => Stringable.ToString(this);
    }

    /// <summary>
    /// Data sampling configuration.
    /// </summary>
    public class SimulatorRoutineRevisionDataSampling
    {
        /// <summary>
        /// Validation window for data sampling.
        /// </summary>
        public int ValidationWindow { get; set; }

        /// <summary>
        /// Sampling window for data sampling.
        /// </summary>
        public int SamplingWindow { get; set; }

        /// <summary>
        /// Granularity for data sampling.
        /// </summary>
        public int Granularity { get; set; }

        /// <summary>
        /// Validation end offset for data sampling.
        /// </summary>
        public string ValidationEndOffset { get; set; }

        /// <inheritdoc />      
        public override string ToString() => Stringable.ToString(this);
    }

    /// <summary>
    /// Logical check configuration.
    /// </summary>
    public class SimulatorRoutineRevisionLogicalCheck
    {
        /// <summary>
        /// Indicates whether logical check is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Timeseries to use for logical check.
        /// </summary>
        public string TimeseriesExternalId { get; set; }

        /// <summary>
        /// Aggregate function to use for logical check.
        /// </summary>
        public string Aggregate { get; set; }

        /// <summary>
        /// Logical check comparison operator.
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// Logical check value.
        /// </summary>
        public double? Value { get; set; }

        /// <inheritdoc />      
        public override string ToString() => Stringable.ToString(this);
    }

    /// <summary>
    /// Steady state detection configuration.
    /// </summary>
    public class SimulatorRoutineRevisionSteadyStateDetection
    {
        /// <summary>
        /// Indicates whether steady state detection is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Timeseries to use for steady state detection.
        /// </summary>
        public string TimeseriesExternalId { get; set; }

        /// <summary>
        /// Aggregate function to use for steady state detection.
        /// </summary>
        public string Aggregate { get; set; }

        /// <summary>
        /// Min section size for steady state detection.
        /// </summary>
        public int? MinSectionSize { get; set; }

        /// <summary>
        /// Var threshold for steady state detection.
        /// </summary>
        public double? VarThreshold { get; set; }

        /// <summary>
        /// Slope threshold for steady state detection.
        /// </summary>
        public double? SlopeThreshold { get; set; }

        /// <inheritdoc />      
        public override string ToString() => Stringable.ToString(this);
    }

    /// <summary>
    /// Input timeseries configuration.
    /// </summary>
    public class SimulatorRoutineRevisionInputTimeseries
    {
        /// <summary>
        /// Name of the input timeseries.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Reference id of the input timeseries.
        /// </summary>
        public string ReferenceId { get; set; }

        /// <summary>
        /// Unit of the input timeseries.
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Unit type of the input timeseries.
        /// </summary>
        public string UnitType { get; set; }

        /// <summary>
        /// External id of the source for the input timeseries.
        /// </summary>
        public string SourceExternalId { get; set; }

        /// <summary>
        /// Aggregate function for the input timeseries.
        /// </summary>
        public string Aggregate { get; set; }

        /// <summary>
        /// External id to save the input timeseries.
        /// </summary>
        public string SaveTimeseriesExternalId { get; set; }

        /// <inheritdoc />      
        public override string ToString() => Stringable.ToString(this);
    }

    /// <summary>
    /// Output timeseries configuration.
    /// </summary>
    public class SimulatorRoutineRevisionOutputTimeseries
    {
        /// <summary>
        /// Name of the output timeseries.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Reference id of the output timeseries.
        /// </summary>
        public string ReferenceId { get; set; }

        /// <summary>
        /// Unit of the output timeseries.
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Unit type of the output timeseries.
        /// </summary>
        public string UnitType { get; set; }

        /// <summary>
        /// External id to save the output timeseries.
        /// </summary>
        public string SaveTimeseriesExternalId { get; set; }

        /// <inheritdoc />      
        public override string ToString() => Stringable.ToString(this);
    }

    /// <summary>
    /// Output sequences configuration.
    /// </summary>
    public class SimulatorRoutineRevisionOutputSequence
    {
        /// <summary>
        /// Name of the output sequence.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Reference id of the output sequence.
        /// </summary>
        public string ReferenceId { get; set; }

        /// <inheritdoc />      
        public override string ToString() => Stringable.ToString(this);
    }

    /// <summary>
    /// Input constants configuration.
    /// </summary>
    public class SimulatorRoutineRevisionInputConstants
    {
        /// <summary>
        /// Name of the input constant.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// External id to save the input constant.
        /// </summary>
        public string SaveTimeseriesExternalId { get; set; }

        /// <summary>
        /// Value of the input constant.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Unit of the input constant.
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Unit type of the input constant.
        /// </summary>
        public string UnitType { get; set; }

        /// <summary>
        /// Reference id of the input constant.
        /// </summary>
        public string ReferenceId { get; set; }

        /// <inheritdoc />      
        public override string ToString() => Stringable.ToString(this);
    }

    /// <summary>
    /// Script configuration.
    /// </summary>
    public class SimulatorRoutineRevisionScriptStage
    {
        /// <summary>
        /// Order of the script.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Description of the script.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// List of steps in the script.
        /// </summary>
        public IEnumerable<SimulatorRoutineRevisionScriptStep> Steps { get; set; }

        /// <inheritdoc />      
        public override string ToString() => Stringable.ToString(this);
    }

    /// <summary>
    /// Step configuration in a script.
    /// </summary>
    public class SimulatorRoutineRevisionScriptStep
    {
        /// <summary>
        /// Order of the step.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Type of the step.
        /// </summary>
        public string StepType { get; set; }

        /// <summary>
        /// Description of the step.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Arguments for the step.
        /// </summary>
        public Dictionary<string, string> Arguments { get; set; }

        /// <inheritdoc />      
        public override string ToString() => Stringable.ToString(this);
    }
}
