// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The description search class.
    /// </summary>
    public class DescriptionSearch
    {
        /// <summary>
        /// Search on Description.
        /// </summary>
        /// <value></value>
        public string Description { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }

    /// <summary>
    /// The event search class.
    /// </summary>
    public class EventSearch : SearchQuery<EventFilter, DescriptionSearch>
    {

        /// <summary>
        /// Create a new empty Event search DTO with pre-initialized emtpy Filter and Search.
        /// </summary>
        /// <returns>New instance of the EventSearch.</returns>
        public static EventSearch Empty()
        {
            return new EventSearch
            {
                Filter = new EventFilter(),
                Search = new DescriptionSearch()
            };
        }
    }
}
