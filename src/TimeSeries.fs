namespace Cognite.Sdk

open System
open Thoth.Json.Net

open Cognite.Sdk.Context

type Numeric =
    | String of string
    | Integer of int64
    | Float of double

type DataPoint = {
    TimeStamp: int64
    Value: Numeric } with

    member this.Encoder =
        Encode.object [
            yield ("timestamp", Encode.int64 this.TimeStamp)
            match this.Value with
            | String value -> yield ("value", Encode.string value)
            | Integer value -> yield ("value", Encode.int64 value)
            | Float value -> yield ("value", Encode.float value)
        ]

module TimeSeries =
    [<Literal>]
    let Url = "/timeseries"

    type Request = {
        Items: DataPoint list } with

        member this.Encoder =
            Encode.object [
                yield ("items", List.map (fun (it: DataPoint) -> it.Encoder) this.Items |> Encode.list)
            ]

    /// **Description**
    ///
    /// **Parameters**
    ///   * `ctx` - parameter of type `Context`
    ///   * `name` - parameter of type `string`
    ///   * `items` - parameter of type `DataPoint list`
    ///
    /// **Output Type**
    ///   * `Async<unit>`
    ///
    /// **Exceptions**
    ///
    let insertData (ctx: Context) (name: string) (items: DataPoint list) = async {
        let request = { Items = items }

        let body = Encode.toString 0 request.Encoder
        let url = Url + sprintf "/update"

        let! response =
            ctx
            |> setMethod Post
            |> setBody body
            |> setResource url
            |> ctx.Fetch
        return response
    }
