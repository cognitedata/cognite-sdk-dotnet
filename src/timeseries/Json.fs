namespace Cognite.Sdk.Timeseries

open System
open Thoth.Json.Net

[<AutoOpen>]
module TimeseriesExtensions =
    type DataPointCreateDto with
        member this.Encoder =
            Encode.object [
                yield "timestamp", Encode.int64 this.TimeStamp
                match this.Value with
                | String value -> yield "value", Encode.string value
                | Integer value -> yield "value", Encode.int64 value
                | Float value -> yield "value", Encode.float value
            ]

    type PointRequest with
        member this.Encoder =
            Encode.object [
                yield ("items", Seq.map (fun (it: DataPointCreateDto) -> it.Encoder) this.Items |> Encode.seq)
            ]

    type DataPointReadDto with
        static member Decoder : Decoder<DataPointReadDto> =
            Decode.object (fun get ->
                {
                    TimeStamp = get.Required.Field "timestamp" Decode.int64
                    Value = get.Required.Field "value" (Decode.oneOf [
                            Decode.int64 |> Decode.map Integer
                            Decode.float |> Decode.map Float
                            Decode.string |> Decode.map Numeric.String
                        ])
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

    type PointResponseDataPoints with
        static member Decoder : Decoder<PointResponseDataPoints> =
            Decode.object (fun get ->
                {
                    Name = get.Required.Field "name" Decode.string
                    DataPoints = get.Required.Field "items" (Decode.list DataPointReadDto.Decoder)
                })

    type PointResponseData with
        static member Decoder : Decoder<PointResponseData> =
            Decode.object (fun get ->
                {
                    Items = get.Required.Field "items" (Decode.list PointResponseDataPoints.Decoder)
                })

    type PointResponse with
        static member Decoder : Decoder<PointResponse> =
            Decode.object (fun get -> {
                Data = get.Required.Field "data" PointResponseData.Decoder
            })

    type TimeseriesCreateDto with
        member this.Encoder =
            Encode.object [
                yield "name", Encode.string this.Name
                if this.Description.IsSome then
                    yield "description", Encode.string this.Description.Value
                if this.IsString.IsSome then
                    yield ("isString", Encode.bool this.IsString.Value)
                if not this.MetaData.IsEmpty then
                    let metaString = Encode.dict (Map.map (fun key value -> Encode.string value) this.MetaData)
                    yield "metadata", metaString
                if this.Unit.IsSome then
                    yield "unit", Encode.string this.Unit.Value
                if this.IsStep.IsSome then
                    yield "isStep", Encode.bool this.IsStep.Value
                if this.AssetId.IsSome then
                    yield "assetId", Encode.int64 this.AssetId.Value
                if not (Seq.isEmpty this.SecurityCategories) then
                    yield "securityCategories", Encode.seq (Seq.map Encode.int64 this.SecurityCategories)
            ]

    type TimeseriesReadDto with
        static member Decoder : Decoder<TimeseriesReadDto> =
            Decode.object (fun get ->
                {
                    Name = get.Required.Field "name" Decode.string
                    Description = get.Optional.Field "description" Decode.string
                    IsString = get.Optional.Field "isString" Decode.bool
                    MetaData = get.Optional.Field "metadata" (Decode.dict Decode.string)
                    Unit = get.Optional.Field "unit" Decode.string
                    AssetId = get.Optional.Field "assetId" Decode.int64
                    IsStep = get.Optional.Field "isStep" Decode.bool
                    SecurityCategories = get.Optional.Field "securityCategories" ((Decode.array Decode.int64) |> Decode.map seq)
                    CreatedTime = get.Required.Field "createdTime" Decode.int64
                    LastUpdatedTime = get.Required.Field "lastUpdatedTime" Decode.int64
                })

    type TimeseriesRequest with
        member this.Encoder =
            Encode.object [
                yield ("items", Seq.map (fun (it: TimeseriesCreateDto) -> it.Encoder) this.Items |> Encode.seq)
            ]

    type TimeseriesResponseData with
        static member Decoder : Decoder<TimeseriesResponseData> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list TimeseriesReadDto.Decoder)
            })

    type TimeseriesResponse with
        static member Decoder : Decoder<TimeseriesResponse> =
            Decode.object (fun get -> {
                Data = get.Required.Field "data" TimeseriesResponseData.Decoder
            })

    let renderQuery (query: QueryParams) =
        match query with
        | Start start -> "start", start.ToString ()
        | End end'  -> "end", end'.ToString ()
        | Aggregates aggr ->
            let list = aggr |> Seq.map (fun a -> a.ToString ()) |> seq<string>
            "aggregates", String.Join(",", list)
        | Granularity gr -> ("granularity", gr.ToString ())
        | Limit limit -> "limit", limit.ToString ()
        | IncludeOutsidePoints iop -> "includeOutsidePoints", iop.ToString()
