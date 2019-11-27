// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Sequences.Rows

open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Tasks.ContextInsensitive
open Oryx
open Thoth.Json.Net

open CogniteSdk
open CogniteSdk.Sequences


[<RequireQualifiedAccess>]
module Delete =
    [<Literal>]
    let DataUrl = "/sequences/data/delete"

    type SequenceDataDelete = {
        Rows: int64 seq
        Id: Identity
    } with
        member this.Encoder =
            Encode.object [
                yield "rows", Encode.seq (Seq.map Encode.int64 this.Rows)
                yield this.Id.Render
            ]

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


[<Extension>]
type DeleteRowsExtensions =
    /// <summary>
    /// Delete multiple Sequences in the same project, along with all their descendants in the Sequence hierarchy if recursive is true.
    /// </summary>
    /// <param name="sequences">The list of Sequences to delete.</param>
    [<Extension>]
    static member DeleteRowsAsync(this: ClientExtension, ids: Identity seq, [<Optional>] token: CancellationToken) : Task =
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
    static member DeleteRowsAsync(this: ClientExtension, ids: int64 seq, [<Optional>] token: CancellationToken) : Task =
        this.DeleteAsync(ids |> Seq.map Identity.Id, token)

    /// <summary>
    /// Delete multiple Sequences in the same project, along with all their descendants in the Sequence hierarchy if recursive is true.
    /// </summary>
    /// <param name="sequences">The list of Sequences to delete.</param>
    [<Extension>]
    static member DeleteRowsAsync(this: ClientExtension, ids: string seq, [<Optional>] token: CancellationToken) : Task =
        this.DeleteAsync(ids |> Seq.map Identity.ExternalId, token)
