// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Sequences

open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Tasks.ContextInsensitive
open Oryx
open Thoth.Json.Net

open CogniteSdk

[<RequireQualifiedAccess>]
module Delete =
    [<Literal>]
    let Url = "/sequences/delete"

    type SequencesDeleteRequest = {
        Items: Identity seq
    } with
        member this.Encoder =
            Encode.object [
                yield "items", this.Items |> Seq.map (fun item -> item.Encoder) |> Encode.seq
            ]
    let DataUrl = "/sequences/data/delete"

    type SequencesDataDeleteRequest = {
        Items: SequenceDataDelete seq
    } with
        member this.Encoder =
            Encode.object [
                yield "items", this.Items |> Seq.map (fun item -> item.Encoder) |> Encode.seq
            ]

    let deleteRowsCore (sequences: SequenceDataDelete seq) (fetch: HttpHandler<HttpResponseMessage, HttpResponseMessage, 'a>) =
        let request : SequencesDataDeleteRequest = {
            Items = sequences
        }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource DataUrl
        >=> fetch
        >=> withError decodeError

    let deleteCore (sequences: Identity seq) (fetch: HttpHandler<HttpResponseMessage, HttpResponseMessage, 'a>) =
        let request : SequencesDeleteRequest = {
            Items = sequences
        }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> withError decodeError

    /// <summary>
    /// Delete multiple sequences rows in the same project
    /// </summary>
    /// <param name="sequenceData">The list of sequences rows to delete.</param>
    /// <param name="next">Async handler to use</param>
    let deleteRows (sequenceData: SequenceDataDelete seq) (next: NextFunc<HttpResponseMessage,'a>) =
        deleteRowsCore sequenceData fetch next

    /// <summary>
    /// Delete multiple sequences rows in the same project
    /// </summary>
    /// <param name="sequenceData">The list of Sequences to delete.</param>
    let deleteRowsAsync (sequenceData: SequenceDataDelete seq) : HttpContext -> HttpFuncResult<HttpResponseMessage> =
        deleteRowsCore sequenceData fetch finishEarly

    /// <summary>
    /// Delete multiple Sequences in the same project, along with all their descendants in the Sequence hierarchy if recursive is true.
    /// </summary>
    /// <param name="sequences">The list of Sequences to delete.</param>
    /// <param name="next">Async handler to use</param>
    let delete (sequences: Identity seq) (next: NextFunc<HttpResponseMessage,'a>) =
        deleteCore sequences fetch next

    /// <summary>
    /// Delete multiple Sequences in the same project, along with all their descendants in the Sequence hierarchy if recursive is true.
    /// </summary>
    /// <param name="sequences">The list of Sequences to delete.</param>
    let deleteAsync<'a> (sequences: Identity seq) : HttpContext -> HttpFuncResult<HttpResponseMessage> =
        deleteCore sequences fetch finishEarly

[<Extension>]
type DeleteSequencesExtensions =
    /// <summary>
    /// Delete multiple Sequences in the same project, along with all their descendants in the Sequence hierarchy if recursive is true.
    /// </summary>
    /// <param name="sequences">The list of Sequences to delete.</param>
    [<Extension>]
    static member DeleteAsync(this: ClientExtension, ids: Identity seq, [<Optional>] token: CancellationToken) : Task =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Delete.deleteAsync ids ctx
            match result with
            | Ok _ -> return ()
            | Error error -> return raiseError error
        } :> _

    /// <summary>
    /// Delete multiple Sequences in the same project, along with all their descendants in the Sequence hierarchy if recursive is true.
    /// </summary>
    /// <param name="sequences">The list of Sequences to delete.</param>
    [<Extension>]
    static member DeleteAsync(this: ClientExtension, ids: int64 seq, [<Optional>] token: CancellationToken) : Task =
        this.DeleteAsync(ids |> Seq.map Identity.Id, token)

    /// <summary>
    /// Delete multiple Sequences in the same project, along with all their descendants in the Sequence hierarchy if recursive is true.
    /// </summary>
    /// <param name="sequences">The list of Sequences to delete.</param>
    [<Extension>]
    static member DeleteAsync(this: ClientExtension, ids: string seq, [<Optional>] token: CancellationToken) : Task =
        this.DeleteAsync(ids |> Seq.map Identity.ExternalId, token)
