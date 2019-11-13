// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Files

open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive
open Oryx
open Thoth.Json.Net

open CogniteSdk

type FileQuery =
    private
    | CaseLimit of int
    | CaseCursor of string

    /// Max number of results to return
    static member Limit limit =
        if limit > MaxLimitSize || limit < 1 then
            failwith "Limit must be set to 1000 or less"
        CaseLimit limit
    /// Cursor return from previous request
    static member Cursor cursor = CaseCursor cursor

    static member Render (this: FileQuery) =
        match this with
        | CaseLimit limit -> "limit", Encode.int limit
        | CaseCursor cursor -> "cursor", Encode.string cursor

[<RequireQualifiedAccess>]
module Items =
    [<Literal>]
    let Url = "/files/list"

    type Request = {
        Filters : FileFilter seq
        Options : FileQuery seq
    } with
        member this.Encoder =
            Encode.object [
                yield "filter", Encode.object [
                    yield! this.Filters |> Seq.map FileFilter.Render
                ]
                yield! this.Options |> Seq.map FileQuery.Render
            ]

    let listCore (options: FileQuery seq) (filters: FileFilter seq)(fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let request : Request = {
            Filters = filters
            Options = options
        }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> withError decodeError
        >=> json FileItemsReadDto.Decoder

    /// <summary>
    /// Retrieves list of files matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="options">Optional limit and cursor</param>
    /// <param name="filters">Search filters</param>
    /// <param name="next">Async handler to use</param>
    /// <returns>List of files matching given filters and optional cursor</returns>
    let list (options: FileQuery seq) (filters: FileFilter seq) (next: NextFunc<FileItemsReadDto,'a>)
        : HttpContext -> HttpFuncResult<'a> =
            listCore options filters fetch next

    /// <summary>
    /// Retrieves list of files matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="options">Optional limit and cursor</param>
    /// <param name="filters">Search filters</param>
    /// <returns>List of files matching given filters and optional cursor</returns>
    let listAsync (options: FileQuery seq) (filters: FileFilter seq)
        : HttpContext -> HttpFuncResult<FileItemsReadDto> =
            listCore options filters fetch finishEarly


[<Extension>]
type ListFilesExtensions =
    /// <summary>
    /// Retrieves list of files matching query, filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="options">Optional limit and cursor</param>
    /// <param name="filters">Search filters</param>
    /// <returns>List of files matching given filters and optional cursor</returns>
    [<Extension>]
    static member ListAsync (this: ClientExtension, options: FileQuery seq, filters: FileFilter seq, [<Optional>] token: CancellationToken) : Task<FileItems> =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Items.listAsync options filters ctx
            match result with
            | Ok ctx ->
                let files = ctx.Response
                let cursor = if files.NextCursor.IsSome then files.NextCursor.Value else Unchecked.defaultof<string>
                let items : FileItems = {
                        NextCursor = cursor
                        Items = files.Items |> Seq.map (fun file -> file.ToFileEntity ())
                    }
                return items
            | Error error -> return raiseError error
        }

    /// <summary>
    /// Retrieves list of files matching filter.
    /// </summary>
    /// <param name="options">Optional limit and cursor</param>
    /// <param name="filters">Search filters</param>
    /// <returns>List of files matching given filters and optional cursor</returns>
    [<Extension>]
    static member ListAsync (this: ClientExtension, filters: FileFilter seq, [<Optional>] token: CancellationToken) : Task<FileItems> =
        let query = ResizeArray<FileQuery>()
        this.ListAsync(query, filters)
