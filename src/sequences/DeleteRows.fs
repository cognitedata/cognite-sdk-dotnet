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

        static member FromEntity (entity: SequenceDataDeleteEntity) =
            {
                Rows = entity.Rows
                Id = entity.Id
            }

    type SequencesDataDeleteRequest = {
        Items: SequenceDataDelete seq
    } with
        member this.Encoder =
            Encode.object [
                yield "items", this.Items |> Seq.map (fun item -> item.Encoder) |> Encode.seq
            ]

    let deleteRowsCore (rowData: SequenceDataDelete seq) (fetch: HttpHandler<HttpResponseMessage, HttpResponseMessage, 'a>) =
        let request : SequencesDataDeleteRequest = {
            Items = rowData
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
    let deleteRows (rowData: SequenceDataDelete seq) (next: NextFunc<HttpResponseMessage,'a>) =
        deleteRowsCore rowData fetch next

    /// <summary>
    /// Delete multiple sequences rows in the same project
    /// </summary>
    /// <param name="sequenceData">The list of Sequences and rows to delete.</param>
    let deleteRowsAsync (rowData: SequenceDataDelete seq) : HttpContext -> HttpFuncResult<HttpResponseMessage> =
        deleteRowsCore rowData fetch finishEarly


[<Extension>]
type DeleteRowsExtensions =
    /// <summary>
    /// Delete multiple Rows in the same sequence.
    /// </summary>
    /// <param name="sequences">The list of rows to delete.</param>
    [<Extension>]
    static member DeleteRowsAsync(this: ClientExtension, rows: SequenceDataDeleteEntity seq, [<Optional>] token: CancellationToken) : Task =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let rowData = rows |> Seq.map Delete.SequenceDataDelete.FromEntity
            let! result = Delete.deleteRowsAsync rowData ctx
            match result with
            | Ok _ -> return ()
            | Error error -> return raiseError error
        } :> _
