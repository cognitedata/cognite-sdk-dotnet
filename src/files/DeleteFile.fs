// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Files

open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive
open Thoth.Json.Net
open Oryx

open CogniteSdk
open CogniteSdk.Files

[<RequireQualifiedAccess>]
module Delete =
    [<Literal>]
    let Url = "/files/delete"

    type FilesDeleteRequest = {
        Items: Identity seq
    } with
        member this.Encoder =
            Encode.object [
                yield "items", this.Items |> Seq.map (fun item -> item.Encoder) |> Encode.seq
            ]

    let deleteCore (files: Identity seq) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let request : FilesDeleteRequest = {
            Items = files
        }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> withError decodeError

    /// <summary>
    /// Delete multiple files in the same project
    /// </summary>
    /// <param name="Files">The list of files to delete.</param>
    /// <param name="next">Async handler to use</param>
    let delete (files: Identity seq) (next: NextFunc<HttpResponseMessage,'a>) =
        deleteCore files fetch next

    /// <summary>
    /// Delete multiple files in the same project
    /// </summary>
    /// <param name="files">The list of files to delete.</param>
    let deleteAsync<'a> (files: Identity seq) : HttpContext -> HttpFuncResult<HttpResponseMessage> =
        deleteCore files fetch finishEarly

[<Extension>]
type DeleteFileExtensions =
    /// <summary>
    /// Delete multiple files in the same project
    /// </summary>
    /// <param name="ids">The list of files to delete</param>
    /// <param name="token">Propagates notification that operations should be canceled</param>
    [<Extension>]
    static member DeleteAsync(this: ClientExtension, ids: Identity seq, [<Optional>] token: CancellationToken) : Task =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Delete.deleteAsync (ids) ctx
            match result with
            | Ok _ -> return ()
            | Error (ApiError error) -> return raise (error.ToException ())
            | Error (Panic error) -> return raise error
        } :> Task

