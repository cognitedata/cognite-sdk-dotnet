// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Sequences.Rows

open System.IO
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
open CogniteSdk.Sequences

type SequenceDataReadDto = {
    Id: int64
    ExternalId: string option
    Columns: ColumnInfoReadDto seq
    Rows: RowDto seq
    NextCursor: string option
} with
    member this.ToSequenceDataReadEntity() =
        let externalId = Option.defaultValue Unchecked.defaultof<string> this.ExternalId
        let columns = Seq.map (fun (c: ColumnInfoReadDto) -> c.ToColumnInfoReadEntity()) this.Columns
        let rows = Seq.map (fun (r: RowDto) -> r.ToEntity()) this.Rows
        let nextCursor = Option.defaultValue Unchecked.defaultof<string> this.NextCursor

        SequenceDataReadEntity(this.Id, externalId, columns, rows, nextCursor)
    static member Decoder : Decoder<SequenceDataReadDto> =
            Decode.object (fun get ->
                {
                    Id = get.Required.Field "id" Decode.int64
                    ExternalId = get.Optional.Field "externalId" Decode.string
                    Columns = get.Required.Field "columns" (Decode.list ColumnInfoReadDto.Decoder)
                    Rows = get.Required.Field "rows" (Decode.list RowDto.Decoder)
                    NextCursor = get.Optional.Field "nextCursor" Decode.string
                }
        )

[<RequireQualifiedAccess>]
module Items =
    [<Literal>]
    let DataUrl = "/sequences/data/list"

    let private addIdentity (identity: Identity) (queries: RowQuery seq) =
        RowQuery.Id identity
        |> Seq.singleton
        |> Seq.append queries

    type DataRequest = {
        Options : RowQuery seq
    } with
        member this.Encoder =
            Encode.object [
                yield! this.Options |> Seq.map RowQuery.Render
            ]

    let listRowsCore (identity: Identity) (options: RowQuery seq) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let request : DataRequest = {
            Options = addIdentity identity options
        }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource DataUrl
        >=> fetch
        >=> withError decodeError
        >=> json SequenceDataReadDto.Decoder

    /// <summary>
    /// Retrieves Sequence data with columns and rows matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="options">Optional limit and cursor</param>
    /// <param name="next">Async handler to use</param>
    /// <returns>Sequences data with rows and columns matching given filters and optional cursor</returns>
    let listRows (identity: Identity) (options: RowQuery seq) (next: NextFunc<SequenceDataReadDto,'a>) : HttpContext -> HttpFuncResult<'a> =
        listRowsCore identity options fetch next

    /// <summary>
    /// Retrieves Sequence data with columns and rows matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="options">Optional limit and cursor</param>
    /// <returns>Sequences data with rows and columns matching given filters and optional cursor</returns>
    let listRowsAsync (identity: Identity) (options: RowQuery seq) : HttpContext -> HttpFuncResult<SequenceDataReadDto> =
        listRowsCore identity options fetch finishEarly


[<Extension>]
type ListRowsExtensions =
    /// <summary>
    /// Retrieves Sequence data with columns and rows matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="options">Optional limit and cursor</param>
    /// <returns>Sequences data with rows and columns matching given filters and optional cursor</returns>
    [<Extension>]
    static member ListRowsAsync (this: ClientExtension, identity: Identity, options: RowQuery seq, [<Optional>] token: CancellationToken) : Task<SequenceDataReadEntity> =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Items.listRowsAsync identity options ctx
            match result with
            | Ok ctx ->
                return ctx.Response.ToSequenceDataReadEntity()
            | Error error -> return raiseError error
        }

    /// <summary>
    /// Retrieves Sequence data with columns and rows matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <returns>Sequences data with rows and columns matching given filters and optional cursor</returns>
    [<Extension>]
    static member ListRowsAsync (this: ClientExtension, identity: Identity, [<Optional>] token: CancellationToken) : Task<SequenceDataReadEntity> =
        this.ListRowsAsync(identity, Seq.empty, token)
