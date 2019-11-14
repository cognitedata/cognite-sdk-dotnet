// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.TimeSeries

open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive
open Oryx
open Thoth.Json.Net

open CogniteSdk


[<RequireQualifiedAccess>]
module Delete =
    [<Literal>]
    let Url = "/timeseries/delete"

    type private DeleteRequest = {
        Items: seq<Identity>
    } with
        member this.Encoder =
            Encode.object [
                yield ("items", Seq.map (fun (it: Identity) -> it.Encoder) this.Items |> Encode.seq)
            ]

    let deleteCore (items: Identity seq) (fetch: HttpHandler<HttpResponseMessage, HttpResponseMessage>) =
        let request : DeleteRequest = { Items = items }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> withError decodeError

    /// <summary>
    /// Delete one or more timeseries.
    /// </summary>
    /// <param name="items">List of timeseries ids to delete.</param>
    /// <param name="next">Async handler to use.</param>
    let delete (items: Identity seq) (next: NextFunc<HttpResponseMessage, HttpResponseMessage>) =
        deleteCore items fetch next

    /// <summary>
    /// Delete one or more timeseries.
    /// </summary>
    /// <param name="items">List of timeseries ids to delete.</param>
    let deleteAsync (items: Identity seq) =
        deleteCore items fetch finishEarly

[<Extension>]
type DeleteTimeSeriesClientExtensions =
    /// <summary>
    /// Delete one or more timeseries.
    /// </summary>
    /// <param name="items">List of timeseries ids to delete.</param>
    [<Extension>]
    static member DeleteAsync (this: ClientExtension, items: Identity seq, [<Optional>] token: CancellationToken) : Task =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Delete.deleteAsync items ctx
            match result with
            | Ok _ -> return ()
            | Error error -> return raiseError error
        } :> Task
