// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;


namespace CogniteSdk
{
    /// <summary>
    /// The sequence update class.
    /// </summary>
    public class SequenceUpdate
    {
        /// <summary>
        /// Set a new value for the Name, or remove the value.
        /// </summary>
        public UpdateNullable<string> Name { get; set; }

        /// <summary>
        /// Set a new value for the Description, or remove the value.
        /// </summary>
        public UpdateNullable<string> Description { get; set; }

        /// <summary>
        /// Set a new value for the assetId, or remove the value.
        /// </summary>
        public UpdateNullable<long?> AssetId { get; set; }

        /// <summary>
        /// Set a new value for the ExternalId, or remove the value.
        /// </summary>
        public UpdateNullable<string> ExternalId { get; set; }

        /// <summary>
        /// Set a new value for the string, or remove the value.
        /// </summary>
        public UpdateNullable<long?> DataSetId { get; set; }

        /// <summary>
        /// Custom, application specific metadata. String key -> String value. Limits: Maximum length of key is 32
        /// bytes, value 512 bytes, up to 16 key-value pairs.
        /// </summary>
        public UpdateDictionary<string> Metadata { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}