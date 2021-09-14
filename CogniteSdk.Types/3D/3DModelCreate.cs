// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The ThreeDModel create class.
    /// </summary>
    public class ThreeDModelCreate
    {
        /// <summary>
        /// The name of the model.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// The DataSetId of the 3D Model.
        /// </summary>
        public long? DataSetId { get; set; }

        /// <summary>
        /// Custom, application specific metadata. String key -> String value
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "System.Text.Json ignores properties that don't have setters")]
        public Dictionary<string, string> Metadata { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}

