// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Query for listing subscription members.
    /// </summary>
    public class ListSubscriptionMembers : CursorQueryBase
    {
        /// <summary>
        /// External ID of the subscription
        /// </summary>
        public string ExternalId { get; set; }

        /// <inheritdoc />
        public override List<(string, string)> ToQueryParams()
        {
            var pairs = base.ToQueryParams();
            pairs.Add(("externalId", ExternalId));
            return pairs;
        }
    }

    /// <summary>
    /// Query for listing subscriptions
    /// </summary>
    public class ListSubscriptions : CursorQueryBase { }
}
