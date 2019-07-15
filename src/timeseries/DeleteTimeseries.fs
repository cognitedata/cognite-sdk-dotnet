namespace Cognite.Sdk

open System.IO
open System.Net.Http
open System.Runtime.CompilerServices
open System.Threading.Tasks

open FSharp.Control.Tasks.V2
open Thoth.Json.Net

open Cognite.Sdk
open Cognite.Sdk.Api
open Cognite.Sdk.Common

[<RequireQualifiedAccess>]
module DeleteTimeseries =
    [<Literal>]
    let Url = "/timeseries/delete"

    /// Used for retrieving multiple time series
    type DeleteRequest = {
        /// Sequence of items to retrieve
        Items: seq<Identity>
    } with
        member this.Encoder =
            Encode.object [
                yield ("items", Seq.map (fun (it: Identity) -> it.Encoder) this.Items |> Encode.seq)
            ]

    let deleteTimeseries (items: Identity seq) (fetch: HttpHandler<HttpResponseMessage, Stream, bool>) =
        let request : DeleteRequest = { Items = items }
        let body = Encode.stringify request.Encoder

        POST
        >=> setVersion V10
        >=> setBody body
        >=> setResource Url
        >=> fetch
        >=> dispose

[<AutoOpen>]
module DeleteTimeseriesApi =
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
    let deleteTimeseries (items: Identity seq) (next: NextHandler<bool, bool>) =
        DeleteTimeseries.deleteTimeseries items fetch next

    let deleteTimeseriesAsync (items: Identity seq) =
        DeleteTimeseries.deleteTimeseries items fetch Async.single

[<Extension>]
type DeleteTimeseriesExtensions =
  /// <summary>
    /// Delete timeseries.
    /// </summary>
    /// <param name="name">The name of the timeseries to delete.</param>
    /// <returns>List of created timeseries.</returns>
    [<Extension>]
    static member DeleteTimeseriesAsync (this: Client) (items: Identity seq) : Task<bool> =
        task {
            let! ctx = deleteTimeseriesAsync items this.Ctx
            match ctx.Result with
            | Ok response ->
                return true
            | Error error ->
                let! err = error2Exception error
                return raise err
        }
