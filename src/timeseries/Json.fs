namespace Cognite.Sdk.Timeseries

open System
open Thoth.Json.Net

[<AutoOpen>]
module TimeseriesExtensions =
    type DataPoint with
        member this.Encoder =
            Encode.object [
                yield ("timestamp", Encode.int64 this.TimeStamp)
                match this.Value with
                | String value -> yield ("value", Encode.string value)
                | Integer value -> yield ("value", Encode.int64 value)
                | Float value -> yield ("value", Encode.float value)
            ]

    type PointRequest with
        member this.Encoder =
            Encode.object [
                yield ("items", List.map (fun (it: DataPoint) -> it.Encoder) this.Items |> Encode.list)
            ]

    type Timeseries with
        member this.Encoder =
            Encode.object [
                yield ("name", Encode.string this.Name)
                yield ("description", Encode.string this.Description)
                yield ("isString", Encode.bool this.IsString)
                if not this.MetaData.IsEmpty then
                    let metaString = Encode.dict (Map.map (fun key value -> Encode.string value) this.MetaData)
                    yield ("metadata", metaString)
                yield ("unit", Encode.string this.Unit)
                yield ("isStep", Encode.bool this.IsStep)
                yield ("assetId", Encode.int64 this.AssetId)
                yield ("securityCategories", Encode.list (List.map Encode.int64 this.SecurityCategories))
            ]
    type TimeseriesRequest with
        member this.Encoder =
            Encode.object [
                yield ("items", List.map (fun (it: Timeseries) -> it.Encoder) this.Items |> Encode.list)
            ]


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
