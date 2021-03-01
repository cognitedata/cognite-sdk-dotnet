// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk
{
    /// <summary>
    /// The data set update class.
    /// </summary>
    public class DataSetUpdate
    {
        /// <summary>
        /// Set a new value for externalId, or remove the value.
        /// </summary>
        public UpdateNullable<string> ExternalId { get; set; }
        /// <summary>
        /// Set a new value for name, or remove the value.
        /// </summary>
        public UpdateNullable<string> Name { get; set; }
        /// <summary>
        /// Set a new value for description, or remove the value
        /// </summary>
        public UpdateNullable<string> Description { get; set; }
        /// <summary>
        /// Custom, application specific metadata. String key -> String value.
        /// </summary>
        public UpdateDictionary<string> Metadata { get; set; }
        /// <summary>
        /// Set a new value for the writeProtected field.
        /// </summary>
        public Update<bool> WriteProtected { get; set; }
    }

    /// <summary>
    /// The data set update item type. Contains the update item for an <see cref="DataSetUpdate">DataSetUpdate</see>.
    /// </summary>
    public class DataSetUpdateItem : UpdateItem<DataSet>
    {
        /// <summary>
        /// Initialize the data set update item with an external Id.
        /// </summary>
        /// <param name="externalId">External Id to set.</param>
        public DataSetUpdateItem(string externalId) : base(externalId)
        {
        }

        /// <summary>
        /// Initialize the data set update item with an internal Id.
        /// </summary>
        /// <param name="id">Internal Id to set.</param>
        public DataSetUpdateItem(long id) : base(id)
        {
        }
    }
}
