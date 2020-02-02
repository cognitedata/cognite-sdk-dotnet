// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

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

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString<DescriptionSearchDto>(this);
    }

    /// <summary>
    /// The event search DTO.
    /// </summary>
    public class EventSearchDto : SearchQueryDto<EventFilterDto, DescriptionSearchDto> {

        /// <summary>
        /// Create a new empty Event search DTO with pre-initialized emtpy Filter and Search.
        /// </summary>
        /// <returns>New instance of the EventSearchDto.</returns>
        public static EventSearchDto Empty ()
        {
            return new EventSearchDto {
                Filter=new EventFilterDto(),
                Search=new DescriptionSearchDto()
            };
        }
    }
}