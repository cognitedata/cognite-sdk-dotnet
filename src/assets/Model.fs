namespace Cognite.Sdk.Assets

open Cognite.Sdk

/// Asset type for responses.
type ResponseAssetDto = {
    /// The Id of the asset.
    Id: int64
    Path: int64 list
    Depth: int
    /// The name of the asset.
    Name: string
    /// The description of the asset.
    Description: string
    /// The parent ID of the asset.
    ParentId: int64 option
    /// Custom, application specific metadata. String key -> String value
    MetaData: Map<string, string>
    /// The source of this asset
    Source: string option
    /// Set ID of the asset in the source. Only applicable if source is specified.
    /// The combination of source and sourceId must be unique.
    SourceId: string option
    /// Time when this asset was created in CDP in milliseconds since Jan 1, 1970.
    CreatedTime: int64
    /// The last time this asset was updated in CDP, in milliseconds since Jan 1, 1970.
    LastUpdatedTime: int64
}

type ParentRef =
    /// ID of parent asset in CDP, if any.
    | ParentId of string
    /// Name of parent. This parent must exist in the same POST request.
    | ParentName of string
    /// Reference ID of parent, to disambiguate if multiple nodes have the same name.
    | ParentRefId of string


/// Asset type for requests.
type RequestAssetDto = {
    /// Name of asset. Often referred to as tag.
    Name: string
    /// Description of asset.
    Description: string
    /// Custom, application specific metadata. String key -> String value
    MetaData: Map<string, string>
    /// The source of this asset
    Source: string option
    /// ID of the asset in the source. Only applicable if source is specified.
    /// The combination of source and sourceId must be unique.
    SourceId: string option
    /// Time when this asset was created in CDP in milliseconds since Jan 1, 1970.
    CreatedTime: int64
    /// The last time this asset was updated in CDP, in milliseconds since Jan 1, 1970.
    LastUpdatedTime: int64
    /// Reference ID used only in post request to disambiguate references to duplicate names.
    RefId: string option
    /// Reference to parent (Id, Name or RefId).
    ParentRef: ParentRef option
}

[<AutoOpen>]
module Model =
    [<Literal>]
    let MaxLimitSize = 10000

    [<Literal>]
    let Url = "/assets"

    type ResponseData = {
        Items: ResponseAssetDto list
        PreviousCursor: string option
        NextCursor : string option
    }

    type AssetResponse = {
        ResponseData: ResponseData
    }

    // Get parameters
    type GetParams =
        | Id of int64
        | Name of string
        | Description of string
        | Path of string
        | MetaData of Map<string, string>
        | Depth of int
        | Fuzziness of int
        | AutoPaging of bool
        | NotLimit of int
        | Cursor of string

        static member Limit limit =
            if limit > MaxLimitSize || limit < 1 then
                failwith "Limit must be set to 1000 or less"
            NotLimit limit

    let Limit = GetParams.Limit

    /// Update parameters
    type UpdateParams =
        /// Set the name of the asset. Often referred to as tag.
        | SetName of string // Name cannot be null
        /// Set or clear the description of asset.
        | SetDescription of string option
        /// Set or clear custom, application specific metadata. String key -> String value
        | SetMetaData of Map<string, string> option
        // Set or clear the source of this asset
        | SetSource of string option
        /// Set or clear ID of the asset in the source. Only applicable if source is specified.
        /// The combination of source and sourceId must be unique.
        | SetSourceId of string option

    type AssetsRequest = {
        Items: RequestAssetDto list
    }