﻿// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk
{
    /// <summary>
    /// Query for listing transformation schedules.
    /// </summary>
    public class TransformationScheduleQuery : CursorQueryBase
    {
        /// <summary>
        /// Whether public transformations should be included in the results. Default is true.
        /// </summary>
        public bool? IncludePublic { get; set; }

        /// <inheritdoc />
        public override List<(string, string)> ToQueryParams()
        {
            var list = base.ToQueryParams();
            if (IncludePublic.HasValue)
                list.Add(("includePublic", IncludePublic.ToString()));

            return list;
        }
    }
}
