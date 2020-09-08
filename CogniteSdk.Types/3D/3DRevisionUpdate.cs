// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// The ThreeDRevision update class.
    /// </summary>
    public class ThreeDRevisionUpdate
    {
        /// <summary>
        /// Set a new value for the string.
        /// </summary>
        public Update<bool> Published { get; set; }
        /// <summary>
        /// Set new value for rotation.
        /// </summary>
        public Update<IEnumerable<double>> Rotation { get; set; }
        /// <summary>
        /// Set new camera target and position.
        /// </summary>
        public Update<RevisionCameraProperties> Camera { get; set; }
        /// <summary>
        /// Custom, application specific metadata. String key -> String value. Limits: Maximum length of key is 32
        /// bytes, value 512 bytes, up to 16 key-value pairs.
        /// </summary>
        public UpdateDictionary<string> Metadata { get; set; }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }

    /// <summary>
    /// The ThreeDRevision update item type. Contains the update item for an <see cref="ThreeDRevisionUpdate">ThreeDRevisionUpdate</see>.
    /// </summary>
    public class ThreeDRevisionUpdateItem : UpdateItem<ThreeDRevisionUpdate>
    {
        /// <summary>
        /// Initialize the ThreeDRevision update item with an internal Id.
        /// </summary>
        /// <param name="id">Internal Id to set.</param>
        public ThreeDRevisionUpdateItem(long id) : base(id)
        {
        }
    }
}