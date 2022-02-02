// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk
{
    /// <summary>
    /// Filter on transformation configurations.
    /// </summary>
    public class TransformationFilter
    {
        /// <summary>
        /// Require transformations to have IsPublic equal to this.
        /// </summary>
        public bool? IsPublic { get; set; }

        /// <summary>
        /// Require transformations to match this regular expression.
        /// </summary>
        public string NameRegex { get; set; }

        /// <summary>
        /// Require transformations to match this regular expression.
        /// </summary>
        public string QueryRegex { get; set; }

        /// <summary>
        /// Require transformations to have destination type equal to this.
        /// </summary>
        public TransformationDestinationType? DestinationType { get; set; }

        /// <summary>
        /// Require transformations to have conflict mode equal to this.
        /// </summary>
        public TransformationConflictMode? ConflictMode { get; set; }

        /// <summary>
        /// Require transformations to be or not be blocked according to this.
        /// </summary>
        public bool? HasBlockedError { get; set; }

        /// <summary>
        /// Require transformations to belong to this project.
        /// </summary>
        public string CdfProjectName { get; set; }

        /// <summary>
        /// Require transformations to be created inside this (inclusive) time range.
        /// </summary>
        public TimeRange CreatedTime { get; set; }

        /// <summary>
        /// Require transformations to be last updated inside this (inclusive) time range.
        /// </summary>
        public TimeRange LastUpdatedTime { get; set; }
    }
}
