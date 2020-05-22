// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The Label read class.
    /// </summary>
    public class Label
    {
        /// <summary>
        /// Placeholder Label Class
        /// </summary>
        public string ExternalId { get; set;}

        public Label()
        {
        }
        /// <summary>
        /// Constructor method for Label type
        /// </summary>
        public Label(string externalID)
        {
            ExternalId = externalID;
        }
    }
}

