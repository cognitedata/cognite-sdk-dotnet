// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The Label read class.
    /// </summary>
    public class Label
    {
        public string PlaceHolder { get; set;}
    }

    /// <summary>
    /// The LabelList read class.
    /// </summary>
    public class LabelList
    {

    /// <summary>
    /// List of externalIds.
    /// </summary>
        public List<string> ExternalId { get; set; }
    }

    public class LabelFilter
    {
        /// <summary>
        /// Labelfilter consists of a list of OrFilters with AndFilters as subfilters
        /// </summary>
        // Another way to look at this is that it is on the form OrFilters<AndFilters<ExternalId>>
        public List<List<string>> Filter { get; set; }
    }
}

