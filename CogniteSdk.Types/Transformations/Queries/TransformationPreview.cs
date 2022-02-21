// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk
{
    /// <summary>
    /// Run a transformation preview.
    /// </summary>
    public class TransformationPreview
    {
        /// <summary>
        /// SQL query to run for preview.
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// True to stringify values in query result.
        /// </summary>
        public bool ConvertToString { get; set; }

        /// <summary>
        /// End-result limit of the query. Default is 1000.
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Limit for how many rows to download from the data sources. Default is 1000.
        /// </summary>
        public int? SourceLimit { get; set; }

        /// <summary>
        /// Limit for how many rows that are used for inferring schema. Default is 10,000.
        /// </summary>
        public int? InferSchemaLimit { get; set; }
    }
}
