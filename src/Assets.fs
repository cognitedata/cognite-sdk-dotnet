namespace Cognite.Sdk

open System
open Thoth.Json.Net

module Assets =
    type Asset = {
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
        LastUpdatedTime: int64 } with

        static member Decoder : Decode.Decoder<Asset> =
            Decode.object (fun get ->
                {
                    Id = get.Required.Field "id" Decode.int64
                    Name = get.Required.Field "name" Decode.string
                    Description = get.Required.Field "description" Decode.string
                    ParentId = get.Optional.Field "parentId" Decode.int64
                    Path = get.Required.Field "path" (Decode.list Decode.int64)
                    Source = get.Optional.Field "source" Decode.string
                    SourceId = get.Optional.Field "sourceId" Decode.string
                    Depth = get.Required.Field "depth" Decode.int
                    MetaData = get.Required.Field "metadata" (Decode.dict Decode.string)
                    CreatedTime = get.Required.Field "createdTime" Decode.int64
                    LastUpdatedTime = get.Required.Field "lastUpdatedTime" Decode.int64
                })

    type Data = { Items: Asset list } with

        static member Decoder : Decode.Decoder<Data> =
            Decode.object (fun get ->
                {
                    Items = get.Required.Field "items" (Decode.list Asset.Decoder)
                })

    type Response = { Data: Data } with

        static member Decoder : Decode.Decoder<Response> =
            Decode.object (fun get ->
                {
                    Data = get.Required.Field "data" Data.Decoder
                })

    type Args =
        | Name of string
        | Description of string
        | Path of string
        | MetaData of Map<string, string> option
        | Depth of int
        | Fuzziness of int
        | AutoPaging of bool
        | Limit of int
        | Cursor of string

    type AssetRequest = {
        Name: string
        Description: string
        MetaData: Map<string, string>
        Source: string option
        SourceId: string option
        CreatedTime: int64
        LastUpdatedTime: int64

        RefId: string option
        ParentRef: ParentRef option

        } with

        member this.Encoder =
            Encode.object [
                yield ("name", Encode.string this.Name)
                yield ("description", Encode.string this.Description)
                yield ("createdTime", Encode.int64 this.CreatedTime)

                if this.Source.IsSome then
                    yield ("source", Encode.string this.Source.Value)
                if this.SourceId.IsSome then
                    yield ("sourceId", Encode.string this.SourceId.Value)
            ]

    type AssetsRequest = {
        Items: AssetRequest list } with

        member this.Encoder =
            Encode.object [
                ("items", List.map (fun (it: AssetRequest) -> it.Encoder) this.Items |> Encode.list)
            ]

    let renderArgs arg =
        match arg with
        | Name name -> ("name", name)
        | Description desc -> ("desc", desc)
        | Path path -> ("path", path)
        | MetaData md -> ("metadata", "fixme")
        | Depth depth -> ("depth", depth.ToString ())
        | Fuzziness fuzz -> ("fuzziness", fuzz.ToString ())
        | AutoPaging value -> ("autopaging", value.ToString().ToLower())
        | Limit limit -> ("limit", limit.ToString ())
        | Cursor cursor -> ("cursor", cursor)

    [<Literal>]
    let Url = "/assets"

    let getAssets (ctx: Context) (args: Args list) : Async<Result<Response, string>> = async {
        let query = args |> List.map renderArgs
        let url = Resource Url
        let! text = ctx.Fetch Get ctx url query

        return Decode.fromString Response.Decoder text
    }

    let create (ctx: Context) (assets: AssetsRequest) = async {
        let body = Encode.toString 0 assets.Encoder
        let url = Resource Url
        let! text = ctx.Fetch Post ctx url []

        return Decode.fromString Response.Decoder text
    }

    let getAsset (ctx: Context) (assetId: int64) : Async<Result<Response, string>> = async {
        let url = Url + sprintf "/%d" assetId |> Resource
        let! text = ctx.Fetch Get ctx url []

        return Decode.fromString Response.Decoder text
    }