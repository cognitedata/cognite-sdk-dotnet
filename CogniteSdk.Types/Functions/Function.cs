// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections.Generic;

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The function read class.
    /// </summary>
    public class Function
    {
        /// <summary>
        /// A server-generated ID for the object.
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// The name of the function.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The file ID to a file uploaded to Cognite's Files API.
        /// This file must be a zip file and contain a file called handler.py.
        /// This file must contain a function named handle with any of
        /// following arguments: data, client and secrets,
        /// which are passed into the function. The zip file can
        /// contain other files as well (model binary data, libraries etc).
        /// Python packages are currently not allowed to be installed, but Cognite
        /// SDK's are available.
        /// </summary>
        public long FileId { get; set; }
        /// <summary>
        /// Owner of this function. Typically used to know who created it.
        /// </summary>
        public string Owner { get; set; }
        /// <summary>
        /// Description of the function.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// API key that can be used inside the function to access data in CDF.
        /// </summary>
        public string ApiKey { get; set; }
        /// <summary>
        /// Object with additional secrets as key/value pairs. These can
        /// e.g. password to simulators or other data sources. Keys must
        /// be lowercase characters, numbers or dashes (-) and at most 15
        /// characters. You can create at most 5 secrets, all keys must be
        /// unique, and cannot be apikey.
        /// </summary>
        public Dictionary<string, string> Secrets { get; set; }
        /// <summary>
        /// The number of milliseconds since 00:00:00 Thursday, 1 January 1970,
        /// Coordinated Universal Time (UTC), minus leap seconds.        /// </summary>
        public long CreatedTime { get; set; }
        /// <summary>
        /// Enum: "Queued" "Deploying" "Ready" "Failed"
        /// Status of the function. It starts in a Queued state,
        /// is then Deploying before it is either Ready or Failed.
        /// If the function is Ready, it can be called.
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// The external ID provided by the client. Must be unique for the resource type.
        /// </summary>
        public string ExternalId { get; set; }
        /// <summary>
        /// Error if function building failed.
        /// </summary>
        public FunctionError Error { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}

