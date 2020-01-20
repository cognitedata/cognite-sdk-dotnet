using System;

namespace CogniteSdk {
    /// <summary>
    /// Identity class. Set either Id or ExternalId.
    /// </summary>
    public class Identity
    {
        private long? _id;
        private string _externalId;

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

        /// <summary>
        /// Identity with internal id.
        /// </summary>
        public long? Id
        {
            get => _id;
            set
            {
                if (_externalId != null)
                {
                    throw new ArgumentException($"Cannot set Id ({value}) when ExternalId ({_externalId}) is already set."); 
                }

                _id = value;
            }
        }

        /// <summary>
        /// Identity with externalId
        /// </summary>
        public string ExternalId
        {
            get => _externalId;
            set
            {
                if (_id.HasValue)
                {
                    throw new ArgumentException($"Cannot set externalId ({value}) when Id ({_id}) is already set.");                    
                }
                
                _externalId = value;
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