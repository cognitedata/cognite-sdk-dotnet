// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0
using CogniteSdk.Types.Common;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CogniteSdk
{
    /// <summary>
    /// The DataSet read class
    /// </summary>
    public class DataSet
    {
        /// <summary>
        /// The external ID provided by the client. Must be unique within the project.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// The name of the data set.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The description of the data set.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Custom, application specific metadata. String key -> String value.
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; }
        /// <summary>
        /// To write data to a write-protected data set,
        /// you need to be a member of a group that has the "datasets:owner" action for the data set.
        /// </summary>
        public bool WriteProtected { get; set; }
        /// <summary>
        /// The Id of this data set.
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Time when this data set was created in CDF in milliseconds since Jan 1, 1970.
        /// </summary>
        public long CreatedTime { get; set; }
        /// <summary>
        /// The last time this data set was updated in CDF, in milliseconds since Jan 1, 1970.
        /// </summary>
        public long LastUpdatedTime { get; set; }
        /// <summary>Determines whether the specified object is equal to the current object, using internalId</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return obj is DataSet dto && dto.Id == Id;
        }
        /// <summary>
        /// Creates a hash code based on the Id attribute.
        /// </summary>
        /// <returns>A hash code for the current object</returns>
        public override int GetHashCode()
        {
            return 2108858624 + Id.GetHashCode();
        }
        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
    /// <summary>
    /// Data set read class (without metadata).
    /// </summary>
    public class DataSetWithoutMetadata : DataSet
    {
        /// <summary>
        /// Custom, application specific metadata. String key -> String value
        /// </summary>
        [JsonIgnore]
        public new Dictionary<string, string> Metadata { get; set; }
    }
}
