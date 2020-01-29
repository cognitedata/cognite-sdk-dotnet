// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

namespace CogniteSdk.Events
{
    /// <summary>
    /// Dto for description search
    /// </summary>
    public class DescriptionSearchDto
    {
        /// <summary>
        /// Search on Description.
        /// </summary>
        /// <value></value>
        public string Description { get; set; }
    }

    /// <summary>
    /// The event search DTO.
    /// </summary>
    public class EventSearchDto : SearchQueryDto<EventFilterDto, DescriptionSearchDto> { }
}