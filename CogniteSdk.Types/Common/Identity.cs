using System;
using System.Runtime.InteropServices;

namespace CogniteSdk {
    /// <summary>
    /// Identity class. Set either Id or ExternalId.
    /// </summary>
    public class Identity
    {
        /// <summary>
        /// Creates an empty identity with both properties null.
        /// </summary>
        public Identity()
        {
            ExternalId = null;
            Id = null;
        }

        /// <summary>
        /// Creates an identity with externalId set.
        /// </summary>
        /// <param name="externalId">The externalId to set</param>
        public Identity(string externalId)
        {
            ExternalId = externalId;
        }

        /// <summary>
        /// Creates an identity with internalId set.
        /// </summary>
        /// <param name="internalId">The internalId to set</param>
        public Identity(long internalId)
        {
            Id = internalId;
        }

        private long? id;
        /// <summary>
        /// Identity with internal id.
        /// </summary>
        public long? Id
        {
            get => id;
            set
            {
                if (Id == null)
                {
                    id = value;
                }
                else
                {
                    throw new ArgumentException("Cannot set Id when ExternalId is already set.");
                }
            }
        }
        private string externalId;

        /// <summary>
        /// Identity with externalId
        /// </summary>
        public string ExternalId
        {
            get => externalId;
            set
            {
                if (Id == null)
                {
                    externalId = value;
                }
                else
                {
                    throw new ArgumentException("Cannot set externalId when Id is already set.");
                }
            }
        }


        /// <summary>
        /// Create new external Id.
        /// </summary>
        /// <param name="externalId">External id value</param>
        /// <returns>New external Id.</returns>
        public static Identity Create(string externalId)
        {
            return new Identity(externalId);
        }

        /// <summary>
        /// Create new internal Id.
        /// </summary>
        /// <param name="internalId">Internal id value</param>
        /// <returns>New internal Id.</returns>
        public static Identity Create(long internalId)
        {
            return new Identity(internalId);
        }
    }
}