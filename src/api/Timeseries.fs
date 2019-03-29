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
    static member TryGetAverage (this: DataPointReadDto, [<Out>] value: byref<float>) =
        match this.Average with
        | Some value' ->
            value <- value'
            true
        | _ -> false

    [<Extension>]
    static member TryGetMax (this: DataPointReadDto, [<Out>] value: byref<float>) =
        match this.Max with
        | Some value' ->
            value <- value'
            true
        | _ -> false

    [<Extension>]
    static member TryGetMin (this: DataPointReadDto, [<Out>] value: byref<float>) =
        match this.Min with
        | Some value' ->
            value <- value'
            true
        | _ -> false

    [<Extension>]
    static member TryGetCount (this: DataPointReadDto, [<Out>] value: byref<int>) =
        match this.Count with
        | Some value' ->
            value <- value'
            true
        | _ -> false

    [<Extension>]
    static member TryGetSum (this: DataPointReadDto, [<Out>] value: byref<float>) =
        match this.Sum with
        | Some value' ->
            value <- value'
            true
        | _ -> false

    [<Extension>]
    static member TryGetInterpolation (this: DataPointReadDto, [<Out>] value: byref<float>) =
        match this.Interpolation with
        | Some value' ->
            value <- value'
            true
        | _ -> false

    [<Extension>]
    static member TryGetStepInterpolation (this: DataPointReadDto, [<Out>] value: byref<float>) =
        match this.StepInterpolation with
        | Some value' ->
            value <- value'
            true
        | _ -> false

    [<Extension>]
    static member TryGetContinousVariance (this: DataPointReadDto, [<Out>] value: byref<float>) =
        match this.ContinousVariance with
        | Some value' ->
            value <- value'
            true
        | _ -> false

    [<Extension>]
    static member TryGetDiscreteVariance (this: DataPointReadDto, [<Out>] value: byref<float>) =
        match this.DiscreteVariance with
        | Some value' ->
            value <- value'
            true
        | _ -> false

    [<Extension>]
    static member TryGetTotalVariation (this: DataPointReadDto, [<Out>] value: byref<float>) =
        match this.TotalVariation with
        | Some value' ->
            value <- value'
            true
        | _ -> false

type Query (query: QueryParams list) =
    let query = query

    member this.Start (start: int64) =
        Query (Start start :: query)

    member this.End (endTime: int64) =
        Query (End endTime :: query)

    member this.Limit (limit: int) =
        Query (Limit limit :: query)

    member this.Aggregates (aggregate: ResizeArray<Aggregate>) =
        Query (QueryParams.Aggregates (List.ofSeq aggregate) :: query)

    member this.IncludeOutsidePoints (iop: bool) =
        Query (IncludeOutsidePoints iop :: query)

    member internal this.Query = query

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
    static member QueryTimeseriesAsync (this: Client) (name: string) (query: Query) : Task<ResizeArray<PointResponseDataPoints>> =
        let worker () : Async<ResizeArray<PointResponseDataPoints>> = async {
            let! result = gueryTimeseries this.Ctx name (List.ofSeq query.Query)
            match result with
            | Ok response ->
                return response |> ResizeArray
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
    static member InsertDataByNameAsync (this: Client) (name: string) (items: ResizeArray<DataPointCreateDto>) : Task<int> =
        let worker () : Async<int> = async {
            let! result = insertDataByName this.Ctx name (List.ofSeq items)
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
    static member CreateTimeseriesAsync (this: Client) (items: ResizeArray<TimeseriesCreateDto>) : Task<int> =
        let worker () : Async<int> = async {
            let! result = createTimeseries this.Ctx (List.ofSeq items)
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
    static member GetTimeseriesAsync (this: Client) (id: int64) : Task<TimeseriesReadDto> =
        let worker () : Async<TimeseriesReadDto> = async {
            let! result = getTimeseries this.Ctx id

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
            let! result = deleteTimeseries this.Ctx name
            match result with
            | Ok response ->
                return response.StatusCode
            | Error error ->
               return raise (Error.error2Exception error)
        }

        worker () |> Async.StartAsTask
