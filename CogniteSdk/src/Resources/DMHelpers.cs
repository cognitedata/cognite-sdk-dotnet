// Copyright 2026 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using CogniteSdk.DataModels;

namespace CogniteSdk.Resources
{
    /// <summary>
    /// Shared helpers for beta core data model resources.
    /// </summary>
    internal static class DMHelpers
    {
        /// <summary>
        /// Extract properties for the given view from the nested space/source dictionary
        /// returned by the instances API.
        /// </summary>
        /// <param name="properties">Nested properties dictionary, keyed by space then by "externalId/version".</param>
        /// <param name="view">View to extract properties for.</param>
        internal static TResult GetFromNestedDicts<TResult>(Dictionary<string, Dictionary<string, TResult>> properties, ViewIdentifier view)
        {
            if (properties is null || !properties.TryGetValue(view.Space, out var bySource))
            {
                return default;
            }
            if (bySource == null)
            {
                System.Diagnostics.Trace.TraceWarning("Source was null when extracting nested view properties {prop}", view);
                return default;
            }

            if (!bySource.TryGetValue($"{view.ExternalId}/{view.Version}", out var v))
            {
                return default;
            }
            return v;
        }
    }
}
