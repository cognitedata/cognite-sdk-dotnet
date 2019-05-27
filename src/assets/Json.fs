namespace Cognite.Sdk.Assets

open Thoth.Json.Net


[<AutoOpen>]
module AssetsExtensions =
    type AssetReadDto with
        static member Decoder : Decoder<AssetReadDto> =
            Decode.object (fun get ->
                let metadata = get.Optional.Field "metadata" (Decode.dict Decode.string)
                {
                    ExternalId = get.Optional.Field "externalId" Decode.string
                    Id = get.Required.Field "id" Decode.int64
                    Name = get.Required.Field "name" Decode.string
                    Description = get.Optional.Field "description" Decode.string
                    ParentId = get.Optional.Field "parentId" Decode.int64
                    Path = get.Required.Field "path" (Decode.list Decode.int64)
                    Source = get.Optional.Field "source" Decode.string
                    Depth = get.Required.Field "depth" Decode.int
                    MetaData = if metadata.IsSome then metadata.Value else Map.empty
                    CreatedTime = get.Required.Field "createdTime" Decode.int64
                    LastUpdatedTime = get.Required.Field "lastUpdatedTime" Decode.int64
                })

    type AssetResponse with
        static member Decoder : Decoder<AssetResponse> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list AssetReadDto.Decoder |> Decode.map seq)
                NextCursor = get.Optional.Field "nextCursor" Decode.string
            })


    type AssetCreateDto with
        member this.Encoder =
            Encode.object [
                yield "name", Encode.string this.Name
                yield "description", Encode.string this.Description
                if not this.MetaData.IsEmpty then
                    let metaString = Encode.dict (Map.map (fun key value -> Encode.string value) this.MetaData)
                    yield "metadata", metaString
                if this.Source.IsSome then
                    yield "source", Encode.string this.Source.Value
                if this.SourceId.IsSome then
                    yield "sourceId", Encode.string this.SourceId.Value
                if this.RefId.IsSome then
                    yield "refId", Encode.string this.RefId.Value
                match this.ParentRef with
                | Some (ParentId parentId) ->
                    yield "parentId", Encode.string parentId
                | Some (ParentName parentName) ->
                    yield "parentId", Encode.string parentName
                | Some (ParentRefId parentRefId) ->
                    yield "parentId", Encode.string parentRefId
                | None -> ()
            ]
    type AssetsCreateRequest with
         member this.Encoder =
            Encode.object [
                yield "items", Seq.map (fun (it: AssetCreateDto) -> it.Encoder) this.Items |> Encode.seq
            ]

    let renderParams (arg: GetParams) =
        match arg with
        | Id id -> "id", id.ToString ()
        | Name name -> "name", name
        | Description desc -> "desc", desc
        | Path path -> "path", path
        | MetaData meta ->
            let metaString = Encode.dict (Map.map (fun key value -> Encode.string value) meta) |> Encode.toString 0
            "metadata", metaString
        | Depth depth -> "depth", depth.ToString ()
        | Fuzziness fuzz -> "fuzziness", fuzz.ToString ()
        | AutoPaging value -> "autopaging", value.ToString().ToLower()
        | NotLimit limit -> "limit", limit.ToString ()
        | Cursor cursor -> "cursor", cursor

    let renderUpdateFields (arg: UpdateParams) =
        match arg with
        | SetName name ->
            "name", Encode.object [
                "set", Encode.string name
            ]
        | SetDescription optDesc ->
            "description", Encode.object [
                match optDesc with
                | Some desc -> yield "set", Encode.string desc
                | None -> yield "setNull", Encode.bool true
            ]
        | SetMetaData optMeta ->
            "metadata", Encode.object [
                match optMeta with
                | Some meta -> yield "set", Encode.dict (Map.map (fun key value -> Encode.string value) meta)
                | None -> yield "setNull", Encode.bool true
            ]
        | SetSource optSource ->
            "source", Encode.object [
                match optSource with
                | Some source -> yield "set", Encode.string source
                | None -> yield "setNull", Encode.bool true
            ]
        | SetSourceId optSourceId ->
            "sourceId", Encode.object [
                match optSourceId with
                | Some sourceId -> yield "set", Encode.string sourceId
                | None -> yield "setNull", Encode.bool true
            ]

    type AssetUpdateRequest with
        member this.Encoder =
            Encode.object [
                yield ("id", Encode.int64 this.Id)
                for arg in this.Params do
                    yield renderUpdateFields arg
            ]

    type AssetsUpdateRequest with
        member this.Encoder =
            Encode.object [
                "items", Seq.map (fun (item:AssetUpdateRequest) -> item.Encoder) this.Items |> Encode.seq
            ]

    type AssetsDeleteRequest with
        member this.Encoder =
            Encode.object [
                "items", Seq.map Encode.int64 this.Items |> Encode.seq
            ]
