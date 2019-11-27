// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Sequences.Rows

open System.Net.Http
open System.Runtime.InteropServices
open System.Runtime.CompilerServices
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive
open Oryx
open Thoth.Json.Net

open CogniteSdk
open CogniteSdk.Sequences

[<RequireQualifiedAccess>]
module Insert =
    [<Literal>]
    let DataUrl = "/sequences/data"

    type SequenceDataCreateDto = {
        Columns: string seq
        Rows: RowDto seq
        Id: Identity
    } with
        member this.Encoder =
            Encode.object [
                yield "columns", Encode.seq (Seq.map Encode.string this.Columns)
                yield "rows", Encode.seq (Seq.map (fun (row: RowDto) -> row.Encoder) this.Rows)
                yield this.Id.Render
            ]

        static member FromEntity (entity: SequenceDataWriteEntity) =
            {
                Columns = entity.Columns
                Rows = entity.Rows |> Seq.map RowDto.FromEntity
                Id = entity.Id
            }

    type SequenceDataItemsCreateDto = {
        Items: SequenceDataCreateDto seq
    } with
        member this.Encoder =
            Encode.object [
                yield "items", Encode.seq (Seq.map (fun (s: SequenceDataCreateDto) -> s.Encoder) this.Items)
            ]


    let insertRowsCore (sequenceData: SequenceDataCreateDto seq) (fetch: HttpHandler<HttpResponseMessage,HttpResponseMessage,'a>)  =
        let request : SequenceDataItemsCreateDto = { Items = sequenceData }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource DataUrl
        >=> fetch
        >=> withError decodeError

    /// <summary>
    /// Create new rows in sequence in the given project.
    /// </summary>
    /// <param name="sequenceData">The Sequences to create.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>List of created Sequences.</returns>
    let insertRows (sequenceData: SequenceDataCreateDto seq) (next: NextFunc<HttpResponseMessage, 'a>) =
        insertRowsCore sequenceData fetch next

    /// <summary>
    /// Create new rows in sequence in the given project.
    /// </summary>
    /// <param name="sequenceData">The Sequences to create.</param>
    /// <returns>List of created Sequences.</returns>
    let insertRowsAsync (sequenceData: SequenceDataCreateDto seq) : HttpContext -> HttpFuncResult<HttpResponseMessage> =
        insertRowsCore sequenceData fetch finishEarly


[<Extension>]
type InsertRowsExtensions =
    [<Extension>]
    static member InsertRowsAsync (this: ClientExtension, rows: SequenceDataWriteEntity seq, [<Optional>] token: CancellationToken) : Task =
        task {
            let sequences' = rows |> Seq.map Insert.SequenceDataCreateDto.FromEntity
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Insert.insertRowsAsync sequences' ctx
            match result with
            | Ok ctx -> return ()
            | Error error -> return raiseError error
        } :> _
