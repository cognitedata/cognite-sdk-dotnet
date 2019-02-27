namespace Cognite.Sdk.Assets

open Cognite.Sdk

[<AutoOpen>]
module Model =
    [<Literal>]
    let MaxLimitSize = 10000

    [<Literal>]
    let Url = "/assets"

    type ResponseAsset = {
        Id: int64
        Path: int64 list
        Depth: int
        Name: string
        Description: string
        ParentId: int64 option
        MetaData: Map<string, string>
        Source: string option
        SourceId: string option
        CreatedTime: int64
        LastUpdatedTime: int64
    }

    type ResponseData = {
        Items: ResponseAsset list
        PreviousCursor: string option
        NextCursor : string option
    }

    type AssetResponse = { ResponseData: ResponseData }

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
        | SetName of string // Name cannot be null
        | SetDescription of string option
        | SetMetaData of Map<string, string> option
        | SetSource of string option
        | SetSourceId of string option

    type RequestAsset = {
        Name: string
        Description: string
        MetaData: Map<string, string>
        Source: string option
        SourceId: string option
        CreatedTime: int64
        LastUpdatedTime: int64

        RefId: string option
        ParentRef: ParentRef option
    }

    type AssetsRequest = {
        Items: RequestAsset list
    }