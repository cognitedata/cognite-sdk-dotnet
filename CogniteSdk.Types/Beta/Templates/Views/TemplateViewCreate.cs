// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using System.Text.Json;

namespace CogniteSdk.Beta
{
    /// <summary>
    /// Source of a template view
    /// </summary>
    public class TemplateViewSource<T>
    {
        /// <summary>
        /// Template field mappings
        /// </summary>
        public Dictionary<string, string> Mappings { get; set; }

        /// <summary>
        /// View type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Filter on data
        /// </summary>
        public T Filter { get; set; }
    }

    /// <summary>
    /// Create a template view
    /// </summary>
    public class TemplateViewCreate<T>
    {
        /// <summary>
        /// External id
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Optional data set id.
        /// </summary>
        public long? DataSetId { get; set; }

        /// <summary>
        /// Template view source.
        /// </summary>
        public TemplateViewSource<T> Source { get; set; }
    }
}
