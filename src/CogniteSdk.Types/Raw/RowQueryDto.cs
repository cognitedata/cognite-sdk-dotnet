// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;

namespace CogniteSdk.Raw
{
    public class RowQueryDto : CursorQueryBase
    {
        /// <summary>
        /// Example: columns=column1,column2 Ordered list of column keys, separated by commas. Leave empty for all, use
        /// single comma to retrieve only row keys.
        /// </summary>
        public string Columns { get; set; }

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

        public new List<(string, string)> ToQueryParams()
        {
            var list = base.ToQueryParams();
            if (Columns != null)
                list.Add(("columns", Columns));
            if (MinLastUpdatedTime.HasValue)
                list.Add(("minLastUpdatedTime", MinLastUpdatedTime.Value.ToString()));
            if (MaxLastUpdatedTime.HasValue)
                list.Add(("maxLastUpdatedTime", MaxLastUpdatedTime.Value.ToString()));

            return list;
        }
    }
}