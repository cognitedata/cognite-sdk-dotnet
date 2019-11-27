// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Sequences

open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive
open Oryx
open Oryx.ResponseReaders
open Thoth.Json.Net

open CogniteSdk

[<AutoOpen>]
module Retrieve =
    [<Literal>]
    let Url = "/sequences/byids"

    type SequenceRequest = {
        Items: Identity seq
    } with
        member this.Encoder  =
            Encode.object [
                yield "items", this.Items |> Seq.map(fun id -> id.Encoder) |> Encode.seq
            ]

    type SequenceResponse = {
        Items: SequenceReadDto seq
    } with
         static member Decoder : Decoder<SequenceResponse> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list SequenceReadDto.Decoder |> Decode.map seq)
            })

    let getByIdsCore (ids: Identity seq) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let request : SequenceRequest = { Items = ids }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> withError decodeError
        >=> json SequenceResponse.Decoder
        >=> map (fun response -> response.Items)

    /// <summary>
    /// Retrieves information about multiple Sequences in the same project.
    /// A maximum of 1000 Sequences IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="SequenceId">The ids of the Sequences to get.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>Sequences with given ids.</returns>
    let getByIds (ids: Identity seq) (next: NextFunc<SequenceReadDto seq,'a>) : HttpContext -> HttpFuncResult<'a> =
        getByIdsCore ids fetch next

    /// <summary>
    /// Retrieves information about multiple Sequences in the same project.
    /// A maximum of 1000 Sequences IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="SequenceId">The ids of the Sequences to get.</param>
    /// <returns>Sequences with given ids.</returns>
    let getByIdsAsync (ids: Identity seq) =
        getByIdsCore ids fetch finishEarly


[<Extension>]
type GetSequencesByIdsClientExtensions =
    /// <summary>
    /// Retrieves information about multiple Sequences in the same project.
    /// A maximum of 1000 Sequences IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="SequenceId">The ids of the Sequences to get.</param>
    /// <returns>Sequences with given ids.</returns>
    [<Extension>]
    static member GetByIdsAsync (this: ClientExtension, ids: seq<Identity>, [<Optional>] token: CancellationToken) : Task<_ seq> =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Retrieve.getByIdsAsync ids ctx
            match result with
            | Ok ctx ->
                let sequences = ctx.Response
                return sequences |> Seq.map (fun sequence -> sequence.ToSequenceEntity ())
            | Error error -> return raiseError error
        }

    /// <summary>
    /// Retrieves information about multiple Sequences in the same project.
    /// A maximum of 1000 Sequences IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="SequenceId">The ids of the Sequences to get.</param>
    /// <returns>Sequences with given ids.</returns>
    [<Extension>]
    static member GetByIdsAsync (this: ClientExtension, ids: int64 seq, [<Optional>] token: CancellationToken) : Task<_ seq> =
        this.GetByIdsAsync(ids |> Seq.map Identity.Id, token)

    /// <summary>
    /// Retrieves information about multiple Sequences in the same project.
    /// A maximum of 1000 Sequences IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="SequenceId">The ids of the Sequences to get.</param>
    /// <returns>Sequences with given ids.</returns>
    [<Extension>]
    static member GetByIdsAsync (this: ClientExtension, ids: string seq, [<Optional>] token: CancellationToken) : Task<_ seq> =
        this.GetByIdsAsync(ids |> Seq.map Identity.ExternalId, token)


