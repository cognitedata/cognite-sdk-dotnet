// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk
{
    /// <summary>
    /// Subscription to notifications on a transformation.
    /// </summary>
    public class TransformationNotification
    {
        /// <summary>
        /// Internal id of notification subscription.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// When the notification subscription was created:
        /// The number of milliseconds since 00:00:00 Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus leap seconds
        /// </summary>
        public long CreatedTime { get; set; }

        /// <summary>
        /// When the notification subscription was last updated:
        /// The number of milliseconds since 00:00:00 Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus leap seconds.
        /// </summary>
        public long LastUpdatedTime { get; set; }

        /// <summary>
        /// Id of the transformation for which notifications are sent.
        /// </summary>
        public long TransformationId { get; set; }

        /// <summary>
        /// E-mail address to send notifications to.
        /// </summary>
        public string Destination { get; set; }
    }
}
