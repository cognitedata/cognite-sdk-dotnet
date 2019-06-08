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
                | NumString value -> yield "value", Encode.string value
                | NumInteger value -> yield "value", Encode.int53 value
                | NumFloat value -> yield "value", Encode.float value
            ]

        static member Decoder : Decoder<DataPointDto> =
            Decode.object (fun get ->
                {
                    TimeStamp = get.Required.Field "timestamp" Decode.int64
                    Value = get.Required.Field "value" (Decode.oneOf [
                            Decode.int64 |> Decode.map NumInteger
                            Decode.float |> Decode.map NumFloat
                            Decode.string |> Decode.map NumString
                        ])
                })

    type DataPointsCreateDto with
        member this.Encoder =
            Encode.object [
                yield ("datapoints", Seq.map (fun (it: DataPointDto) -> it.Encoder) this.DataPoints |> Encode.seq)
                match this.Identity with
                | Identity.Id id -> yield "id", Encode.int64 id
                | Identity.ExternalId externalId -> yield "externalId", Encode.string externalId
            ]

    type PointRequest with
        member this.Encoder =
            Encode.object [
                yield ("items", Seq.map (fun (it: DataPointsCreateDto) -> it.Encoder) this.Items |> Encode.seq)
            ]

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

    type PointResponseDataPoints with
        static member Decoder : Decoder<PointResponseDataPoints> =
            Decode.object (fun get ->
                {
                    Id = get.Required.Field "id" Decode.int64
                    ExternalId = get.Optional.Field "exteralId" Decode.string
                    IsString = get.Required.Field "isString" Decode.bool
                    DataPoints = get.Required.Field "datapoints" (Decode.list DataPointDto.Decoder)
                })
    type PointResponseAggregateDataPoints with
        static member Decoder : Decoder<PointResponseAggregateDataPoints> =
            Decode.object (fun get ->
                {
                    Id = get.Required.Field "id" Decode.int64
                    ExternalId = get.Optional.Field "exteralId" Decode.string
                    DataPoints = get.Required.Field "datapoints" (Decode.list AggregateDataPointReadDto.Decoder)
                })

    type PointResponse with
        static member Decoder : Decoder<PointResponse> =
            Decode.object (fun get ->
                {
                    Items = get.Required.Field "items" (Decode.list PointResponseDataPoints.Decoder)
                })
    type AggregatePointResponse with
        static member Decoder : Decoder<AggregatePointResponse> =
            Decode.object (fun get ->
                {
                    Items = get.Required.Field "items" (Decode.list PointResponseAggregateDataPoints.Decoder)
                })

    type TimeseriesCreateDto with
        member this.Encoder =
            Encode.object [
                if this.ExternalId.IsSome then
                    yield "externalId", Encode.string this.ExternalId.Value
                if this.Name.IsSome then
                    yield "name", Encode.string this.Name.Value
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

    type TimeseriesRequest with
        member this.Encoder =
            Encode.object [
                yield ("items", Seq.map (fun (it: TimeseriesCreateDto) -> it.Encoder) this.Items |> Encode.seq)
            ]

    type TimeseriesResponse with
        static member Decoder : Decoder<TimeseriesResponse> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list TimeseriesReadDto.Decoder)
                NextCursor = get.Optional.Field "nextCursor" Decode.string
            })

    let renderParams (arg: QueryParam) =
        match arg with
        | Limit limit -> "limit", limit.ToString ()
        | IncludeMetaData imd -> "includeMetadata", imd.ToString().ToLower()
        | Cursor cursor -> "cursor", cursor
        | AssetIds ids ->
            let list = ids |> Seq.map (fun a -> a.ToString ()) |> seq<string>
            "assetIds", sprintf "[%s]" (String.Join (",", list))

    let renderQuery (param: QueryDataParam) : string*Thoth.Json.Net.JsonValue =
        match param with
        | Start start -> "start", Encode.string start
        | End end'  -> "end", Encode.string end'
        | Aggregates aggr ->
            let aggregates = aggr |> Seq.map (fun a -> Encode.string (a.ToString ())) |> Array.ofSeq
            "aggregates", Encode.array aggregates
        | Granularity gr -> "granularity", Encode.string (gr.ToString ())
        | QueryDataParam.Limit limit -> "limit", Encode.int limit
        | IncludeOutsidePoints iop -> "includeOutsidePoints", Encode.bool iop

    let renderDataQuery (defaultQuery: QueryDataParam seq) (args: (int64*(QueryDataParam seq)) seq) =
        Encode.object [
            yield "items", Encode.list [
                for (id, arg) in args do
                    yield Encode.object [
                        for param in arg do
                            yield renderQuery param
                        yield "id", Encode.int64 id
                    ]
            ]

            for param in defaultQuery do
                yield renderQuery param
        ]

    let renderLatestOption (option: QueryLatestParam) : string*Thoth.Json.Net.JsonValue =
        match option with
        | Before before -> "before", Encode.string before
        | QueryLatestParam.Id id -> "Id", Encode.int53 id
        | QueryLatestParam.ExternalId externalId -> "externalId", Encode.string externalId

(*
    let renderDataLatestQuery (options: QueryLatestParam seq) =
        Encode.object [
            for option in options do
                yield renderLatestOption option
        ]
*)
    type LatestDataRequest with
        member this.Encoder =
            Encode.object [
                if this.Before.IsSome then
                    yield "before", Encode.string this.Before.Value
                match this.Identity with
                | Identity.Id id -> yield "id", Encode.int53 id
                | Identity.ExternalId id -> yield "externalId", Encode.string id
            ]

    type TimeseriesLatestRequest with
        member this.Encoder =
            Encode.object [
                yield ("items", Seq.map (fun (it: LatestDataRequest) -> it.Encoder) this.Items |> Encode.seq)
            ]


    type Item with
        member this.Encoder =
            Encode.object [
                yield "id", Encode.int53 this.Id
            ]
    type TimeseriesReadRequest with
        member this.Encoder =
            Encode.object [
                yield ("items", Seq.map (fun (it: Item) -> it.Encoder) this.Items |> Encode.seq)
            ]


    type Granularity with
        member this.ToString () =
            match this with
            | Day day when day = 1 -> "d"
            | Day day -> sprintf "d%d" day
            | Hour hour when hour = 1 -> "h"
            | Hour hour -> sprintf "h%d" hour
            | Minute min when min = 1 -> "m"
            | Minute min -> sprintf "m%d" min
            | Second sec when sec = 1 -> "s"
            | Second sec -> sprintf "s%d" sec
