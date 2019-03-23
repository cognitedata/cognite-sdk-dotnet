namespace Cognite.Sdk.Timeseries

open System
open Thoth.Json.Net

[<AutoOpen>]
module TimeseriesExtensions =
    type DataPointDto with
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
                yield ("items", List.map (fun (it: DataPointDto) -> it.Encoder) this.Items |> Encode.list)
            ]

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
                if not this.SecurityCategories.IsEmpty then
                    yield "securityCategories", Encode.list (List.map Encode.int64 this.SecurityCategories)
            ]
    type TimeseriesRequest with
        member this.Encoder =
            Encode.object [
                yield ("items", List.map (fun (it: TimeseriesCreateDto) -> it.Encoder) this.Items |> Encode.list)
            ]

    type TimeseriesResponse with
        member this.Decoder =
            ()

    let renderQuery (query: QueryParams) =
        match query with
        | Start start -> ("start", start.ToString ())
        | End end'  -> ("end", end'.ToString ())
        | Aggregates aggr ->
            let list = aggr |> List.map (fun a -> a.ToString ()) |> ResizeArray<string>
            ("aggregates", String.Join(",", list))
        | Granularity gr -> ("granularity", gr.ToString ())
        | Limit limit -> ("limit", limit.ToString ())
        | IncludeOutsidePoints iop -> ("includeOutsidePoints", iop.ToString())
