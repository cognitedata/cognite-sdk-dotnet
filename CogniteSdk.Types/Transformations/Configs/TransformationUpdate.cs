// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk
{
    /// <summary>
    /// Update a transformation configuration.
    /// </summary>
    public class TransformationUpdate
    {
        /// <summary>
        /// Name of transformation.
        /// </summary>
        public Update<string> Name { get; set; }

        /// <summary>
        /// External id of transformation.
        /// </summary>
        public Update<string> ExternalId { get; set; }

        /// <summary>
        /// Transformation SQL query.
        /// </summary>
        public Update<string> Query { get; set; }

        /// <summary>
        /// Object describing what the result of the transformation is written to.
        /// </summary>
        public Update<TransformationDestination> Destination { get; set; }

        /// <summary>
        /// How to handle when data already exists in the destination.
        /// </summary>
        public Update<TransformationConflictMode> ConflictMode { get; set; }

        /// <summary>
        /// How NULL values are ignored on update. If true, they are ignored, otherwise the destination
        /// field is set to null.
        /// </summary>
        public Update<bool> IgnoreNullFields { get; set; }

        /// <summary>
        /// True if transformation is visible to all in project, false if only to owner.
        /// </summary>
        public Update<bool> IsPublic { get; set; }

        /// <summary>
        /// Api key used for reading from source.
        /// </summary>
        public UpdateNullable<string> SourceApiKey { get; set; }

        /// <summary>
        /// Api key used for writing to destination.
        /// </summary>
        public UpdateNullable<string> DestinationApiKey { get; set; }

        /// <summary>
        /// Oidc credentials used for reading from source.
        /// </summary>
        public UpdateNullable<TransformationOidcCredentials> SourceOidcCredentials { get; set; }

        /// <summary>
        /// Oidc credentials used for writing to destination.
        /// </summary>
        public UpdateNullable<TransformationOidcCredentials> DestinationOidcCredentials { get; set; }
    }

    /// <summary>
    /// Item containing an update to a transformation by internal or external id.
    /// </summary>
    public class TransformationUpdateItem : UpdateItem<TransformationUpdate>
    {
        /// <summary>
        /// Initialize transformation update item with external id.
        /// </summary>
        /// <param name="externalId">Transformation external id</param>
        public TransformationUpdateItem(string externalId) : base(externalId)
        {
        }

        /// <summary>
        /// Initialize transformation update item with internal id.
        /// </summary>
        /// <param name="internalId">Transformation internal id</param>
        public TransformationUpdateItem(long internalId) : base(internalId)
        {
        }
    }
}
