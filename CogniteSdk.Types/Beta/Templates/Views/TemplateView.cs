// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Class representing a retrieved template view
    /// </summary>
    public class TemplateView<T>
    {
        /// <summary>
        /// External id
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Optional data set id.
        /// </summary>
        public long? DataSetId { get; set; }

        /// <summary>
        /// Template view source.
        /// </summary>
        public TemplateViewSource<T> Source { get; set; }

        /// <summary>
        /// Time this instance was created in milliseconds since 01/01/1970
        /// </summary>
        public long CreatedTime { get; set; }

        /// <summary>
        /// Time this instance was last updated in milliseconds since 01/01/1970
        /// </summary>
        public long LastUpdatedTime { get; set; }
    }
}
