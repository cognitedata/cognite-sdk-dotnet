// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The Label read class.
    /// </summary>
    [System.Obsolete("The Label class is under development, and currently only available for use in playground")]
    public class Label
    {
        /// <summary>
        /// Placeholder Label Class
        /// </summary>
        public string ExternalId { get; set;}

        /// <summary>
        /// Empty constructor method for Label type
        /// </summary>
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

