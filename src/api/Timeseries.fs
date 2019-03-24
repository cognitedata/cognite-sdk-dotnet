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
    /// Delete timeseries.
    /// </summary>
    /// <param name="name">The name of the time series to delete.</param>
    /// <returns>List of created assets.</returns>
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
