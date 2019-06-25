namespace Cognite.Sdk.Assets

open System.Text
open Cognite.Sdk.Common

/// Asset type for responses.
type AssetReadDto = {
    /// External Id provided by client. Should be unique within the project.
    ExternalId: string option
    /// The name of the asset.
    Name: string
    /// The parent ID of the asset.
    ParentId: int64 option
    /// The description of the asset.
    Description: string option
    /// Custom, application specific metadata. String key -> String value
    MetaData: Map<string, string>
    /// The source of this asset
    Source: string option
    /// The Id of the asset.
    Id: int64
    ///IDs of assets on the path to the asset.
    Path: int64 seq
    /// Asset path depth (number of levels below root node).
    Depth: int
    /// Time when this asset was created in CDF in milliseconds since Jan 1, 1970.
    CreatedTime: int64
    /// The last time this asset was updated in CDF, in milliseconds since Jan 1, 1970.
    LastUpdatedTime: int64
}

/// Asset type for create requests.
type AssetCreateDto = {
    ExternalId: string option
    /// Name of asset. Often referred to as tag.
    Name: string
    /// Javascript friendly internal ID given to the object.
    ParentId: int64 option
    /// Description of asset.
    Description: string option
    /// Custom, application specific metadata. String key -> String value
    MetaData: Map<string, string>
    /// The source of this asset (NOTE: will be replaced with external Id)
    Source: string option
    /// External Id provided by client. Should be unique within the project.
    ParentExternalId: string option
}

[<AutoOpen>]
module Model =
    [<Literal>]
    let MaxLimitSize = 1000

    [<Literal>]
    let Url = "/assets"

    type AssetResponse = {
        Items: AssetReadDto seq
        NextCursor : string option
    }

    /// Asset update parameters
    type UpdateParams =
        /// Set the name of the asset. Often referred to as tag.
        | SetName of string // Name cannot be null
        /// Set or clear the description of asset.
        | SetDescription of string option
        /// Set or clear custom, application specific metadata. String key ->
        /// String value
        | SetMetaData of Map<string, string> option
        // Set or clear the source of this asset
        | SetSource of string option
        /// Set or clear ID of the asset in the source. Only applicable if
        /// source is specified. The combination of source and sourceId must be
        /// unique.
        | SetExternalId of string option
        | SetParentId of int64 option
        | SetParentExternalId of string option

    type AssetsCreateRequest = {
        Items: AssetCreateDto seq
    }

    type AssetUpdateRequest = {
        Id: int64
        Params: UpdateParams seq
    }

    type AssetsUpdateRequest = {
        Items: AssetUpdateRequest seq
    }

    type AssetsDeleteRequest = {
        Items: Identity seq
    }