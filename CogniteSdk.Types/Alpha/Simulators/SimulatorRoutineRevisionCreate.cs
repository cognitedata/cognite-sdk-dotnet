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
        public ISimulatorRoutineRevisionDataSampling DataSampling { get; set; }

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
    /// Data sampling base configuration.
    /// </summary>
    public interface ISimulatorRoutineRevisionDataSampling
    {
        /// <summary>
        /// Whether sampling is enabled or not
        /// </summary>
        bool Enabled {get;}
    }

    /// <summary>
    /// Data sampling disabled configuration.
    /// </summary>
    public class SimulatorRoutineRevisionDataSamplingDisabled: ISimulatorRoutineRevisionDataSampling
    {
        /// <summary>
        /// Indicates whether data sampling is enabled.
        /// </summary>
        public bool Enabled { get; } = false;

        public override string ToString() => Stringable.ToString(this);
        
        /// <summary>
        /// Equality check
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is SimulatorRoutineRevisionDataSamplingDisabled other && Enabled == other.Enabled;
        }

        /// <summary>
        /// Get the hash code
        /// </summary>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + Enabled.GetHashCode();
                return hash;
            }
        }
    }

    /// <summary>
    /// Data sampling enabled configuration.
    /// </summary>
    public class SimulatorRoutineRevisionDataSamplingEnabled : ISimulatorRoutineRevisionDataSampling
    {
        /// <summary>
        /// Indicates whether data sampling is enabled.
        /// </summary>
        public bool Enabled { set; get; } = true;

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
        /// Equality check
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is SimulatorRoutineRevisionDataSamplingEnabled other)
            {
                return Enabled == other.Enabled &&
                    ValidationWindow == other.ValidationWindow &&
                    SamplingWindow == other.SamplingWindow &&
                    Granularity == other.Granularity;
            }
            return false;
        }

        /// <summary>
        /// Get the hash code
        /// </summary>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + Enabled.GetHashCode();
                hash = hash * 23 + ValidationWindow.GetHashCode();
                hash = hash * 23 + SamplingWindow.GetHashCode();
                hash = hash * 23 + Granularity.GetHashCode();
                return hash;
            }
        }

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
