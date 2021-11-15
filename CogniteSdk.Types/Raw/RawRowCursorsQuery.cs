// Copyright 2021 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Globalization;

namespace CogniteSdk
{
    /// <summary>
    /// Query for fetching cursors for parallel read
    /// </summary>
    public class RawRowCursorsQuery : IQueryParams
    {
        /// <summary>
        /// The number of cursors to return, between 1 and 10000. By default it's 10.
        /// </summary>
        public int? NumberOfCursors { get; set; }
        /// <summary>
        /// It is an exclusive filter, specifed as the number of milliseconds that have elapsed since 00:00:00 Thursday,
        /// 1 January 1970, Coordinated Universal Time (UTC), minus leap seconds.
        /// </summary>
        public long? MinLastUpdatedTime { get; set; }

        /// <summary>
        /// It is an inclusive filter, specified as the number of milliseconds that have elapsed since 00:00:00 Thursday,
        /// 1 January 1970, Coordinated Universal Time (UTC), minus leap seconds.
        /// </summary>
        public long? MaxLastUpdatedTime { get; set; }

        /// <inheritdoc/>
        public List<(string, string)> ToQueryParams()
        {
            var list = new List<(string, string)>();
            if (MinLastUpdatedTime.HasValue)
                list.Add(("minLastUpdatedTime", MinLastUpdatedTime.Value.ToString(CultureInfo.InvariantCulture)));
            if (MaxLastUpdatedTime.HasValue)
                list.Add(("maxLastUpdatedTime", MaxLastUpdatedTime.Value.ToString(CultureInfo.InvariantCulture)));
            if (NumberOfCursors.HasValue)
                list.Add(("numberOfCursors", NumberOfCursors.Value.ToString(CultureInfo.InvariantCulture)));

            return list;
        }
    }
}
