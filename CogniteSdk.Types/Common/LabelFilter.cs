// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Linq;

using CogniteSdk.Types.Common;


namespace CogniteSdk
{
    /// <summary>
    /// The LabelContainsAnyFilter class. When instantiated can be fed to LabelFilter to create a multi-label OR filter.
    /// </summary>
    public class LabelContainsAnyFilter
    {
        /// <summary>
        /// Match all elements that contains any of the provided labels (CogniteExternalIds).
        /// </summary>
        public IEnumerable<CogniteExternalId> ContainsAny { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public LabelContainsAnyFilter() { }

        /// <summary>
        /// Create a LabelContainsAnyFilter with the provided set of labels (CogniteExternalIds).
        /// </summary>
        public LabelContainsAnyFilter(IEnumerable<CogniteExternalId> labels)
        {
            ContainsAny = labels;
        }

        /// <summary>
        /// Create a LabelContainsAnyFilter with the provided set of labels (strings).
        /// </summary>
        public LabelContainsAnyFilter(IEnumerable<string> labels)
        {
            ContainsAny = labels.Select(label => new CogniteExternalId(label));
        }
    }

    /// <summary>
    /// The LabelContainsAllFilter class. When instantiated can be fed to LabelFilter to create a multi-label AND filter.
    /// </summary>
    public class LabelContainsAllFilter
    {
        /// <summary>
        /// Match only elements that contains all provided labels (CogniteExternalIds).
        /// </summary>
        public IEnumerable<CogniteExternalId> ContainsAll { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public LabelContainsAllFilter() { }

        /// <summary>
        /// Create a LabelContainsAllFilter with the provided set of labels (CogniteExternalIds).
        /// </summary>
        public LabelContainsAllFilter(IEnumerable<CogniteExternalId> labels)
        {
            ContainsAll = labels;
        }

        /// <summary>
        /// Create a LabelContainsAllFilter with the provided set of labels (strings).
        /// </summary>
        public LabelContainsAllFilter(IEnumerable<string> labels)
        {
            ContainsAll = labels.Select(label => new CogniteExternalId(label));
        }
    }

    /// <summary>
    /// The Cognite Label filter class.
    /// </summary>
    public class LabelFilter
    {
        /// <summary>
        /// Filter labels that contains any of the given labels (OR-filter).
        /// </summary>
        public IEnumerable<CogniteExternalId> ContainsAny { get; set; }

        /// <summary>
        /// Filter labels that contains all of the given labels (AND-filter).
        /// </summary>
        public IEnumerable<CogniteExternalId> ContainsAll { get; set; }

        /// <summary>
        /// LabelFilter with LabelContainsAllFilter parameter creates a multilabel AND-filter.
        /// </summary>
        public LabelFilter(LabelContainsAllFilter labels)
        {
            ContainsAll = labels.ContainsAll;
        }

        /// <summary>
        /// LabelFilter with LabelContainsAnyFilter parameter creates a multilabel OR-filter.
        /// </summary>
        public LabelFilter(LabelContainsAnyFilter labels)
        {
            ContainsAny = labels.ContainsAny;
        }

        /// <summary>
        /// Create LabelsContainsAll filter from string of externalIds.
        /// </summary>
        /// <param name="labels">Sequence of label externalIds.</param>
        /// <returns>Label filter.</returns>
        public static LabelFilter All(IEnumerable<string> labels)
        {
            return new LabelFilter(new LabelContainsAllFilter(labels));
        }

        /// <summary>
        /// Create LabelsContainsAll filter from externalId string.
        /// </summary>
        /// <param name="label">label externalId.</param>
        /// <returns>Label filter.</returns>
        public static LabelFilter All(string label)
        {
            return new LabelFilter(new LabelContainsAllFilter(new[] { label }));
        }

        /// <summary>
        /// Create LabelsContainsAll filter from string of externalIds.
        /// </summary>
        /// <param name="labels">Sequence of label externalIds.</param>
        /// <returns>Label filter.</returns>
        public static LabelFilter Any(IEnumerable<string> labels)
        {
            return new LabelFilter(new LabelContainsAnyFilter(labels));
        }

        /// <summary>
        /// Create LabelsContainsAny filter from externalId string.
        /// </summary>
        /// <param name="label">label externalId.</param>
        /// <returns>Label filter.</returns>
        public static LabelFilter Any(string label)
        {
            return new LabelFilter(new LabelContainsAnyFilter(new[] { label }));
        }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}

