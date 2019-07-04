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
                    Source = get.Optional.Field "source" Decode.string
                    MetaData = if metadata.IsSome then metadata.Value else Map.empty
                    CreatedTime = get.Required.Field "createdTime" Decode.int64
                    LastUpdatedTime = get.Required.Field "lastUpdatedTime" Decode.int64
                })

    type AssetCreateDto with
        member this.Encoder =
            Encode.object [
                yield! Encode.optionalProperty "externalId" Encode.string this.ExternalId
                yield "name", Encode.string this.Name
                if this.ParentId.IsSome then
                    yield "parentId", Encode.int53 this.ParentId.Value
                if this.Description.IsSome then
                    yield "description", Encode.string this.Description.Value
                if not this.MetaData.IsEmpty then
                    let metaString = Encode.propertyBag this.MetaData
                    yield "metadata", metaString
                if this.Source.IsSome then
                    yield "source", Encode.string this.Source.Value
                if this.ParentExternalId.IsSome then
                    yield "parentExternalId", Encode.string this.ParentExternalId.Value
            ]

