// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Types.Assets
{
    public class AssetQuery
    {
        /// <summary>
        /// Filter on assets with strict matching.
        /// </summary>
        public AssetFilterDto Filter { get; set; }

        /// <summary>
        /// Limits the number of results to return.
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Cursor to next result page.
        /// </summary>
        public string Cursor { get; set; }

        /// <summary>
        /// Set of aggregated properties to include.
        /// </summary>
        public IEnumerable<string> AggregatedProperties { get; set; }

        /// <summary>
        /// Splits the data set into N partitions. You need to follow the cursors within each partition in order to
        /// receive all the data. Example: 1/10.
        /// </summary>
        public string Partition { get; set; }
    }
}