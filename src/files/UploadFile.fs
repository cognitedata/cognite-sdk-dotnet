// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Files

open System.Net.Http
open System.Runtime.InteropServices
open System.Runtime.CompilerServices
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive
open Oryx

open CogniteSdk


[<RequireQualifiedAccess>]
module Create =
    [<Literal>]
    let Url = "/files"

    let createCore (file: FileWriteDto) (fetch: HttpHandler<HttpResponseMessage,HttpResponseMessage,'a>)  =
        let request : FileWriteDto = file

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> withError decodeError
        >=> json FileCreatedDto.Decoder

    /// <summary>
    /// Create new file in the given project.
    /// </summary>
    /// <param name="file">The file to create.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>File result with upload url</returns>
    let create (file: FileWriteDto) (next: NextFunc<FileCreatedDto, 'a>) =
        createCore file fetch next

    /// <summary>
    /// Create new file in the given project.
    /// </summary>
    /// <param name="file">The file to create.</param>
    /// <returns>File result with upload url</returns>
    let createAsync (file: FileWriteDto) =
        createCore file fetch finishEarly

[<Extension>]
type CreateFilesExtensions =
    /// <summary>
    /// Create new file in the given project.
    /// </summary>
    /// <param name="file">The file to create.</param>
    /// <returns>File result with upload url</returns>
    [<Extension>]
    static member CreateAsync (this: ClientExtension, file: FileEntity, [<Optional>] token: CancellationToken) : Task<FileEntity> =
        task {
            let file' = FileWriteDto.FromFileEntity file
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Create.createAsync file' ctx
            match result with
            | Ok ctx ->
                return ctx.Response.ToFileEntity ()
            | Error error -> return raiseError error
        }
