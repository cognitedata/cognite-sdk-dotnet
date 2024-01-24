// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk.Alpha
{

    /// <summary>
    /// A Simulator routine .
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
        public List<SimulatorRoutineRevisionScript> Script { get; set; }

        /// <summary>
        /// Configuration settings for the simulator routine revision.
        /// </summary>
        public SimulatorRoutineConfiguration Configuration { get; set; }

        public override string ToString() => Stringable.ToString(this);

    }

    /// <summary>
    /// Configuration settings for the simulator routine revision.
    /// </summary>
    public class SimulatorRoutineConfiguration
    {
        /// <summary>
        /// Schedule configuration.
        /// </summary>
        public Schedule Schedule { get; set; }

        /// <summary>
        /// Data sampling configuration.
        /// </summary>
        public DataSampling DataSampling { get; set; }

        /// <summary>
        /// Logical check configuration.
        /// </summary>
        public LogicalCheck LogicalCheck { get; set; }

        /// <summary>
        /// Steady state detection configuration.
        /// </summary>
        public SteadyStateDetection SteadyStateDetection { get; set; }

        /// <summary>
        /// List of input timeseries configurations.
        /// </summary>
        public List<InputTimeseries> InputTimeseries { get; set; }

        /// <summary>
        /// List of output timeseries configurations.
        /// </summary>
        public List<OutputTimeseries> OutputTimeseries { get; set; }

        /// <summary>
        /// List of input constants configurations.
        /// </summary>
        public List<InputConstants> InputConstants { get; set; }
    }

    /// <summary>
    /// Schedule configuration.
    /// </summary>
    public class Schedule
    {
        /// <summary>
        /// Indicates whether the schedule is enabled.
        /// </summary>
        public bool Enabled { get; set; }
    }

    /// <summary>
    /// Data sampling configuration.
    /// </summary>
    public class DataSampling
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
    }

    /// <summary>
    /// Logical check configuration.
    /// </summary>
    public class LogicalCheck
    {
        /// <summary>
        /// Indicates whether logical check is enabled.
        /// </summary>
        public bool Enabled { get; set; }
    }

    /// <summary>
    /// Steady state detection configuration.
    /// </summary>
    public class SteadyStateDetection
    {
        /// <summary>
        /// Indicates whether steady state detection is enabled.
        /// </summary>
        public bool Enabled { get; set; }
    }

    /// <summary>
    /// Input timeseries configuration.
    /// </summary>
    public class InputTimeseries
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
    }

    /// <summary>
    /// Output timeseries configuration.
    /// </summary>
    public class OutputTimeseries
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
    }

    /// <summary>
    /// Input constants configuration.
    /// </summary>
    public class InputConstants
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
    }

    /// <summary>
    /// Script configuration.
    /// </summary>
    public class SimulatorRoutineRevisionScript
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
        public List<SimulatorRoutineRevisionScriptStep> Steps { get; set; }
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
        public SimulatorRoutineRevisionArguments Arguments { get; set; }
    }

    /// <summary>
    /// Arguments for a step.
    /// </summary>
    public class SimulatorRoutineRevisionArguments
    {
        /// <summary>
        /// E.g argument type can be "outputTimeSeries".
        /// </summary>
        public string ArgumentType { get; set; }

        /// <summary>
        /// Simulator object name
        /// </summary>
        public string ObjectName { get; set; }

        /// <summary>
        /// Simulator object property
        /// </summary>
        public string ObjectProperty { get; set; }

        /// <summary>
        /// Reference id
        /// </summary>
        public string ReferenceId { get; set; }
    }


}