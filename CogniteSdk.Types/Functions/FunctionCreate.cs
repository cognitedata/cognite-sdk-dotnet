using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The function write class.
    /// </summary>
    public class FunctionCreate
    {
        /// <summary>
        /// The name of the function.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The external ID provided by the client. Must be unique.
        /// </summary>
        public string ExternalId { get; set; }

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
        /// </summary>
        public Dictionary<string, string> Secrets { get; set; }

        /// <summary>
        /// Relative path from the root folder to the file containing the handle function.
        /// Defaults to handler.py. Must be on POSIX path format.
        /// </summary>
        public string FunctionPath { get; set; }

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
        /// Specify a different python package index, allowing for packages published
        /// in private repositories. Supports basic HTTP authentication as described
        /// in pip basic authentication. See the documentation for additional
        /// information related to the security risks of using this option.
        /// </summary>
        public string IndexUrl { get; set; }

        /// <summary>
        /// Extra package index URLs to use when building the function,
        /// allowing for packages published in private repositories.
        /// Supports basic HTTP authentication as described in pip basic authentication.
        /// See the documentation for additional information related
        /// to the security risks of using this option.
        /// </summary>
        public IEnumerable<string> ExtraIndexUrls { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return Stringable.ToString(this);
        }
    }

}
