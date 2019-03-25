namespace Cognite.Sdk.Api

open System
open System.Threading.Tasks;
open System.Runtime.InteropServices
open System.Runtime.CompilerServices
open System.Collections.Generic

open Cognite.Sdk
open Cognite.Sdk.Api
open Cognite.Sdk.Timeseries

[<Extension>]
type TimeseriesExtension =
    exn


[<Extension>]
type ClientTimeseriesExtensions =
    /// <summary>
    /// Retrieves a list of data points from a single time series.
    /// </summary>
    /// <param name="name">The name of the timeseries to insert data into.</param>
    /// <param name="items">The list of data points to insert.</param>
    /// <returns>Http status code.</returns>
    [<Extension>]
    static member QueryTimeseries (this: Client) (name: string) (query: ResizeArray<QueryParams>) : Task<ResizeArray<DataPointDto>> =
        let worker () : Async<ResizeArray<DataPointDto>> = async {
            let! result = gueryTimeseries this.Ctx name (List.ofSeq query)
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
    static member InsertDataByName (this: Client) (name: string) (items: ResizeArray<DataPointDto>) : Task<int> =
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
    static member CreateTimeseries (this: Client) (items: ResizeArray<TimeseriesCreateDto>) : Task<int> =
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
    static member GetTimeseries (this: Client) (id: int64) : Task<TimeseriesReadDto> =
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
    static member DeleteTimeseries (this: Client) (name: string) : Task<int> =
        let worker () : Async<int> = async {
            let! result = deleteTimeseries this.Ctx name
            match result with
            | Ok response ->
                return response.StatusCode
            | Error error ->
               return raise (Error.error2Exception error)
        }

        worker () |> Async.StartAsTask
