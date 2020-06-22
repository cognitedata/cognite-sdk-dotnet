// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0
using System.Collections.Generic;

using CogniteSdk.Types.Common;


namespace CogniteSdk
{
    /// <summary>
    ///
    /// </summary>
    public class LabelContainsAnyFilter {
        /// <summary>
        ///
        /// </summary>
        public IEnumerable<CogniteExternalId> ContainsAny { get;set; }
        /// <summary>
        ///
        /// </summary>
        public LabelContainsAnyFilter(){}
        /// <summary>
        ///
        /// </summary>
        public LabelContainsAnyFilter(IEnumerable<CogniteExternalId> labels){
            ContainsAny = labels;
        }
    }
    /// <summary>
    ///
    /// </summary>
    public class LabelContainsAllFilter {
        /// <summary>
        ///
        /// </summary>
        public IEnumerable<CogniteExternalId> ContainsAll { get;set; }
        /// <summary>
        ///
        /// </summary>
        public LabelContainsAllFilter(){}
        /// <summary>
        ///
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
        ///
        /// </summary>
        public LabelFilter(string containsFilter)
        {
            Contains = new CogniteExternalId(containsFilter);
        }

        /// <summary>
        ///
        /// </summary>
        public LabelFilter(CogniteExternalId containsFilter)
        {
            Contains = containsFilter;
        }

        /// <summary>
        ///
        /// </summary>
        public LabelFilter(LabelContainsAllFilter labels)
        {
            ContainsAll = labels.ContainsAll;
        }
        /// <summary>
        ///
        /// </summary>
        public LabelFilter(LabelContainsAnyFilter labels)
        {
            ContainsAny = labels.ContainsAny;
        }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}

