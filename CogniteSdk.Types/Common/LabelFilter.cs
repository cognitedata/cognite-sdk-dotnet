// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0
using System.Collections.Generic;

using CogniteSdk.Types.Common;


namespace CogniteSdk
{
    /// <summary>
    /// The LabelContainsAnyFilter class. When instantiated can be fed to LabelFilter to create a multi-label OR filter.
    /// </summary>
    public class LabelContainsAnyFilter {
        /// <summary>
        /// Match all elements that contains any of the provided labels (CogniteExternalIds).
        /// </summary>
        public IEnumerable<CogniteExternalId> ContainsAny { get;set; }
        /// <summary>
        /// Default constructor.
        /// </summary>
        public LabelContainsAnyFilter(){}
        /// <summary>
        /// Create a LabelContainsAnyFilter with the provided set of labels (CogniteExternalIds)
        /// </summary>
        public LabelContainsAnyFilter(IEnumerable<CogniteExternalId> labels){
            ContainsAny = labels;
        }
    }
    /// <summary>
    /// The LabelContainsAllFilter class. When instantiated can be fed to LabelFilter to create a multi-label AND filter.
    /// </summary>
    public class LabelContainsAllFilter {
        /// <summary>
        /// Match only elements that contains all provided labels (CogniteExternalIds).
        /// </summary>
        public IEnumerable<CogniteExternalId> ContainsAll { get;set; }
        /// <summary>
        /// Default constructor.
        /// </summary>
        public LabelContainsAllFilter(){}
        /// <summary>
        /// Create a LabelContainsAllFilter with the provided set of labels (CogniteExternalIds)
        /// </summary>
        public LabelContainsAllFilter(IEnumerable<CogniteExternalId> labels){
            ContainsAll = labels;
        }
    }

    /// <summary>
    /// The Cognite Label filter class.
    /// </summary>
    [System.Obsolete("The ExternalId class is under development, and currently only available for use in playground")]
    public class LabelFilter
    {
        /// <summary>
        /// Filter labels that contains a single label
        /// </summary>
        public CogniteExternalId Contains { get; set; }
        /// <summary>
        /// Filter labels that contains any of the given labels (OR-filter)
        /// </summary>
        public IEnumerable<CogniteExternalId> ContainsAny { get; set; }
        /// <summary>
        /// Filter labels that contains all of the given labels (AND-filter)
        /// </summary>
        public IEnumerable<CogniteExternalId> ContainsAll { get; set; }

        /// <summary>
        /// LabelFilter with string parameter creates a single label filter
        /// </summary>
        public LabelFilter(string containsFilter)
        {
            Contains = new CogniteExternalId(containsFilter);
        }

        /// <summary>
        /// LabelFilter with CogniteExternalID parameter creates a single label filter
        /// </summary>
        public LabelFilter(CogniteExternalId containsFilter)
        {
            Contains = containsFilter;
        }

        /// <summary>
        /// LabelFilter with LabelContainsAllFilter parameter creates a multilabel AND-filter
        /// </summary>
        public LabelFilter(LabelContainsAllFilter labels)
        {
            ContainsAll = labels.ContainsAll;
        }
        /// <summary>
        /// LabelFilter with LabelContainsAnyFilter parameter creates a multilabel OR-filter
        /// </summary>
        public LabelFilter(LabelContainsAnyFilter labels)
        {
            ContainsAny = labels.ContainsAny;
        }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}

