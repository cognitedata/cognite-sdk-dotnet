// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;

namespace CogniteSdk.Raw
{
    public class RowQueryDto
    {
        /// <summary>
        /// Limits the number of results to return.
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Example: columns=column1,column2 Ordered list of column keys, separated by commas. Leave empty for all, use
        /// single comma to retrieve only row keys.
        /// </summary>
        public string Columns { get; set; }

        /// <summary>
        /// Cursor to next result page.
        /// </summary>
        public string Cursor { get; set; }

        /// <summary>
        /// It is an exclusive filter, specifed as the number of milliseconds that have elapsed since 00:00:00 Thursday,
        /// 1 January 1970, Coordinated Universal Time (UTC), minus leap seconds.
        /// </summary>
        public long MinLastUpdatedTime { get; set; }

        /// <summary>
        /// It is an inclusive filter, specified as the number of milliseconds that have elapsed since 00:00:00 Thursday,
        /// 1 January 1970, Coordinated Universal Time (UTC), minus leap seconds.
        /// </summary>
        public long MaxLastUpdatedTime { get; set; }

        public IEnumerable<Tuple<string, string>> ToQuery()
        {
            var list = new List<Tuple<string, string>>
            {
                // FIXME: 
                new Tuple<string, string>("cursor", Cursor)
            };
            return list;
        }
    }
}