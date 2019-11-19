// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Sequences

open System.Net.Http
open System.Runtime.InteropServices
open System.Runtime.CompilerServices
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive
open Oryx
open Oryx.ResponseReaders
open Thoth.Json.Net

open CogniteSdk


[<RequireQualifiedAccess>]
module Create =
    [<Literal>]
    let Url = "/sequences"
    let DataUrl = "/sequences/data"

    type Request = {
        Items: SequenceCreateDto seq
    } with
         member this.Encoder =
            Encode.object [
                yield "items", Seq.map (fun (it: SequenceCreateDto) -> it.Encoder) this.Items |> Encode.seq
            ]

    type SequenceResponse = {
        Items: SequenceReadDto seq
    } with
         static member Decoder : Decoder<SequenceResponse> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list SequenceReadDto.Decoder |> Decode.map seq)
            })

    let createRowsCore (sequenceData: SequenceDataCreateDto seq) (fetch: HttpHandler<HttpResponseMessage,HttpResponseMessage,'a>)  =
        let request : SequenceDataItemsCreateDto = { Items = sequenceData }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource DataUrl
        >=> fetch
        >=> withError decodeError

    let createCore (sequences: SequenceCreateDto seq) (fetch: HttpHandler<HttpResponseMessage,HttpResponseMessage,'a>)  =
        let request : Request = { Items = sequences }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> withError decodeError
        >=> json SequenceResponse.Decoder
        >=> map (fun res -> res.Items)

    /// <summary>
    /// Create new rows in sequence in the given project.
    /// </summary>
    /// <param name="sequenceData">The Sequences to create.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>List of created Sequences.</returns>
    let createRows (sequenceData: SequenceDataCreateDto seq) (next: NextFunc<HttpResponseMessage, 'a>) =
        createRowsCore sequenceData fetch next

    /// <summary>
    /// Create new rows in sequence in the given project.
    /// </summary>
    /// <param name="sequenceData">The Sequences to create.</param>
    /// <returns>List of created Sequences.</returns>
    let createRowsAsync (sequenceData: SequenceDataCreateDto seq) : HttpContext -> HttpFuncResult<HttpResponseMessage> =
        createRowsCore sequenceData fetch finishEarly

    /// <summary>
    /// Create new Sequences in the given project.
    /// </summary>
    /// <param name="Sequences">The Sequences to create.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>List of created Sequences.</returns>
    let create (sequences: SequenceCreateDto seq) (next: NextFunc<SequenceReadDto seq, 'a>) =
        createCore sequences fetch next

    /// <summary>
    /// Create new Sequences in the given project.
    /// </summary>
    /// <param name="Sequences">The Sequences to create.</param>
    /// <returns>List of created Sequences.</returns>
    let createAsync (sequences: SequenceCreateDto seq) =
        createCore sequences fetch finishEarly

[<Extension>]
type CreateSequencesExtensions =
    /// <summary>
    /// Create new Sequences in the given project.
    /// </summary>
    /// <param name="Sequences">The Sequences to create.</param>
    /// <returns>List of created Sequences.</returns>
    [<Extension>]
    static member CreateAsync (this: ClientExtension, sequences: SequenceEntity seq, [<Optional>] token: CancellationToken) : Task<SequenceEntity seq> =
        task {
            let sequences' = sequences |> Seq.map SequenceCreateDto.FromSequenceCreateDto
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Create.createAsync sequences' ctx
            match result with
            | Ok ctx ->
                return ctx.Response |> Seq.map (fun sequence -> sequence.ToSequenceEntity ())
            | Error error -> return raiseError error
        }
