
using System;
using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk.Alpha
{

    /// <summary>
    /// A Simulator routine revision.
    /// </summary>
    public class SimulatorRoutineRevision
    {
        /// <summary>
        /// The unique identifier of the routine revision.
        /// </summary>
        public long Id { get; set; }

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
        public IEnumerable<SimulatorRoutineRevisionStage> Script { get; set; }

        /// <summary>
        /// Configuration settings for the simulator routine revision.
        /// </summary>
        public SimulatorRoutineRevisionConfiguration Configuration { get; set; }

        /// <summary>
        /// The data set id of the routine revision.
        /// </summary>        
        public long DataSetId { get; set; }

        /// <summary>
        /// The external id of the simulator.
        /// </summary>
        public string SimulatorExternalId { get; set; }

        /// <summary>
        /// The id of the user who created the revision.
        /// </summary>
        public string CreatedByUserId { get; set; }

        /// <summary>
        /// The time when the revision was created.
        /// </summary>
        public long CreatedTime { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}
