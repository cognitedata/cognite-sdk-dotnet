// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk
{
    /// <summary>
    /// List notifications.
    /// </summary>
    public class TransformationNotificationQuery : CursorQueryBase
    {
        /// <summary>
        /// List only notifications for the specified transformation by internal id.
        /// </summary>
        public long? TransformationId { get; set; }

        /// <summary>
        /// List only notifications for the specified transformation by external id.
        /// </summary>
        public string TransformationExternalId { get; set; }

        /// <summary>
        /// Filter by notification destination.
        /// </summary>
        public string Destination { get; set; }

        /// <inheritdoc />
        public override List<(string, string)> ToQueryParams()
        {
            var list = base.ToQueryParams();
            if (TransformationId.HasValue)
                list.Add(("transformationId", TransformationId.ToString()));
            else if (TransformationExternalId != null)
                list.Add(("transformationExternalId", TransformationExternalId));
            if (Destination != null)
                list.Add(("destination", Destination));

            return list;
        }
    }
}
