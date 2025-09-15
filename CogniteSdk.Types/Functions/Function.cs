// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// Status of a function deployment.
    /// </summary>
    public enum FunctionStatus
    {
        /// <summary>
        /// The function is queued for deployment.
        /// </summary>
        Queued,
        /// <summary>
        /// Function is in the process of deploying.
        /// </summary>
        Deploying,
        /// <summary>
        /// Function is deployed and ready to be called.
        /// </summary>
        Ready,
        /// <summary>
        /// Function failed to deploy.
        /// </summary>
        Failed
    }

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
        /// Custom, application specific metadata. String key -> String value.
        /// Limits: Maximum length of key is 32, value 512 characters, up to 16 key-value pairs.
        /// Maximum size of entire metadata is 4096 bytes.
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// Object with additional secrets as key/value pairs. These can
        /// e.g. password to simulators or other data sources. Keys must
        /// be lowercase characters, numbers or dashes (-) and at most 15
        /// characters. You can create at most 5 secrets, all keys must be
        /// unique, and cannot be apikey.
        /// 
        /// The secrets are returned scrambled if set.
        /// </summary>
        public Dictionary<string, string> Secrets { get; set; }

        /// <summary>
        /// Object with environment variables as key/value pairs. Keys can contain only letters, 
        /// numbers or the underscore character. You can create at most 100 environment variables.
        /// </summary>
        public Dictionary<string, string> EnvVars { get; set; }

        /// <summary>
        /// Number of CPU cores per function. Allowed range and default value
        /// are given by the limits endpoint. On Azure, only the default value is used.
        /// </summary>
        /// <value></value>
        public float? Cpu { get; set; }

        /// <summary>
        /// Memory per function measured in GB. Allowed range and default value are given
        /// by the limits endpoint. On Azure, only the default value is used.
        /// </summary>
        public float? Memory { get; set; }

        /// <summary>
        /// The runtime of the function. For exmple,
        /// runtime "py38" translates to the latest version of the Python 3.8 series.
        /// 
        /// Defaults to the latest available python version.
        /// </summary>
        public string Runtime { get; set; }

        /// <summary>
        /// The complete specification of the function runtime with major,
        /// minor, and patch version numbers.
        /// </summary>
        /// <value></value>
        public string RuntimeVersion { get; set; }

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
        public FunctionStatus Status { get; set; }

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
