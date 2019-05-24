namespace Cognite.Sdk.Assets

open System.Text

type IDecoder =
    abstract member Decoder: unit -> Decoder


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

type ParentRef =
    /// ID of parent asset in CDF, if any.
    | ParentId of string
    /// Name of parent. This parent must exist in the same POST request.
    | ParentName of string
    /// Reference ID of parent, to disambiguate if multiple nodes have the same name.
    | ParentRefId of string


/// Asset type for create requests.
type AssetCreateDto = {
    /// Name of asset. Often referred to as tag.
    Name: string
    /// Description of asset.
    Description: string
    /// Custom, application specific metadata. String key -> String value
    MetaData: Map<string, string>
    /// The source of this asset (NOTE: will be replaced with external Id)
    Source: string option
    /// ID of the asset in the source. Only applicable if source is specified.
    /// The combination of source and sourceId must be unique (NOTE: will be replaced with external Id)
    SourceId: string option
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

    type AssetResponse = {
        Items: AssetReadDto seq
        NextCursor : string option
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

    /// Asset update parameters
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
        Items: int64 seq
    }