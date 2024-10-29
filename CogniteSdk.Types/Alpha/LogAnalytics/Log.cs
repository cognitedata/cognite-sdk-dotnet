using CogniteSdk.DataModels;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// A retrieved log from ILA.
    /// </summary>
    /// <typeparam name="T">Type of the properties bag.</typeparam>
    public class Log<T>
    {
        /// <summary>
        /// Id of the space the log belongs to.
        /// </summary>
        public string Space { get; set; }
        /// <summary>
        /// The number of milliseconds since 00:00:00 Thursday 1 January 1970, Coordinated
        /// Unversal Time (UTC) minus leap seconds.
        /// </summary>
        public long CreatedTime { get; set; }
        /// <summary>
        /// Spaces to containers to properties and their values for the requested containers.
        /// You can use <see cref="StandardInstanceData"/> as a fallback for generic results here.
        /// </summary>
        public T Properties { get; set; }
    }
}