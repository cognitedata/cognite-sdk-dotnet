// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System.Collections.Generic;
using CogniteSdk.Types.Common;

namespace CogniteSdk
{
    /// <summary>
    /// Query for listing available 3D outputs.
    /// </summary>
    public class ThreeDOutputListQuery : IQueryParams
    {
        /// <summary>
        /// Format identifier, e.g. 'ept-pointcloud' (point cloud).
        /// Well known formats are: 'ept-pointcloud' (point cloud data)
        /// or 'reveal-directory' (output supported by Reveal). 'all-outputs'
        /// can be used to retrieve all outputs for a 3D revision.
        /// Note that some of the outputs are internal,
        /// where the format and availability might change without warning.
        /// </summary>
        public string Format { get; set; }

        /// <inheritdoc/>
        public List<(string, string)> ToQueryParams()
        {
            var list = new List<(string, string)>();
            if (Format != null)
                list.Add(("format", Format));

            return list;
        }

        /// <inheritdoc />
        public override string ToString() => Stringable.ToString(this);
    }

    /// <summary>
    /// Named a versioned model output.
    /// </summary>
    public class ThreeDRevisionOutput
    {
        /// <summary>
        /// Format identifier
        /// </summary>
        public string Format { get; set; }
        /// <summary>
        /// Version of the output format, starting at 1.
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        /// Reference to 3D file containing output.
        /// 3D file can either be a single file or folder.
        /// </summary>
        public long BlobId { get; set; }
    }
}
