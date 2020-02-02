// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Globalization;
using CogniteSdk.Types.Common;

namespace CogniteSdk.Raw
{
    /// <summary>
    /// The row query DTO.
    /// </summary>
    public class RowQueryDto : CursorQueryBase
    {
        /// <summary>
        /// Example: columns=column1,column2 Ordered list of column keys, separated by commas. Leave empty for all, use
        /// single comma to retrieve only row keys.
        /// </summary>
        public IEnumerable<string> Columns { get; set; }

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
        public new List<(string, string)> ToQueryParams()
        {
            var list = base.ToQueryParams();
            if (Columns != null)
                list.Add(("columns", string.Join(",", Columns)));
            if (MinLastUpdatedTime.HasValue)
                list.Add(("minLastUpdatedTime", MinLastUpdatedTime.Value.ToString(CultureInfo.InvariantCulture)));
            if (MaxLastUpdatedTime.HasValue)
                list.Add(("maxLastUpdatedTime", MaxLastUpdatedTime.Value.ToString(CultureInfo.InvariantCulture)));

            return list;
        }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<RowQueryDto>(this);
    }
}