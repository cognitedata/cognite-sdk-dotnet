namespace Cognite.Sdk.Assets

open Thoth.Json.Net
open Cognite.Sdk
open System

[<AutoOpen>]
module AssetsExtensions =
    type ResponseAssetDto with
        static member Decoder : Decode.Decoder<ResponseAssetDto> =
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

    type ResponseData with
        static member Decoder : Decode.Decoder<ResponseData> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list ResponseAssetDto.Decoder)
                PreviousCursor = get.Optional.Field "previousCursor" Decode.string
                NextCursor = get.Optional.Field "nextCursor" Decode.string
            })

    type AssetResponse with
            static member Decoder : Decode.Decoder<AssetResponse> =
                Decode.object (fun get -> {
                    ResponseData = get.Required.Field "data" ResponseData.Decoder
                })

    type CreateAssetDto with
        member this.Encoder =
            Encode.object [
                yield ("name", Encode.string this.Name)
                yield ("description", Encode.string this.Description)
                if not this.MetaData.IsEmpty then
                    let metaString = Encode.dict (Map.map (fun key value -> Encode.string value) this.MetaData)
                    yield ("metadata", metaString)
                if this.Source.IsSome then
                    yield ("source", Encode.string this.Source.Value)
                if this.SourceId.IsSome then
                    yield ("sourceId", Encode.string this.SourceId.Value)
                if this.RefId.IsSome then
                    yield ("refId", Encode.string this.RefId.Value)
                match this.ParentRef with
                | Some (ParentId parentId) ->
                    yield ("parentId", Encode.string parentId)
                | Some (ParentName parentName) ->
                    yield ("parentId", Encode.string parentName)
                | Some (ParentRefId parentRefId) ->
                    yield ("parentId", Encode.string parentRefId)
                | None -> ()
            ]
    type AssetsRequest with
         member this.Encoder =
            Encode.object [
                yield ("items", List.map (fun (it: CreateAssetDto) -> it.Encoder) this.Items |> Encode.list)
            ]

    let renderParams (arg: GetParams) =
        match arg with
        | Id id -> ("id", id.ToString())
        | Name name -> ("name", name)
        | Description desc -> ("desc", desc)
        | Path path -> ("path", path)
        | MetaData meta ->
            let metaString = Encode.dict (Map.map (fun key value -> Encode.string value) meta) |> Encode.toString 0
            ("metadata", metaString)
        | Depth depth -> ("depth", depth.ToString())
        | Fuzziness fuzz -> ("fuzziness", fuzz.ToString ())
        | AutoPaging value -> ("autopaging", value.ToString().ToLower())
        | NotLimit limit -> ("limit", limit.ToString ())
        | Cursor cursor -> ("cursor", cursor)

    let renderUpdateFields (arg: UpdateParams) =
        match arg with
        | SetName name ->
            ("name", Encode.object [
                ("set", Encode.string name)
            ])
        | SetDescription optDesc ->
            ("description", Encode.object [
                match optDesc with
                | Some desc -> yield ("set", Encode.string desc)
                | None -> yield ("setNull", Encode.bool true)
            ])
        | SetMetaData optMeta ->
            ("metadata", Encode.object [
                match optMeta with
                | Some meta -> yield ("set", Encode.dict (Map.map (fun key value -> Encode.string value) meta))
                | None -> yield ("setNull", Encode.bool true)
            ])
        | SetSource optSource ->
            ("source", Encode.object [
                match optSource with
                | Some source -> yield ("set", Encode.string source)
                | None -> yield ("setNull", Encode.bool true)
            ])
        | SetSourceId optSourceId ->
            ("sourceId", Encode.object [
                match optSourceId with
                | Some sourceId -> yield ("set", Encode.string sourceId)
                | None -> yield ("setNull", Encode.bool true)
            ])

    /// JSON decode response and map decode error string to exception so we don't get more response error types.
    let decodeResponse decoder result =
        result
        |> Result.bind (fun res ->
            let ret = Decode.fromString decoder res
            match ret with
            | Error error ->
                DecodeError error |> Error
            | Ok value -> Ok value
        )