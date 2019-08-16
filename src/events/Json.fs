namespace Fusion.Events

open Thoth.Json.Net

open Fusion


[<AutoOpen>]
module EventExtensions =
    type ReadDto with
        static member Decoder : Decoder<ReadDto> =
            Decode.object (fun get ->
                let metadata = get.Optional.Field "metadata" (Decode.dict Decode.string)
                let assetIds = get.Optional.Field "assetIds" (Decode.list Decode.int64)
                {
                    ExternalId = get.Optional.Field "externalId" Decode.string
                    StartTime = get.Optional.Field "startTime" Decode.int64
                    EndTime = get.Optional.Field "endTime" Decode.int64
                    Type = get.Optional.Field "type" Decode.string
                    SubType = get.Optional.Field "subType" Decode.string
                    Description = get.Optional.Field "description" Decode.string
                    MetaData = if metadata.IsSome then metadata.Value else Map.empty
                    AssetIds = if assetIds.IsSome then assetIds.Value else List.empty
                    Source = get.Optional.Field "source" Decode.string
                    Id = get.Required.Field "id" Decode.int64
                    CreatedTime = get.Required.Field "createdTime" Decode.int64
                    LastUpdatedTime = get.Required.Field "lastUpdatedTime" Decode.int64
                })

    type WriteDto with
        member this.Encoder =
            Encode.object [
                if this.ExternalId.IsSome then 
                    yield "externalId", Encode.string this.ExternalId.Value
                if this.StartTime.IsSome then
                    yield "startTime", Encode.int64 this.StartTime.Value
                if this.EndTime.IsSome then
                    yield "endTime", Encode.int64 this.EndTime.Value
                if this.Type.IsSome then
                    yield "type", Encode.string this.Type.Value
                if this.SubType.IsSome then
                    yield "subType", Encode.string this.SubType.Value
                if this.Description.IsSome then
                    yield "description", Encode.string this.Description.Value
                if not this.MetaData.IsEmpty then
                    let metaString = Encode.propertyBag this.MetaData
                    yield "metadata", metaString
                if not (Seq.isEmpty this.AssetIds) then
                    let assetIdString = Encode.seq (Seq.map Encode.int53 this.AssetIds)
                    yield "assetIds", assetIdString

            ]
