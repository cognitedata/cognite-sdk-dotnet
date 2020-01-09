namespace CogniteSdk.Types.Common
{
    /// <summary>
    /// Abstract base for Identity case classes.
    /// </summary>
    public abstract class Identity {}

    /// <summary>
    /// Id case class.
    /// </summary>
    public class IdentityId : Identity
    {
        /// <summary>
        /// A server-generated ID for the object.
        /// </summary>
        public long Id { get; set; }
    }

    /// <summary>
    /// ExternalId case class.
    /// </summary>
    public class IdentityExternalId : Identity
    {
        /// <summary>
        /// The external ID provided by the client. Must be unique for the resource type.
        /// </summary>
        public string ExternalId { get; set; }
    }
}