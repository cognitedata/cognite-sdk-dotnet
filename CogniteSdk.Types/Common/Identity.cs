// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using CogniteSdk.DataModels;

namespace CogniteSdk
{
    /// <summary>
    /// Identity class. Set either Id, ExternalId or InstanceId.
    /// </summary>
    public class Identity
    {
        private InstanceIdentifier _instanceId;
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
        /// <param name="id">The internalId to set</param>
        public Identity(long id)
        {
            Id = id;
        }

        /// <summary>
        /// Creates an identity with externalId and space set.
        /// </summary>
        /// <param name="instanceId">Instance Identifier containing space and externalId</param>
        public Identity(InstanceIdentifier instanceId)
        {
            InstanceId = instanceId;
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
                if (_instanceId != null)
                {
                    throw new ArgumentException($"Cannot set Id ({value}) when InstanceId ({_instanceId}) is already set.");
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
                if (_instanceId != null)
                {
                    throw new ArgumentException($"Cannot set externalId ({value}) when InstanceId ({_instanceId}) is already set.");
                }

                _externalId = value;
            }
        }
        /// <summary>
        /// Identity with instanceId
        /// </summary>
        public InstanceIdentifier InstanceId
        {
            get => _instanceId;
            set
            {
                if (_id.HasValue)
                {
                    throw new ArgumentException($"Cannot set instanceId ({value}) when Id ({_id}) is already set.");
                }
                if (_externalId != null)
                {
                    throw new ArgumentException($"Cannot set instanceId ({value}) when ExternalId ({_externalId}) is already set.");
                }

                _instanceId = value ?? new InstanceIdentifier();
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

        /// <summary>
        /// Create new instance Id.
        /// </summary>
        /// <param name="instanceId">Instance id value</param>
        /// <returns>New instance Id.</returns>
        public static Identity Create(InstanceIdentifier instanceId)
        {
            return new Identity(instanceId);
        }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            if (Id.HasValue)
            {
                return $"{{ Id = {Id} }}";
            }
            else if (ExternalId != null)
            {
                return $"{{ ExternalId = \"{ExternalId}\" }}";
            }
            else
            {
                return InstanceId.ToString();
            }

        }

        /// <summary>
        /// Return true if <paramref name="obj"/> is another, identitical Identity.
        /// </summary>
        /// <param name="obj">Object to compare</param>
        /// <returns>True if equal, false otherwise</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (!(obj is Identity other))
            {
                return false;
            }

            if (Id.HasValue)
            {
                return Id == other.Id;
            }

            if (ExternalId != null)
            {
                return ExternalId == other.ExternalId;
            }

            if (InstanceId != null)
            {
                return InstanceId.Equals(other.InstanceId);
            }

            return !other.Id.HasValue && other.ExternalId == null && other.InstanceId == null;
        }

        /// <summary>
        /// Returns a hashcode representing this Identity.
        /// </summary>
        /// <returns>Hashcode representing this</returns>
        public override int GetHashCode()
        {
            if (Id.HasValue)
            {
                return Id.GetHashCode();
            }
            else if (InstanceId != null)
            {
                return InstanceId.GetHashCode();
            }
            else
            {
                return ExternalId?.GetHashCode() ?? 0;
            }
        }
    }
}
