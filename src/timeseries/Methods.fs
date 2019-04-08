namespace Cognite.Sdk.Timeseries

open Thoth.Json.Net

open Cognite.Sdk
open Cognite.Sdk.Common
open Cognite.Sdk.Request

[<RequireQualifiedAccess>]
module Internal =
    let insertDataByName (name: string) (items: DataPointCreateDto list) (fetch: HttpHandler) =
        let request : PointRequest = { Items = items }

        let body = Encode.toString 0 request.Encoder
        let url = Url + sprintf "/data/%s" name

        POST
        >=> setBody body
        >=> setResource url

    let insertDataByNameResult (name: string) (items: DataPointCreateDto list) (fetch: HttpHandler) (ctx: HttpContext) =
        insertDataByName name items fetch ctx
        |> Async.map (fun ctx -> ctx.Result)

    let createTimeseries (items: TimeseriesCreateDto list) (fetch: HttpHandler) =
        let request : TimeseriesRequest = { Items = items }

        let body = Encode.toString 0 request.Encoder

        POST
        >=> setBody body
        >=> setResource Url

    let createTimeseriesResult (items: TimeseriesCreateDto list) (fetch: HttpHandler) (ctx: HttpContext) =
        createTimeseries items fetch ctx
        |> Async.map (fun ctx -> ctx.Result)

    let getTimeseries (id: int64) (fetch: HttpHandler) =
        let decoder = decodeResponse TimeseriesResponse.Decoder (fun res -> res.Data.Items.[0])
        let url = Url + sprintf "/%d" id

        GET
        >=> setResource url
        >=> decoder

    let getTimeseriesResult (id: int64) (fetch: HttpHandler) (ctx: HttpContext) =
        getTimeseries id fetch ctx
        |> Async.map (fun ctx -> ctx.Result)

    let queryTimeseries (name: string) (query: QueryParams list) (fetch: HttpHandler) =
        let decoder = decodeResponse PointResponse.Decoder (fun res -> res.Data.Items)
        let url = Url + sprintf "/data/%s" name
        let query = query |> List.map renderQuery

        GET
        >=> setResource url
        >=> addQuery query
        >=> fetch
        >=> decoder

    let queryTimeseriesResult (name: string) (query: QueryParams list) (fetch: HttpHandler) (ctx: HttpContext) =
        queryTimeseries name query fetch ctx
        |> Async.map (fun ctx -> ctx.Result)

    let deleteTimeseries (name: string) (fetch: HttpHandler) =
        let url = Url + sprintf "/data/%s" name

        DELETE
        >=> setResource url
        >=> fetch

    let deleteTimeseriesResult (name: string) (fetch: HttpHandler) (ctx: HttpContext) =
        deleteTimeseries name fetch ctx
        |> Async.map (fun ctx -> ctx.Result)

[<AutoOpen>]
module Methods =

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
    let insertDataByName (name: string) (items: DataPointCreateDto list) (ctx: HttpContext) =
        Internal.insertDataByNameResult name items Request.fetch ctx

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
    let createTimeseries (items: TimeseriesCreateDto list) (ctx: HttpContext) =
        Internal.createTimeseriesResult items Request.fetch ctx

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
    let getTimeseries (id: int64) (ctx: HttpContext) =
        Internal.getTimeseriesResult id Request.fetch ctx

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
    let queryTimeseries (name: string) (query: QueryParams list) (ctx: HttpContext) =
        Internal.queryTimeseries name query Request.fetch ctx


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
    let deleteTimeseries (name: string) (ctx: HttpContext) =
        Internal.deleteTimeseriesResult name Request.fetch ctx