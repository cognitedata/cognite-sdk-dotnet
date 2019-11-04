// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Files

open Oryx
open Thoth.Json.Net

[<AutoOpen>]
module FileExtensions =
    type FileReadDto with
        static member Decoder : Decoder<FileReadDto> =
            Decode.object (fun get ->
                let metadata = get.Optional.Field "metadata" (Decode.dict Decode.string)
                let assetIds = get.Optional.Field "assetIds" (Decode.list Decode.int64)
                {
                    Id = get.Required.Field "id" Decode.int64
                    ExternalId = get.Optional.Field "externalId" Decode.string
                    Name = get.Required.Field "name" Decode.string
                    Source = get.Optional.Field "source" Decode.string
                    MimeType = get.Optional.Field "mimeType" Decode.string
                    MetaData = Option.defaultValue Map.empty metadata
                    AssetIds = Option.defaultValue List.empty assetIds
                    SourceCreatedTime = get.Optional.Field "sourceCreatedTime" Decode.int64
                    SourceModifiedTime = get.Optional.Field "sourceModifiedTime" Decode.int64
                    CreatedTime = get.Required.Field "createdTime" Decode.int64
                    LastUpdatedTime = get.Required.Field "lastUpdatedTime" Decode.int64
                    UploadedTime = get.Optional.Field "uploadedTime" Decode.int64
                    Uploaded = get.Required.Field "uploaded" Decode.bool
                }
            )

    type FileCreatedDto with
        static member Decoder : Decoder<FileCreatedDto> =
            Decode.object (fun get ->
                let metadata = get.Optional.Field "metadata" (Decode.dict Decode.string)
                let assetIds = get.Optional.Field "assetIds" (Decode.list Decode.int64)
                {
                    Id = get.Required.Field "id" Decode.int64
                    ExternalId = get.Optional.Field "externalId" Decode.string
                    Name = get.Required.Field "name" Decode.string
                    Source = get.Optional.Field "source" Decode.string
                    MimeType = get.Optional.Field "mimeType" Decode.string
                    MetaData = Option.defaultValue Map.empty metadata
                    AssetIds = Option.defaultValue List.empty assetIds
                    SourceCreatedTime = get.Optional.Field "sourceCreatedTime" Decode.int64
                    SourceModifiedTime = get.Optional.Field "sourceModifiedTime" Decode.int64
                    CreatedTime = get.Required.Field "createdTime" Decode.int64
                    LastUpdatedTime = get.Required.Field "lastUpdatedTime" Decode.int64
                    UploadedTime = get.Optional.Field "uploadedTime" Decode.int64
                    Uploaded = get.Required.Field "uploaded" Decode.bool
                    UploadUrl = get.Required.Field "uploadUrl" Decode.string
                }
            )

    type FileWriteDto with
        member this.Encoder =
            Encode.object [
                yield "name", Encode.string this.Name
                yield! Encode.optionalProperty "externalId" Encode.string this.ExternalId
                yield! Encode.optionalProperty "source" Encode.string this.Source
                yield! Encode.optionalProperty "mimeType" Encode.string this.MimeType
                yield! Encode.optionalProperty "sourceCreatedTime" Encode.int53 this.SourceCreatedTime
                yield! Encode.optionalProperty "sourceModifiedTime" Encode.int53 this.SourceModifiedTime
                if not this.MetaData.IsEmpty then
                    yield "metadata", Encode.propertyBag this.MetaData
            ]

    type FileFilter with
        static member Render (this: FileFilter) =
            match this with
            | CaseName name -> "name", Encode.string name
            | CaseMimeType mimeType -> "mimeType", Encode.string mimeType
            | CaseMetaData metadata -> "metadata", Encode.propertyBag metadata
            | CaseAssetIds assetIds -> "assetIds", Encode.int53seq assetIds
            | CaseSource source -> "source", Encode.string source
            | CaseCreatedTime createdTime -> "createdTime", createdTime.Encoder
            | CaseLastUpdatedTime lastUpdatedTime -> "lastUpdatedTime", lastUpdatedTime.Encoder
            | CaseUploadedTime uploadedTime -> "uploadedTime", uploadedTime.Encoder
            | CaseSourceCreatedTime sourceCreatedTime -> "sourceCreatedTime", sourceCreatedTime.Encoder
            | CaseSourceModifiedTime sourceModifiedTime -> "sourceModifiedTime", sourceModifiedTime.Encoder
            | CaseExternalIdPrefix externalIdPrefix -> "externalIdPrefix", Encode.string externalIdPrefix
            | CaseUploaded uploaded -> "uploaded", Encode.bool uploaded

    type FileItemsReadDto with
        static member Decoder : Decoder<FileItemsReadDto> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list FileReadDto.Decoder |> Decode.map seq)
                NextCursor = get.Optional.Field "nextCursor" Decode.string
            })