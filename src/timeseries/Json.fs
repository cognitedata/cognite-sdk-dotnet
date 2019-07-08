namespace Cognite.Sdk.Timeseries

open System
open Thoth.Json.Net
open Cognite.Sdk
open Cognite.Sdk.Common


[<AutoOpen>]
module TimeseriesExtensions =
    type DataPointDto with
        member this.Encoder =
            Encode.object [
                yield "timestamp", Encode.int53 this.TimeStamp
                match this.Value with
                | CaseString value -> yield "value", Encode.string value
                | CaseInteger value -> yield "value", Encode.int53 value
                | CaseFloat value -> yield "value", Encode.float value
            ]

        static member Decoder : Decoder<DataPointDto> =
            Decode.object (fun get ->
                {
                    TimeStamp = get.Required.Field "timestamp" Decode.int64
                    Value = get.Required.Field "value" (Decode.oneOf [
                            // We do not decode integers as we don't want to end up with a mix of integers and floats.
                            Decode.float |> Decode.map CaseFloat
                            Decode.string |> Decode.map CaseString
                        ])
                })

    type AggregateDataPointReadDto with
        static member Decoder : Decoder<AggregateDataPointReadDto> =
            Decode.object (fun get ->
                {
                    TimeStamp = get.Required.Field "timestamp" Decode.int64
                    Average = get.Optional.Field "average" Decode.float
                    Max = get.Optional.Field "max" Decode.float
                    Min = get.Optional.Field "min" Decode.float
                    Count = get.Optional.Field "count" Decode.int
                    Sum = get.Optional.Field "sum" Decode.float
                    Interpolation = get.Optional.Field "interpolation" Decode.float
                    StepInterpolation = get.Optional.Field "stepInterpolation" Decode.float
                    ContinousVariance = get.Optional.Field "continousVariance" Decode.float
                    DiscreteVariance = get.Optional.Field "descreteVariaance" Decode.float
                    TotalVariation = get.Optional.Field "totalVariance" Decode.float
                })

    type TimeseriesWriteDto with
        member this.Encoder =
            Encode.object [
                if this.ExternalId.IsSome then
                    yield "externalId", Encode.string this.ExternalId.Value
                if this.Name.IsSome then
                    yield "name", Encode.string this.Name.Value
                if this.LegacyName.IsSome then
                    yield "legacyName", Encode.string this.LegacyName.Value
                if this.Description.IsSome then
                    yield "description", Encode.string this.Description.Value
                if this.IsString then
                    yield ("isString", Encode.bool this.IsString)
                if not this.MetaData.IsEmpty then
                    let metaString = Encode.dict (Map.map (fun key value -> Encode.string value) this.MetaData)
                    yield "metadata", metaString
                if this.Unit.IsSome then
                    yield "unit", Encode.string this.Unit.Value
                if this.IsStep then
                    yield "isStep", Encode.bool this.IsStep
                if this.AssetId.IsSome then
                    yield "assetId", Encode.int53 this.AssetId.Value
                if not (Seq.isEmpty this.SecurityCategories) then
                    yield "securityCategories", Encode.seq (Seq.map Encode.int53 this.SecurityCategories)
            ]

    type TimeseriesReadDto with
        static member Decoder : Decoder<TimeseriesReadDto> =
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

