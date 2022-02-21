// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk
{
    /// <summary>
    /// Create notifications for transformations.
    /// </summary>
    public class TransformationNotificationCreate
    {
        /// <summary>
        /// Create a transformation notification with transformation external id.
        /// </summary>
        /// <param name="externalId">Transformation external id</param>
        public TransformationNotificationCreate(string externalId)
        {
            TransformationExternalId = externalId;
        }

        /// <summary>
        /// Create a transformation notification with transformation internal id.
        /// </summary>
        /// <param name="internalId">Transformation internal id</param>
        public TransformationNotificationCreate(long internalId)
        {
            TransformationId = internalId;
        }

        /// <summary>
        /// Transformation internal id to create notification for.
        /// </summary>
        public long? TransformationId { get; }

        /// <summary>
        /// Transformation external id to create notification for.
        /// </summary>
        public string TransformationExternalId { get; }

        /// <summary>
        /// Destination to send notifications to.
        /// </summary>
        public string Destination { get; set; }
    }
}
