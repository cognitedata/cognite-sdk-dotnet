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

[<AutoOpen>]
module Retrieve =
    [<Literal>]
    let Url = "/files/byids"
    type FileRequest = {
        Items: Identity seq
    } with
        member this.Encoder  =
            Encode.object [
                yield "items", this.Items |> Seq.map(fun id -> id.Encoder) |> Encode.seq
            ]

    type FileResponse = {
        Items: FileReadDto seq
    } with
         static member Decoder : Decoder<FileResponse> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list FileReadDto.Decoder |> Decode.map seq)
            })

    let getByIdsCore (ids: Identity seq) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let request : FileRequest = { Items = ids }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> withError decodeError
        >=> json FileResponse.Decoder
        >=> map (fun response -> response.Items)


    /// <summary>
    /// Retrieves information about multiple files in the same project.
    /// A maximum of 1000 files IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="FileIds">The ids of the files to get.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>Files with given ids.</returns>
    let getByIds (ids: Identity seq) (next: NextFunc<FileReadDto seq,'a>) : HttpContext -> HttpFuncResult<'a> =
        getByIdsCore ids fetch next

    /// <summary>
    /// Retrieves information about multiple files in the same project.
    /// A maximum of 1000 files IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="FileIds">The ids of the files to get.</param>
    /// <returns>Files with given ids.</returns>
    let getByIdsAsync (ids: Identity seq) =
        getByIdsCore ids fetch finishEarly


[<Extension>]
type GetFilesByIdsClientExtensions =
    /// <summary>
    /// Retrieves information about multiple files in the same project.
    /// A maximum of 1000 files IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="FileIds">The ids of the files to get.</param>
    /// <returns>Files with given ids.</returns>
    [<Extension>]
    static member GetByIdsAsync (this: ClientExtension, ids: seq<Identity>, [<Optional>] token: CancellationToken) : Task<_ seq> =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Retrieve.getByIdsAsync ids ctx
            match result with
            | Ok ctx ->
                let files = ctx.Response
                return files |> Seq.map (fun file -> file.ToFileEntity ())
            | Error error -> return raiseError error
        }

    /// <summary>
    /// Retrieves information about multiple files in the same project.
    /// A maximum of 1000 files IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="FileIds">The ids of the files to get.</param>
    /// <returns>Files with given ids.</returns>
    [<Extension>]
    static member GetByIdsAsync (this: ClientExtension, ids: seq<int64>, [<Optional>] token: CancellationToken) : Task<_ seq> =
        this.GetByIdsAsync(ids |> Seq.map Identity.Id, token)

    /// <summary>
    /// Retrieves information about multiple files in the same project.
    /// A maximum of 1000 files IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="FileIds">The ids of the files to get.</param>
    /// <returns>Files with given ids.</returns>
    [<Extension>]
    static member GetByIdsAsync (this: ClientExtension, ids: seq<string>, [<Optional>] token: CancellationToken) : Task<_ seq> =
        this.GetByIdsAsync(ids |> Seq.map Identity.ExternalId, token)
