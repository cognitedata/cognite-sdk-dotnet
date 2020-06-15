    // Copyright 2020 Cognite AS
    // SPDX-License-Identifier: Apache-2.0
    using System;
    using System.Collections.Generic;

    using CogniteSdk.Types.Common;

    namespace CogniteSdk
    {
        /// <summary    >
        /// The functionCallLogEntry read class.
        /// </summary>
        public class FunctionCallLogEntry
        {
            /// <summary>
            /// Timestamp for the log entry.
            /// </summary>
            public long Timestamp { get; set; }

            /// <summary>
            /// Single line from stdout / stderr.
            /// </summary>
            public string Message { get; set; }

            /// <inheritdoc />
            public override string ToString() => Stringable.ToString(this);
        }
    }

