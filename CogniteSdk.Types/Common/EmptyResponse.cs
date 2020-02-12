// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk
{
    /// <summary>
    /// Empty object, by design contains no properties. Used for parsing the delete response.
    /// </summary>
    public class EmptyResponse {
        /// <inheritdoc />
        public override string ToString() => "{ }";
    }
}