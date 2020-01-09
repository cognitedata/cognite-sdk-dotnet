namespace CogniteSdk.Types.Common
{
    /// <summary>
    /// Abstract base for Identity case classes.
    /// </summary>
    public abstract class IdentityBase {}

    /// <summary>
    /// Id case class.
    /// </summary>
    public class IdentityId : IdentityBase
    {
        /// <summary>
        /// A server-generated ID for the object.
        /// </summary>
        public long Id { get; set; }
    }

    /// <summary>
    /// ExternalId case class.
    /// </summary>
    public class IdentityExternalId : IdentityBase
    {
        /// <summary>
        /// The external ID provided by the client. Must be unique for the resource type.
        /// </summary>
        public string ExternalId { get; set; }
    }

    /// <summary>
    /// Both Id and ExternalId case class. Needed for deserialization. Only one of them
    /// will/should be set.
    /// </summary>
    public class Identity : IdentityBase
    {
        /// <summary>
        /// A server-generated ID for the object.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The external ID provided by the client. Must be unique for the resource type.
        /// </summary>
        public string ExternalId { get; set; }
    }
}