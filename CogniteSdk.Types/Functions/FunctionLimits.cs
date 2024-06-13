using System.Collections.Generic;

namespace CogniteSdk
{
    /// <summary>
    /// Range of resource limits for functions.
    /// </summary>
    public class FunctionResourceRange
    {
        /// <summary>
        /// The minimum value you can request when creating a function.
        /// </summary>
        public float Min { get; set; }
        /// <summary>
        /// The maximum value you can request when creating a function.
        /// </summary>
        public float Max { get; set; }
        /// <summary>
        /// The default value when creating a function.
        /// </summary>
        public float Default { get; set; }
    }

    /// <summary>
    /// Limits for functions in a specific project.
    /// </summary>
    public class FunctionLimits
    {
        /// <summary>
        /// Timeout of each function call.
        /// </summary>
        public int TimeoutMinutes { get; set; }
        /// <summary>
        /// The number of CPU cores per function execution.
        /// </summary>
        public FunctionResourceRange CpuCores { get; set; }
        /// <summary>
        /// The amount of available memory in GB per function execution.
        /// </summary>
        public FunctionResourceRange MemoryGb { get; set; }
        /// <summary>
        /// Available runtimes. For example runtime "py38" translates to the latest version of the Python 3.8 series.
        /// </summary>
        public IEnumerable<string> Runtimes { get; set; }
        /// <summary>
        /// Maximum response size of function calls.
        /// </summary>
        public int ResponseSizeMb { get; set; }
    }
}