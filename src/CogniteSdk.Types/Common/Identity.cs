namespace CogniteSdk {
    /// <summary>
    /// Abstract base for Identity case classes.
    /// </summary>
    public abstract class Identity { }

    /// <summary>
    /// The Id case class.
    /// </summary>
    public class IdentityId : Identity {
        /// <summary>
        /// The IdentityId constructor.
        /// </summary>
        /// <param name="id">The Id value to use.</param>
        public IdentityId (long id) {
            this.Id = id;
        }

        /// <summary>
        /// A server-generated ID for the object.
        /// </summary>
        public long Id { get; set; }
    }

    /// <summary>
    /// The ExternalId case class.
    /// </summary>
    public class IdentityExternalId : Identity {
        /// <summary>
        /// The IdentityExternalId constructor.
        /// </summary>
        /// <param name="externalId">The ExternalId value to use.</param>
        public IdentityExternalId (string externalId) {
            this.ExternalId = externalId;
        }

        /// <summary>
        /// The external ID provided by the client. Must be unique for the resource type.
        /// </summary>
        public string ExternalId { get; set; }
    }
}