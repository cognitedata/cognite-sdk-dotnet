// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0


using CogniteSdk.Types.Common;

namespace CogniteSdk.Files
{
    /// <summary>
    /// The file update DTO.
    /// </summary>
    public class FileUpdateDto
    {
        /// <summary>
        /// Set a new value for the string, or remove the value.
        /// </summary>
        public Update<string> ExternalId { get; set; }

        /// <summary>
        /// Set a new value for the string, or remove the value.
        /// </summary>
        public Update<string> Source { get; set; }

        /// <summary>
        /// Change that will be applied to the array.
        /// </summary>
        public SequenceUpdate<long> AssetIds { get; set; }

        /// <summary>
        /// Custom, application specific metadata. String key -> String value. Limits: Maximum length of key is 32
        /// bytes, value 512 bytes, up to 16 key-value pairs.
        /// </summary>
        public DictUpdate<string> Metadata { get; set; }

        /// <summary>
        /// Set a new value for the string, or remove the value.
        /// </summary>
        public Update<string> MimeType { get; set; }

        /// <summary>
        /// Set a new value for the start time, or remove the value.
        /// </summary>
        public Update<long?> SourceCreatedTime { get; set; }

        /// <summary>
        /// Set a new value for the end time, or remove the value.
        /// </summary>
        public Update<long?> SourceModifiedTime { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<FileUpdateDto>(this);
    }

    /// <summary>
    /// The file update item DTO. Contains the update item for an <see cref="FileUpdateDto">FileUpdateDto</see>.
    /// </summary>
    public class FileUpdateItem : UpdateItem<FileUpdateDto> {
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