// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk.Login
{
    /// <summary>
    /// The Login Status Read DTO.
    /// </summary>
    public class LoginStatusRead
    {
        /// <summary>
        /// The user principal, e.g john.doe@corporation.com.
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Whether the user is logged in or not.
        /// </summary>
        public bool LoggedIn { get; set; }

        /// <summary>
        /// Name of project user belongs to.
        /// </summary>
        public string Project { get; set; }

        /// <summary>
        /// Internal project id of the project
        /// </summary>
        public long ProjectId { get; set; }

        /// <summary>
        /// ID of the api key making the request. This is optional and only present if an api key is used as
        /// authentication.
        /// </summary>
        public long? ApiKeyId { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<LoginStatusRead>(this);
    }
}

