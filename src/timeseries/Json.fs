namespace CogniteSdk.TimeSeries

open Thoth.Json.Net
open Oryx
open CogniteSdk


[<AutoOpen>]
module TimeseriesExtensions =
    type NumericDataPointDto with
        static member Decoder : Decoder<NumericDataPointDto> =
            Decode.object (fun get ->
                {
                    TimeStamp = get.Required.Field "timestamp" Decode.int64
                    Value = get.Required.Field "value" Decode.float
                })

    type StringDataPointDto with
        static member Decoder : Decoder<StringDataPointDto> =
            Decode.object (fun get ->
                {
                    TimeStamp = get.Required.Field "timestamp" Decode.int64
                    Value = get.Required.Field "value" Decode.string
                })

    type TimeSeriesWriteDto with
        member this.Encoder =
            Encode.object [
                yield! Encode.optionalProperty "externalId" Encode.string this.ExternalId
                yield! Encode.optionalProperty "name" Encode.string this.Name
                yield! Encode.optionalProperty "legacyName" Encode.string this.LegacyName
                yield! Encode.optionalProperty "description" Encode.string this.Description
                yield! Encode.optionalProperty "unit" Encode.string this.Unit
                yield! Encode.optionalProperty "assetId" Encode.int53 this.AssetId
                if this.IsString then
                    yield ("isString", Encode.bool this.IsString)
                if not this.MetaData.IsEmpty then
                    let metaString = Encode.dict (Map.map (fun key value -> Encode.string value) this.MetaData)
                    yield "metadata", metaString
                if this.IsStep then
                    yield "isStep", Encode.bool this.IsStep
                if not (Seq.isEmpty this.SecurityCategories) then
                    yield "securityCategories", Encode.seq (Seq.map Encode.int53 this.SecurityCategories)
            ]

    type TimeSeriesReadDto with
        static member Decoder : Decoder<TimeSeriesReadDto> =
            Decode.object (fun get ->
                let metadata = get.Optional.Field "metadata" (Decode.dict Decode.string)
                {
                    Id = get.Required.Field "id" Decode.int64
                    ExternalId = get.Optional.Field "externalId" Decode.string
                    Name = get.Optional.Field "name" Decode.string
                    IsString = get.Required.Field "isString" Decode.bool
                    MetaData = if metadata.IsSome then metadata.Value else Map.empty
                    Unit = get.Optional.Field "unit" Decode.string
                    AssetId = get.Optional.Field "assetId" Decode.int64
                    IsStep = get.Required.Field "isStep" Decode.bool
                    Description = get.Optional.Field "description" Decode.string
                    SecurityCategories = get.Optional.Field "securityCategories" ((Decode.array Decode.int64) |> Decode.map seq)
                    CreatedTime = get.Required.Field "createdTime" Decode.int64
                    LastUpdatedTime = get.Required.Field "lastUpdatedTime" Decode.int64
                })

