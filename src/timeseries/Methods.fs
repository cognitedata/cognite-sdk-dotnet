namespace Cognite.Sdk.Timeseries

open Thoth.Json.Net

open Cognite.Sdk
open Cognite.Sdk.Common
open Cognite.Sdk.Request


[<AutoOpen>]
module Methods =
    [<Literal>]
    let Url = "/timeseries"

    /// **Description**
    ///
    /// Inserts a list of data points to a time series. If a data point is
    /// posted to a timestamp that is already in the series, the existing
    /// data point for that timestamp will be overwritten.
    ///
    /// **Parameters**
    ///   * `ctx` - parameter of type `Context`
    ///   * `name` - parameter of type `string`
    ///   * `items` - parameter of type `DataPoint list`
    ///
    /// **Output Type**
    ///   * `Async<Result<string,ResponseError>>`
    ///
    let insertDataByName (ctx: Context) (name: string) (items: DataPointDto list) = async {
        let request : PointRequest = { Items = items }

        let body = Encode.toString 0 request.Encoder
        let url = Url + sprintf "/data/%s" name

        let! response =
            ctx
            |> setMethod POST
            |> setBody body
            |> setResource url
            |> ctx.Fetch
        return response
    }

    /// **Description**
    ///
    /// Create new timeseries
    ///
    /// **Parameters**
    ///   * `ctx` - parameter of type `Context`
    ///   * `items` - parameter of type `Timeseries list`
    ///
    /// **Output Type**
    ///   * `Async<Result<string,exn>>`
    ///
    let createTimeseries (ctx: Context) (items: TimeseriesCreateDto list) = async {
        let request : TimeseriesRequest = { Items = items }

        let body = Encode.toString 0 request.Encoder
        let url = Url

        let! response =
            ctx
            |> setMethod POST
            |> setBody body
            |> setResource url
            |> ctx.Fetch
        return response
    }

    /// **Description**
    ///
    /// Get timeseries with given id. Retrieves the details of an existing time
    /// series given a project id and the unique time series identifier
    /// generated when the time series was created.
    ///
    /// **Parameters**
    ///   * `ctx` - parameter of type `Context`
    ///   * `id` - parameter of type `int64`
    ///
    /// **Output Type**
    ///   * `Async<Result<TimeseriesReadDto list,exn>>`
    ///
    let getTimeseries (ctx: Context) (id: int64) = async {
        let url = Url + sprintf "/%d" id

        let! response =
            ctx
            |> setMethod GET
            |> setResource url
            |> ctx.Fetch

        return response
            |> decodeResponse TimeseriesResponse.Decoder
            |> Result.map (fun res -> res.Data.Items.[0])
    }

    /// **Description**
    ///
    /// Query time series. Retrieves a list of data points from a single time series.
    /// This operation supports aggregation but not pagination.
    ///
    /// **Parameters**
    ///   * `ctx` - parameter of type `Context`
    ///   * `name` - parameter of type `string`
    ///   * `query` - parameter of type `QueryParams list`
    ///
    /// **Output Type**
    ///   * `Async<Result<string,exn>>`
    ///
    let gueryTimeseries (ctx: Context) (name: string) (query: QueryParams list) = async {
        let url = Url + sprintf "/data/%s" name
        let query = query |> List.map renderQuery

        let! response =
            ctx
            |> setMethod GET
            |> setResource url
            |> addQuery query
            |> ctx.Fetch

        return response
            |> decodeResponse PointResponse.Decoder
            |> Result.map (fun res -> res.Data.Items)
    }

    /// **Description**
    ///
    /// Deletes a time series object given the name of the time series.
    ///
    /// **Parameters**
    ///   * `ctx` - The client context. Parameter of type `Context`
    ///   * `name` - The name of timeseries to delete. Parameter of type `string`.
    ///
    /// **Output Type**
    ///   * `Async<Result<string,ResponseError>>`
    ///
    let deleteTimeseries (ctx: Context) (name: string) = async {
        let url = Url + sprintf "/data/%s" name

        let! response =
            ctx
            |> setMethod DELETE
            |> setResource url
            |> ctx.Fetch

        return response
    }