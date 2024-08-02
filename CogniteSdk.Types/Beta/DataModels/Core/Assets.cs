// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.Generic;

namespace CogniteSdk.Beta.DataModels.Core
{
    /// <summary>
    /// The AssetClass core concept is used to define the class of an asset,
    /// serving as a base type for describing common properties for assets.
    /// </summary>
    public class CogniteAssetClass : CogniteDescribable
    {
        /// <summary>
        /// A unique identifier for the class of asset.
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Textual string for which standard the code is from.
        /// </summary>
        public string Standard { get; set; }
    }

    /// <summary>
    /// The AssetType core concept is used to define the type of an asset.
    /// </summary>
    public class CogniteAssetType : CogniteDescribable
    {
        /// <summary>
        /// A unique identifier for the type of asset.
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Class of this type, direct relation to AssetClass.
        /// </summary>
        public DirectRelationIdentifier AssetClass { get; set; }
    }

    /// <summary>
    /// Representation of an asset in the core data model.
    /// </summary>
    public class CogniteAsset : CogniteAssetBase
    {
        /// <summary>
        /// Parent of this asset.
        /// </summary>
        public DirectRelationIdentifier Parent { get; set; }
        /// <summary>
        /// List of assets that reference this asset as parent.
        /// </summary>
        public IEnumerable<DirectRelationIdentifier> Children { get; set; }
        /// <summary>
        /// Asset at the top of the hierarchy.
        /// </summary>
        public DirectRelationIdentifier Root { get; set; }
        /// <summary>
        /// Materialized path of this asset.
        /// </summary>
        public IEnumerable<DirectRelationIdentifier> Path { get; set; }
        /// <summary>
        /// Equipment associated with this asset.
        /// </summary>
        public DirectRelationIdentifier Equipment { get; set; }
        /// <summary>
        /// Time series associated with this asset.
        /// </summary>
        public DirectRelationIdentifier TimeSeries { get; set; }
        /// <summary>
        /// Class of this asset.
        /// </summary>
        public DirectRelationIdentifier AssetClass { get; set; }
        /// <summary>
        /// Type of this asset.
        /// </summary>
        public DirectRelationIdentifier Type { get; set; }
        /// <summary>
        /// Activities associated with this asset.
        /// </summary>
        public DirectRelationIdentifier Activities { get; set; }
        /// <summary>
        /// Files associated with this asset.
        /// </summary>
        public DirectRelationIdentifier Files { get; set; }
    }

    /// <summary>
    /// Base class for CogniteAsset.
    /// </summary>
    public abstract class CogniteAssetBase : CogniteCoreInstanceBase, ICogniteVisualizable
    {
        /// <inheritdoc />
        public DirectRelationIdentifier Object3D { get; set; }
    }
}