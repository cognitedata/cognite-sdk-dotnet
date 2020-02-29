// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0


using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The file update DTO.
    /// </summary>
    public class FileUpdate
    {
        /// <summary>
        /// Set a new value for the string, or remove the value.
        /// </summary>
        public UpdateNullable<string> ExternalId { get; set; }

        /// <summary>
        /// Set a new value for the string, or remove the value.
        /// </summary>
        public UpdateNullable<long?> DataSetId { get; set; }

        /// <summary>
        /// Set a new value for the string, or remove the value.
        /// </summary>
        public UpdateNullable<string> Source { get; set; }

        /// <summary>
        /// Change that will be applied to the array.
        /// </summary>
        public UpdateEnumerable<long> AssetIds { get; set; }

        /// <summary>
        /// Custom, application specific metadata. String key -> String value. Limits: Maximum length of key is 32
        /// bytes, value 512 bytes, up to 16 key-value pairs.
        /// </summary>
        public UpdateDictionary<string> Metadata { get; set; }

        /// <summary>
        /// Set a new value for the string, or remove the value.
        /// </summary>
        public UpdateNullable<string> MimeType { get; set; }

        /// <summary>
        /// Set a new value for the start time, or remove the value.
        /// </summary>
        public UpdateNullable<long?> SourceCreatedTime { get; set; }

        /// <summary>
        /// Set a new value for the end time, or remove the value.
        /// </summary>
        public UpdateNullable<long?> SourceModifiedTime { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }

    /// <summary>
    /// The file update item DTO. Contains the update item for an <see cref="FileUpdate">FileUpdate</see>.
    /// </summary>
    public class FileUpdateItem : UpdateItem<FileUpdate>
    {
        /// <summary>
        /// Initialize the file update item with an external Id.
        /// </summary>
        /// <param name="externalId">External Id to set.</param>
        public FileUpdateItem(string externalId) : base(externalId)
        {
        }

        /// <summary>
        /// Initialize the file update item with an internal Id.
        /// </summary>
        /// <param name="id">Internal Id to set.</param>
        public FileUpdateItem(long id) : base(id)
        {
        }
    }
}