using System.Runtime.InteropServices;

namespace CogniteSdk {
    /// <summary>
    /// Abstract base for Identity case classes.
    /// </summary>
    public abstract class Identity
    {
        /// <summary>
        /// Create new external Id.
        /// </summary>
        /// <param name="externalId">External id value</param>
        /// <returns>New external Id.</returns>
        public static IdentityExternalId ExternalId(string externalId)
        {
            return new IdentityExternalId(externalId);
        }

        /// <summary>
        /// Create new internal Id.
        /// </summary>
        /// <param name="internalId">Internal id value</param>
        /// <returns>New internal Id.</returns>
        public static IdentityId Id(long internalId)
        {
            return new IdentityId(internalId);
        }
    }

    /// <summary>
    /// The Id case class.
    /// </summary>
    public class IdentityId : Identity {
        /// <summary>
        /// The IdentityId constructor.
        /// </summary>
        /// <param name="id">The Id value to use.</param>
        public IdentityId (long id) {
            Id = id;
        }

        /// <summary>
        /// A server-generated ID for the object.
        /// </summary>
        public new long Id { get; set; }
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
            ExternalId = externalId;
        }

        /// <summary>
        /// The external ID provided by the client. Must be unique for the resource type.
        /// </summary>
        public new string ExternalId { get; set; }
    }
}