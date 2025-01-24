// Copyright 2025 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Alpha
{
    /// <summary>
    /// Create a new configuration revision.
    /// </summary>
    public class CreateConfigRevision
    {
        /// <summary>
        /// Integration external ID.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// Optional revision description. Should contain the list of changes.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The actual config revision contents.
        /// </summary>
        public string Config { get; set; }
    }

    /// <summary>
    /// Metadata about a configuration revision.
    /// </summary>
    public class ConfigRevisionMetadata
    {
        /// <summary>
        /// Integration external ID.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// Config revision number. This is simply an incrementing integer.
        /// </summary>
        public int Revision { get; set; }
        /// <summary>
        /// Optional revision description. Should contain the list of changes.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Time when this revision was created in milliseconds since Jan 1, 1970.
        /// </summary>
        public long CreatedTime { get; set; }
        /// <summary>
        /// Time when this revision was last updated in milliseconds since Jan 1, 1970.
        /// </summary>
        public long LastUpdatedTime { get; set; }
    }

    /// <summary>
    /// An integration configuration revision.
    /// </summary>
    public class ConfigRevision : ConfigRevisionMetadata
    {
        /// <summary>
        /// The actual config revision contents.
        /// </summary>
        public string Config { get; set; }
    }

    /// <summary>
    /// Query for retrieving integration config revisions.
    /// </summary>
    public class ConfigRevisionQuery : IQueryParams
    {
        /// <summary>
        /// Integration external ID, required.
        /// </summary>
        public string Integration { get; set; }
        /// <summary>
        /// Config revision to fetch. If left out, fetches the latest revision.
        /// </summary>
        public int? Revision { get; set; }

        /// <inheritdoc />
        public List<(string, string)> ToQueryParams()
        {
            var res = new List<(string, string)>
            {
                ("integration", Integration)
            };
            if (Revision.HasValue)
            {
                res.Add(("revision", Revision.Value.ToString()));
            }
            return res;
        }
    }



    /// <summary>
    /// Query for listing integration config revisions.
    /// </summary>
    public class ConfigRevisionsQuery : IQueryParams
    {
        /// <summary>
        /// Integration external ID.
        /// </summary>
        public string Integration { get; set; }

        /// <inheritdoc />
        public List<(string, string)> ToQueryParams()
        {
            var res = new List<(string, string)>();
            if (Integration != null)
            {
                res.Add(("integration", Integration));
            }
            return res;
        }
    }


}