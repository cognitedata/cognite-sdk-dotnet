namespace CogniteSdk.Types.Common {
    /// <summary>
    /// Abstract base for Identity case classes.
    /// </summary>
    public abstract class Identity { }

    /// <summary>
    /// Id case class.
    /// </summary>
    public class IdentityId : Identity {
        public IdentityId (long id) {
            this.Id = id;
        }

        /// <summary>
        /// A server-generated ID for the object.
        /// </summary>
        public long Id { get; set; }
    }

    /// <summary>
    /// ExternalId case class.
    /// </summary>
    public class IdentityExternalId : Identity {
        public IdentityExternalId (string externalId) {
            this.ExternalId = externalId;
        }

        /// <summary>
        /// The external ID provided by the client. Must be unique for the resource type.
        /// </summary>
        public string ExternalId { get; set; }
    }
}