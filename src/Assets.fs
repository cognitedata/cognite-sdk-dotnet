namespace Cognite.Sdk

open System
open Thoth.Json.Net
open Request

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

    type Data = {
        Items: Asset list } with

        static member Decoder : Decode.Decoder<Data> =
            Decode.object (fun get ->
                {
                    Items = get.Required.Field "items" (Decode.list Asset.Decoder)
                })

    type Response = {
        Data: Data } with

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
        Asset : Asset

        RefId: string option
        ParentName: string option
        ParentRefId: string option } with

        member this.Encoder =
            Encode.object [ 
                yield ("id", Encode.int64 this.Asset.Id)
                yield ("name", Encode.string this.Asset.Name)
                yield ("description", Encode.string this.Asset.Description)
                if this.Asset.Source.IsSome then
                    yield ("source", Encode.string this.Asset.Source.Value)
            ]

    type Request = {
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
    let Url = "/assets?"

    let getAssets (context: Context) (args: Args list) = async {
        let query = args |> List.map renderArgs
        let! text = fetch Get context Url query

        return Decode.fromString Response.Decoder text
    }

    let create (context: Context) (assets: Request) = async {
        let body = Encode.toString 0 assets.Encoder

        let! text = fetch Post context Url []

        return Decode.fromString Response.Decoder text
    }