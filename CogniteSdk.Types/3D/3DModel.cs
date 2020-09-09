// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;

using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The ThreeDModel read class.
    /// </summary>
    public class ThreeDModel
    {
        /// <summary>
        /// The Id of the 3D Model.
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// The name of the model.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The creation time of the 3D model.
        /// </summary>
        public long CreatedTime { get; set; }

        /// <summary>
        /// Custom, application specific metadata. String key -> String value
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "System.Text.Json ignores properties that don't have setters")]
        public Dictionary<string, string> Metadata { get; set; }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return obj is ThreeDModel dto &&
                   Id == dto.Id;
        }

        /// <summary>Serves as the default hash function.</summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return 2108858624 + Id.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }
}

