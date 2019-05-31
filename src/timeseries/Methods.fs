namespace Cognite.Sdk.Timeseries

open Cognite.Sdk
open Cognite.Sdk.Common
open Cognite.Sdk.Request


[<RequireQualifiedAccess>]
module Internal =
    let getTimeseries (query: QueryParams seq) (fetch: HttpHandler) =
        let decoder = decodeResponse TimeseriesResponse.Decoder id
        let url = Url
        let query = query |> Seq.map renderParams |> List.ofSeq

        GET
        >=> setVersion V10
        >=> setResource url
        >=> addQuery query
        >=> fetch
        >=> decoder

    let getTimeseriesResult (name: string) (query: QueryParams seq) (fetch: HttpHandler) (ctx: HttpContext) =
        getTimeseries query fetch ctx
        |> Async.map (fun ctx -> ctx.Result)

    let insertData (items: seq<DataPointsCreateDto>) (fetch: HttpHandler) =
        let request : PointRequest = { Items = items }

        let body = encodeToString request.Encoder
        let url = Url + "/data"

        POST
        >=> setVersion V10
        >=> setBody body
        >=> setResource url

    let insertDataResult (items: seq<DataPointsCreateDto>) (fetch: HttpHandler) (ctx: HttpContext) =
        insertData items fetch ctx
        |> Async.map (fun ctx -> ctx.Result)

    let createTimeseries (items: seq<TimeseriesCreateDto>) (fetch: HttpHandler) =
        let request : TimeseriesRequest = { Items = items }

        let body = encodeToString request.Encoder

        POST
        >=> setVersion V10
        >=> setBody body
        >=> setResource Url

    let createTimeseriesResult (items: seq<TimeseriesCreateDto>) (fetch: HttpHandler) (ctx: HttpContext) =
        createTimeseries items fetch ctx
        |> Async.map (fun ctx -> ctx.Result)

    let getTimeseriesByIds (ids: seq<int64>) (fetch: HttpHandler) =
        let decoder = decodeResponse TimeseriesResponse.Decoder (fun res -> res.Items)
        let url = Url + sprintf "/byids"

        let request : RetrieveRequest = {
            Items = [
                for id in ids do
                    yield { Id = id }
            ]
        }
        let body = encodeToString request.Encoder

        POST
        >=> setVersion V10
        >=> setResource url
        >=> setBody body
        >=> fetch
        >=> decoder

    let getTimeseriesByIdsResult (ids: seq<int64>) (fetch: HttpHandler) (ctx: HttpContext) =
        getTimeseriesByIds ids fetch ctx
        |> Async.map (fun ctx -> ctx.Result)

    let deleteTimeseries (name: string) (fetch: HttpHandler) =
        let url = Url + sprintf "/%s" name

        DELETE
        >=> setVersion V05
        >=> setResource url
        >=> fetch

    let deleteTimeseriesResult (name: string) (fetch: HttpHandler) (ctx: HttpContext) =
        deleteTimeseries name fetch ctx
        |> Async.map (fun ctx -> ctx.Result)

    let getTimeseriesData (defaultArgs: QueryDataParams seq) (args: (int64*(QueryDataParams seq)) seq) (fetch: HttpHandler) =
        let url = Url + "/data/list"
        let decoder = decodeResponse PointResponse.Decoder (fun res -> res.Items)
        let request = renderDataQuery defaultArgs args
        let body = encodeToString request
        printfn "Body: %s" body

        POST
        >=> setVersion V10
        >=> setResource url
        >=> setBody body
        >=> fetch
        >=> decoder

    let getTimeseriesDataResult (defaultArgs: QueryDataParams seq) (args: (int64*(QueryDataParams seq)) seq) (fetch: HttpHandler) (ctx: HttpContext) =
        getTimeseriesData defaultArgs args fetch ctx
        |> Async.map (fun ctx -> ctx.Result)


[<AutoOpen>]
module Methods =

    /// **Description**
    ///
    /// Retrieves a list of all time series in a project, sorted by name
    /// alphabetically. Parameters can be used to select a subset of time
    /// series. This operation supports pagination.
    ///
    /// https://doc.cognitedata.com/api/v1/#operation/getTimeSeries
    ///
    /// **Parameters** * `query` - parameter of type `seq<QueryParams>` * `ctx`
    /// - parameter of type `HttpContext`
    ///
    /// **Output Type** * `HttpHandler<FSharp.Data.HttpResponse,TimeseriesResponse>`
    ///
    let getTimeseries (query: QueryParams seq) (ctx: HttpContext) =
        Internal.getTimeseries query Request.fetch ctx

    /// **Description**
    ///
    /// Inserts a list of data points to a time series. If a data point is
    /// posted to a timestamp that is already in the series, the existing
    /// data point for that timestamp will be overwritten.
    ///
    /// **Parameters**
    ///   * `name` - The name of the timeseries to insert data points into.
    ///   * `items` - The list of data points to insert.
    ///   * `ctx` - The request HTTP context to use.
    ///
    /// **Output Type**
    ///   * `Async<Result<HttpResponse,ResponseError>>`
    ///
    let insertDataByName (items: DataPointsCreateDto list) (ctx: HttpContext) =
        Internal.insertData items Request.fetch ctx

    /// **Description**
    ///
    /// Create new timeseries
    ///
    /// **Parameters**
    ///   * `items` - The list of timeseries to create.
    ///   * `ctx` - The request HTTP context to use.
    ///
    /// **Output Type**
    ///   * `Async<Result<HttpResponse,ResponseError>>`
    ///
    let createTimeseries (items: TimeseriesCreateDto list) (ctx: HttpContext) =
        Internal.createTimeseries items Request.fetch ctx

    /// **Description**
    ///
    /// Get timeseries with given id. Retrieves the details of an existing time
    /// series given a project id and the unique time series identifier
    /// generated when the time series was created.
    ///
    /// **Parameters**
    ///   * `id` - The id of the timeseries to get.
    ///   * `ctx` - The request HTTP context to use.
    ///
    /// **Output Type**
    ///   * `Async<Result<TimeseriesReadDto list,exn>>`
    ///
    let getTimeseriesByIds (ids: seq<int64>) (ctx: HttpContext) =
        Internal.getTimeseriesByIds ids Request.fetch ctx

    /// **Description**
    ///
    /// Query time series. Retrieves a list of data points from a single time series.
    /// This operation supports aggregation but not pagination.
    ///
    /// **Parameters**
    ///   * `name` - parameter of type `string`
    ///   * `query` - parameter of type `QueryParams seq`
    ///   * `ctx` - The request HTTP context to use.
    ///
    /// **Output Type**
    ///   * `Async<Result<HttpResponse,ResponseError>>`
    ///
    let getTimeseriesData (defaultArgs: QueryDataParams seq) (args: (int64*(QueryDataParams seq)) seq)  (ctx: HttpContext) =
        Internal.getTimeseriesData defaultArgs args Request.fetch ctx

    /// **Description**
    ///
    /// Deletes a time series object given the name of the time series.
    ///
    /// **Parameters**
    ///   * `name` - The name of the timeseries to delete.
    ///   * `ctx` - The request HTTP context to use.
    ///
    /// **Output Type**
    ///   * `Async<Result<HttpResponse,ResponseError>>`
    ///
    let deleteTimeseries (name: string) (ctx: HttpContext) =
        Internal.deleteTimeseries name Request.fetch ctx