// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Assets

open Oryx
open Thoth.Json.Net

[<AutoOpen>]
module AssetJsonExtensions =
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
                    Source = get.Optional.Field "source" Decode.string
                    MetaData = if metadata.IsSome then metadata.Value else Map.empty
                    CreatedTime = get.Required.Field "createdTime" Decode.int64
                    LastUpdatedTime = get.Required.Field "lastUpdatedTime" Decode.int64
                    RootId = get.Required.Field "rootId" Decode.int64
                })

    type AssetWriteDto with
        member this.Encoder =
            Encode.object [
                yield "name", Encode.string this.Name
                yield! Encode.optionalProperty "externalId" Encode.string this.ExternalId
                yield! Encode.optionalProperty "parentId" Encode.int53 this.ParentId
                yield! Encode.optionalProperty "description" Encode.string this.Description
                yield! Encode.optionalProperty "source" Encode.string this.Source
                yield! Encode.optionalProperty "parentExternalId" Encode.string this.ParentExternalId
                if not this.MetaData.IsEmpty then
                    let metaString = Encode.propertyBag this.MetaData
                    yield "metadata", metaString
            ]

    type AssetFilter with
        static member Render (this: AssetFilter) =
            match this with
            | CaseName name -> "name", Encode.string name
            | CaseParentIds ids -> "parentIds", Encode.int53seq ids
            | CaseRootIds ids -> "rootIds", ids |> Seq.map(fun id -> id.Encoder) |> Encode.seq
            | CaseSource source -> "source", Encode.string source
            | CaseMetaData md -> "metaData", Encode.propertyBag md
            | CaseCreatedTime time -> "createdTime", time.Encoder
            | CaseLastUpdatedTime time -> "lastUpdatedTime", time.Encoder
            | CaseRoot root -> "root", Encode.bool root
            | CaseExternalIdPrefix prefix -> "externalIdPrefix", Encode.string prefix

    type AssetItemsReadDto with
        static member Decoder : Decoder<AssetItemsReadDto> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list AssetReadDto.Decoder |> Decode.map seq)
                NextCursor = get.Optional.Field "nextCursor" Decode.string
            })
