// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CogniteSdk.Assets
{
    /// <summary>
    /// The Asset read DTO.
    /// </summary>
    public class AssetReadDto
    {
        /// <summary>
        /// External Id provided by client. Must be unique within the project.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// The name of the asset.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The parent ID of the asset.
        /// </summary>
        public long? ParentId { get; set; }

        /// <summary>
        /// The description of the asset.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Javascript friendly internal ID given to the object.
        /// </summary>
        public long? DataSetId { get; set; }

        /// <summary>
        /// Custom, application specific metadata. String key -> String value
        /// </summary>
        public IDictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// The source of this asset.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// The Id of the asset.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Time when this asset was created in CDF in milliseconds since Jan 1, 1970.
        /// </summary>
        public long CreatedTime { get; set; }

        /// <summary>
        /// The last time this asset was updated in CDF, in milliseconds since Jan 1, 1970.
        /// </summary>
        public long LastUpdatedTime { get; set; }

        /// <summary>
        /// InternalId of the root object.
        /// </summary>
        public long RootId { get; set; }

        /// <summary>
        /// External Id of parent asset provided by client. Must be unique within the project.
        /// </summary>
        public string ParentExternalId { get; set; }

        /// <summary>
        /// Aggregated metrics of the asset.
        /// </summary>
        public AggregateResult Aggregates { get; set; }

        /// <summary>
        /// Return user friendly string representation of the object.
        /// </summary>
        /// <returns>String representation.</returns>
        public override string ToString()
        {
            var props = new List<string> { "{"};
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(this))
            {
                var name = descriptor.Name;
                object value = descriptor.GetValue(this);
                value ??= "null";
                props.Add($"\t{name}={value}");
            }
            props.Add("}");

            return string.Join("\n", props);
        }
    }
}

