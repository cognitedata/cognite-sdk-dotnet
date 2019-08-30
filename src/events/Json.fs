// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Events

open Oryx
open Thoth.Json.Net

[<AutoOpen>]
module EventExtensions =
    type EventReadDto with
        static member Decoder : Decoder<EventReadDto> =
            Decode.object (fun get ->
                let metadata = get.Optional.Field "metadata" (Decode.dict Decode.string)
                let assetIds = get.Optional.Field "assetIds" (Decode.list Decode.int64)
                {
                    ExternalId = get.Optional.Field "externalId" Decode.string
                    StartTime = get.Optional.Field "startTime" Decode.int64
                    EndTime = get.Optional.Field "endTime" Decode.int64
                    Type = get.Optional.Field "type" Decode.string
                    SubType = get.Optional.Field "subtype" Decode.string
                    Description = get.Optional.Field "description" Decode.string
                    MetaData = if metadata.IsSome then metadata.Value else Map.empty
                    AssetIds = if assetIds.IsSome then assetIds.Value else List.empty
                    Source = get.Optional.Field "source" Decode.string
                    Id = get.Required.Field "id" Decode.int64
                    CreatedTime = get.Required.Field "createdTime" Decode.int64
                    LastUpdatedTime = get.Required.Field "lastUpdatedTime" Decode.int64
                })

    type EventWriteDto with
        member this.Encoder =
            Encode.object [
                yield! Encode.optionalProperty "externalId" Encode.string this.ExternalId
                yield! Encode.optionalProperty "startTime" Encode.int64 this.StartTime
                yield! Encode.optionalProperty "endTime" Encode.int64 this.EndTime
                yield! Encode.optionalProperty "type" Encode.string this.Type
                yield! Encode.optionalProperty "subtype" Encode.string this.SubType
                yield! Encode.optionalProperty "description" Encode.string this.Description
                if not this.MetaData.IsEmpty then
                    let metaString = Encode.propertyBag this.MetaData
                    yield "metadata", metaString
                if not (Seq.isEmpty this.AssetIds) then
                    let assetIdString = Seq.map Encode.int53 this.AssetIds |> Encode.seq
                    yield "assetIds", assetIdString

            ]

    type EventFilter with
        static member Render (this: EventFilter) =
            match this with
            | CaseStartTime startTime -> "name", startTime.Encoder
            | CaseEndTime endTime -> "parentIds", endTime.Encoder
            | CaseMetaData md -> "metaData", Encode.propertyBag md
            | CaseAssetIds ids -> "assetIds", ids |> Encode.int53seq
            | CaseAssetRootIds rootIds -> "rootAssetIds", rootIds |> Encode.int53seq
            | CaseSource source -> "source", Encode.string source
            | CaseType eventType -> "type", Encode.string eventType
            | CaseSubtype eventSubType -> "subType", Encode.string eventSubType
            | CaseCreatedTime createdTime -> "createdTime", createdTime.Encoder
            | CaseLastUpdatedTime updateTime -> "lastUpdatedTime", updateTime.Encoder
            | CaseExternalIdPrefix prefix -> "externalIdPrefix", Encode.string prefix

    type EventItemsReadDto with
        static member Decoder : Decoder<EventItemsReadDto> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list EventReadDto.Decoder |> Decode.map seq)
                NextCursor = get.Optional.Field "nextCursor" Decode.string
            })