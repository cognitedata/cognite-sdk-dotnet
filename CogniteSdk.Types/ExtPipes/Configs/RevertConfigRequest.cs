﻿// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk
{
    /// <summary>
    /// Request to revert a configuration to a previous revision.
    /// </summary>
    public class RevertConfigRequest
    {
        /// <summary>
        /// Extraction pipeline external id.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// Revision of config to revert to.
        /// </summary>
        public int Revision { get; set; }
    }
}
