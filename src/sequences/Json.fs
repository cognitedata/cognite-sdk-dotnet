// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Sequences

open Oryx
open Thoth.Json.Net

[<AutoOpen>]
module SequencesJsonExtensions =
    type ColumnReadDto with
        static member Decoder : Decoder<ColumnReadDto> =
            Decode.object (fun get ->
                let metadata = get.Optional.Field "metadata" (Decode.dict Decode.string)
                {
                    Name = get.Optional.Field "name" Decode.string
                    ExternalId = get.Optional.Field "externalId" Decode.string
                    Description = get.Optional.Field "description" Decode.string
                    ValueType = get.Required.Field "valueType" ValueType.Decoder
                    MetaData = metadata |> Option.defaultValue Map.empty
                    CreatedTime = get.Required.Field "createdTime" Decode.int64
                    LastUpdatedTime = get.Required.Field "lastUpdatedTime" Decode.int64
                }
            )

    type SequenceReadDto with
        static member Decoder : Decoder<SequenceReadDto> =
            Decode.object (fun get ->
                let metadata = get.Optional.Field "metadata" (Decode.dict Decode.string)
                let columns = Decode.list ColumnReadDto.Decoder |> Decode.map Seq.ofList
                {
                    Id = get.Required.Field "id" Decode.int64
                    Name = get.Optional.Field "name" Decode.string
                    Description = get.Optional.Field "description" Decode.string
                    AssetId = get.Optional.Field "assetId" Decode.int64
                    ExternalId = get.Optional.Field "externalId" Decode.string
                    MetaData = metadata |> Option.defaultValue Map.empty
                    Columns = get.Required.Field "columns" columns
                    CreatedTime = get.Required.Field "createdTime" Decode.int64
                    LastUpdatedTime = get.Required.Field "lastUpdatedTime" Decode.int64
                })

    type SequenceItemsReadDto with
        static member Decoder : Decoder<SequenceItemsReadDto> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list SequenceReadDto.Decoder |> Decode.map Seq.ofList)
                NextCursor = get.Optional.Field "nextCursor" Decode.string
            })

    type ColumnCreateDto with
        member this.Encoder =
            Encode.object [
                yield! Encode.optionalProperty "name" Encode.string this.Name
                yield "externalId", Encode.string this.ExternalId
                yield! Encode.optionalProperty "description" Encode.string this.Description
                yield "valueType", Encode.string (this.ValueType.ToString())
                if not this.MetaData.IsEmpty then
                    let metaString = Encode.propertyBag this.MetaData
                    yield "metadata", metaString
            ]

    type SequenceCreateDto with
        member this.Encoder =
            Encode.object [
                yield! Encode.optionalProperty "name" Encode.string this.Name
                yield! Encode.optionalProperty "description" Encode.string this.Description
                yield! Encode.optionalProperty "assetId" Encode.int53 this.AssetId
                yield! Encode.optionalProperty "externalId" Encode.string this.ExternalId
                if not this.MetaData.IsEmpty then
                    let metaString = Encode.propertyBag this.MetaData
                    yield "metadata", metaString
                yield "columns", Encode.seq (Seq.map (fun (c: ColumnCreateDto) -> c.Encoder) this.Columns)
            ]

    type SequenceFilter with
        static member Render (this: SequenceFilter) =
            match this with
            | CaseName name -> "name", Encode.string name
            | CaseExternalIdPrefix prefix -> "externalIdPrefix", Encode.string prefix
            | CaseMetaData md -> "metadata", Encode.propertyBag md
            | CaseAssetIds ids -> "assetIds", ids |> Seq.map Encode.int64 |> Encode.seq
            | CaseRootAssetIds ids -> "rootAssetIds", ids |> Seq.map Encode.int64 |> Encode.seq
            | CaseCreatedTime time -> "createdTime", time.Encoder
            | CaseLastUpdatedTime time -> "lastUpdatedTime", time.Encoder
