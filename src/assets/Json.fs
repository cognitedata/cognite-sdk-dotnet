namespace Cognite.Sdk.Assets

open Thoth.Json.Net

open Cognite.Sdk
open Cognite.Sdk.Common


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
                if this.ExternalId.IsSome then
                    yield "externalId", Encode.string this.ExternalId.Value
                yield "name", Encode.string this.Name
                if this.ParentId.IsSome then
                    yield "parentId", Encode.int64' this.ParentId.Value
                if this.Description.IsSome then
                    yield "description", Encode.string this.Description.Value
                if not this.MetaData.IsEmpty then
                    let metaString = Encode.dict (Map.map (fun key value -> Encode.string value) this.MetaData)
                    yield "metadata", metaString
                if this.Source.IsSome then
                    yield "source", Encode.string this.Source.Value
                if this.ParentExternalId.IsSome then
                    yield "parentExternalId", Encode.string this.ParentExternalId.Value
            ]
    type AssetsCreateRequest with
         member this.Encoder =
            Encode.object [
                yield "items", Seq.map (fun (it: AssetCreateDto) -> it.Encoder) this.Items |> Encode.seq
            ]

    let renderParams (arg: GetParams) =
        match arg with
        | NotLimit limit -> "limit", limit.ToString ()
        | Cursor cursor -> "cursor", cursor
        | Name name -> "name", name
        | ParentIds ids ->
            let ids' = List.ofSeq ids
            "parentIds", Encode.list (List.map Encode.int64' ids') |> Encode.toString 0
        | Source source -> "source", source
        | Root root -> "root", root.ToString().ToLower()
        | MinCreatedTime value -> "minCreatedTime", value.ToString ()
        | MaxCreatedTime value -> "maxCreatedTime", value.ToString ()
        | MinLastUpdatedTime value -> "minLastUpdatedTime", value.ToString ()
        | MaxLastUpdatedTime value -> "maxLastUpdatedTime", value.ToString ()
        | ExternalIdPrefix externalId -> "externalIdPrefix", externalId

        (*
        | MetaData meta ->
            let metaString = Encode.dict (Map.map (fun key value -> Encode.string value) meta) |> Encode.toString 0
            "metadata", metaString

        | Depth depth -> "depth", depth.ToString ()
        | Fuzziness fuzz -> "fuzziness", fuzz.ToString ()
        | AutoPaging value -> "autopaging", value.ToString().ToLower()
        *)
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
        | SetExternalId optExternalId ->
            "externalId", Encode.object [
                match optExternalId with
                | Some externalId -> yield "set", Encode.string externalId
                | None -> yield "setNull", Encode.bool true
            ]
        | SetParentExternalId optParentExternalId ->
            "externalId", Encode.object [
                match optParentExternalId with
                | Some parentExternalId -> yield "set", Encode.string parentExternalId
                | None -> yield "setNull", Encode.bool true
            ]
        | SetParentId optParentId ->
            "parentId", Encode.object [
                match optParentId with
                | Some parentId -> yield "set", Encode.int64' parentId
                | None -> yield "setNull", Encode.bool true
            ]

    type AssetUpdateRequest with
        member this.Encoder =
            Encode.object [
                yield ("id", Encode.int64' this.Id)
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
                "items", Seq.map (fun id ->
                    match id with
                    | Id id -> Encode.int64' id
                    | ExternalId id -> Encode.string id
                ) this.Items |> Encode.seq
            ]
