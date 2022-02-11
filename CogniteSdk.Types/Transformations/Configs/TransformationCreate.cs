// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk
{
    /// <summary>
    /// Object to create a new transformation.
    /// </summary>
    public class TransformationCreate
    {
        /// <summary>
        /// Name of transformation.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// External id of transformation.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Transformation SQL query.
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Object describing what the result of the transformation is written to.
        /// </summary>
        public TransformationDestination Destination { get; set; }

        /// <summary>
        /// How to handle when data already exists in the destination.
        /// </summary>
        public TransformationConflictMode? ConflictMode { get; set; }

        /// <summary>
        /// How NULL values are ignored on update. If true, they are ignored, otherwise the destination
        /// field is set to null.
        /// </summary>
        public bool IgnoreNullFields { get; set; }

        /// <summary>
        /// True if transformation is visible to all in project, false if only to owner.
        /// </summary>
        public bool? IsPublic { get; set; }

        /// <summary>
        /// Api key used for reading from source.
        /// </summary>
        public string SourceApiKey { get; set; }

        /// <summary>
        /// Api key used for writing to destination.
        /// </summary>
        public string DestinationApiKey { get; set; }

        /// <summary>
        /// Oidc credentials used for reading from source.
        /// </summary>
        public TransformationOidcCredentials SourceOidcCredentials { get; set; }

        /// <summary>
        /// Oidc credentials used for writing to destination.
        /// </summary>
        public TransformationOidcCredentials DestinationOidcCredentials { get; set; }
    }
}
