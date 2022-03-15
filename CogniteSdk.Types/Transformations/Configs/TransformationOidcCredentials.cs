// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk
{
    /// <summary>
    /// Oidc credentials object for transformations.
    /// </summary>
    public class TransformationOidcCredentials
    {
        /// <summary>
        /// OIDC client id.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// OIDC client secret.
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Space separated list of scopes.
        /// </summary>
        public string Scopes { get; set; }

        /// <summary>
        /// OIDC token uri.
        /// </summary>
        public string TokenUri { get; set; }

        /// <summary>
        /// Cdf project name.
        /// </summary>
        public string CdfProjectName { get; set; }

        /// <summary>
        /// OIDC audience.
        /// </summary>
        public string Audience { get; set; }
    }
}
