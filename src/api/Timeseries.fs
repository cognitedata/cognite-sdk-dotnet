namespace Cognite.Sdk.Api

open System
open System.Threading.Tasks;
open System.Runtime.InteropServices
open System.Runtime.CompilerServices

open Cognite.Sdk
open Cognite.Sdk.Api
open Cognite.Sdk.Timeseries

[<Extension>]
type TimeseriesExtension =
    [<Extension>]
    static member TryGetValue (this: DataPointCreateDto, [<Out>] value: byref<Int64>) =
        match this.Value with
        | Integer value' ->
            value <- value'
            true
        | _ -> false

    [<Extension>]
    static member TryGetValue (this: DataPointCreateDto, [<Out>] value: byref<string>) =
        match this.Value with
        | String value' ->
            value <- value'
            true
        | _ -> false

    [<Extension>]
    static member TryGetValue (this: DataPointCreateDto, [<Out>] value: byref<float>) =
        match this.Value with
        | Float value' ->
            value <- value'
            true
        | _ -> false

    [<Extension>]
    static member TryGetValue (this: DataPointReadDto, [<Out>] value: byref<Int64>) =
        match this.Value with
        | Integer value' ->
            value <- value'
            true
        | _ -> false

    [<Extension>]
    static member TryGetValue (this: DataPointReadDto, [<Out>] value: byref<string>) =
        match this.Value with
        | String value' ->
            value <- value'
            true
        | _ -> false

    [<Extension>]
    static member TryGetValue (this: DataPointReadDto, [<Out>] value: byref<float>) =
        match this.Value with
        | Float value' ->
            value <- value'
            true
        | _ -> false

    [<Extension>]
    static member TryGetAverage (this: AggregateDataPointReadDto, [<Out>] value: byref<float>) =
        match this.Average with
        | Some value' ->
            value <- value'
            true
        | _ -> false

    [<Extension>]
    static member TryGetMax (this: AggregateDataPointReadDto, [<Out>] value: byref<float>) =
        match this.Max with
        | Some value' ->
            value <- value'
            true
        | _ -> false

    [<Extension>]
    static member TryGetMin (this: AggregateDataPointReadDto, [<Out>] value: byref<float>) =
        match this.Min with
        | Some value' ->
            value <- value'
            true
        | _ -> false

    [<Extension>]
    static member TryGetCount (this: AggregateDataPointReadDto, [<Out>] value: byref<int>) =
        match this.Count with
        | Some value' ->
            value <- value'
            true
        | _ -> false

    [<Extension>]
    static member TryGetSum (this: AggregateDataPointReadDto, [<Out>] value: byref<float>) =
        match this.Sum with
        | Some value' ->
            value <- value'
            true
        | _ -> false

    [<Extension>]
    static member TryGetInterpolation (this: AggregateDataPointReadDto, [<Out>] value: byref<float>) =
        match this.Interpolation with
        | Some value' ->
            value <- value'
            true
        | _ -> false

    [<Extension>]
    static member TryGetStepInterpolation (this: AggregateDataPointReadDto, [<Out>] value: byref<float>) =
        match this.StepInterpolation with
        | Some value' ->
            value <- value'
            true
        | _ -> false

    [<Extension>]
    static member TryGetContinousVariance (this: AggregateDataPointReadDto, [<Out>] value: byref<float>) =
        match this.ContinousVariance with
        | Some value' ->
            value <- value'
            true
        | _ -> false

    [<Extension>]
    static member TryGetDiscreteVariance (this: AggregateDataPointReadDto, [<Out>] value: byref<float>) =
        match this.DiscreteVariance with
        | Some value' ->
            value <- value'
            true
        | _ -> false

    [<Extension>]
    static member TryGetTotalVariation (this: AggregateDataPointReadDto, [<Out>] value: byref<float>) =
        match this.TotalVariation with
        | Some value' ->
            value <- value'
            true
        | _ -> false

type Query (query: QueryDataParams list) =
    let query = query

    member this.Start (start: string) =
        Query (Start start :: query)

    member this.End (endTime: string) =
        Query (End endTime :: query)

    member this.Limit (limit: int) =
        Query (QueryDataParams.Limit limit :: query)

    member this.Aggregates (aggregate: Aggregate seq) =
        Query (QueryDataParams.Aggregates aggregate :: query)

    member this.IncludeOutsidePoints (iop: bool) =
        Query (IncludeOutsidePoints iop :: query)

    member internal this.Query = Seq.ofList query

    static member Create () =
        Query []

[<Extension>]
type ClientTimeseriesExtensions =
    /// <summary>
    /// Retrieves a list of data points from a single time series.
    /// </summary>
    /// <param name="name">The name of the timeseries to insert data into.</param>
    /// <param name="items">The list of data points to insert.</param>
    /// <returns>Http status code.</returns>

    [<Extension>]
    static member GetTimeseriesDataAsync (this: Client) (defaultQuery: Query) (query: Tuple<int64, Query> seq) : Task<seq<PointResponseDataPoints>> =
        let worker () : Async<seq<PointResponseDataPoints>> = async {
            let defaultQuery' = defaultQuery.Query
            let query' = query |> Seq.map (fun (id, query) -> (id, query.Query))
            let! result = Internal.getTimeseriesDataResult defaultQuery' query'  this.Fetch this.Ctx
            match result with
            | Ok response ->
                return response
            | Error error ->
               return raise (Error.error2Exception error)
        }

        worker () |> Async.StartAsTask

    /// <summary>
    /// Insert data into named time series.
    /// </summary>
    /// <param name="name">The name of the timeseries to insert data into.</param>
    /// <param name="items">The list of data points to insert.</param>
    /// <returns>Http status code.</returns>
    [<Extension>]
    static member InsertDataByNameAsync (this: Client) (name: string) (items: DataPointCreateDto seq) : Task<int> =
        let worker () : Async<int> = async {
            let! result = Internal.insertDataByNameResult name items this.Fetch this.Ctx
            match result with
            | Ok response ->
                return response.StatusCode
            | Error error ->
               return raise (Error.error2Exception error)
        }

        worker () |> Async.StartAsTask

    /// <summary>
    /// Create a new timeseries.
    /// </summary>
    /// <param name="items">The list of timeseries to create.</param>
    /// <returns>Http status code.</returns>
    [<Extension>]
    static member CreateTimeseriesAsync (this: Client) (items: seq<TimeseriesCreateDto>) : Task<int> =
        let worker () : Async<int> = async {
            let! result = Internal.createTimeseriesResult items this.Fetch this.Ctx
            match result with
            | Ok response ->
                return response.StatusCode
            | Error error ->
               return raise (Error.error2Exception error)
        }

        worker () |> Async.StartAsTask

    /// <summary>
    /// Get timeseries with given id.
    /// </summary>
    /// <param name="id">The id of the timeseries to get.</param>
    /// <returns>The timeseries with the given id.</returns>
    [<Extension>]
    static member GetTimeseriesByIdsAsync (this: Client) (ids: seq<int64>) : Task<seq<TimeseriesReadDto>> =
        let worker () : Async<seq<TimeseriesReadDto>> = async {
            let! result = Internal.getTimeseriesByIdsResult ids this.Fetch this.Ctx

            match result with
            | Ok response ->
                return response
            | Error error ->
               return raise (Error.error2Exception error)
        }

        worker () |> Async.StartAsTask

    /// <summary>
    /// Delete timeseries.
    /// </summary>
    /// <param name="name">The name of the timeseries to delete.</param>
    /// <returns>List of created timeseries.</returns>
    [<Extension>]
    static member DeleteTimeseriesAsync (this: Client) (name: string) : Task<int> =
        let worker () : Async<int> = async {
            let! result = Internal.deleteTimeseriesResult name this.Fetch this.Ctx
            match result with
            | Ok response ->
                return response.StatusCode
            | Error error ->
               return raise (Error.error2Exception error)
        }

        worker () |> Async.StartAsTask
