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
        public IEnumerable<SimulatorRoutineRevisionLogicalCheck> LogicalCheck { get; set; }

        /// <summary>
        /// Steady state detection configuration.
        /// </summary>
        public IEnumerable<SimulatorRoutineRevisionSteadyStateDetection> SteadyStateDetection { get; set; }

        /// <summary>
        /// List of inputs. Can be either timeseries or constants.
        /// </summary>
        public IEnumerable<SimulatorRoutineRevisionInput> Inputs { get; set; }

        /// <summary>
        /// List of outputs. Outputs can optionally be saved to a timeseries.
        /// </summary>
        public IEnumerable<SimulatorRoutineRevisionOutput> Outputs { get; set; }

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
        /// Cron expression for the schedule.
        /// </summary>
        public string CronExpression { get; set; }

        /// <inheritdoc />      
        public override string ToString() => Stringable.ToString(this);
    }

    /// <summary>
    /// Data sampling configuration.
    /// </summary>
    public class SimulatorRoutineRevisionDataSampling
    {
        /// <summary>
        /// Indicates whether data sampling is enabled.
        /// </summary>
        public bool Enabled { get; set; }

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
