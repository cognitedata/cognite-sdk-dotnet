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
open Thoth.Json.Net

open CogniteSdk


[<RequireQualifiedAccess>]
module Create =
    [<Literal>]
    let Url = "/files"

    let createCore (file: FileWriteDto) (fetch: HttpHandler<HttpResponseMessage,HttpResponseMessage,'a>)  =
        let decodeResponse = Decode.decodeResponse FileCreatedDto.Decoder id
        let request : FileWriteDto = file

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> decodeResponse

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
        createCore file fetch Task.FromResult

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
            let! ctx = Create.createAsync file' this.Ctx
            match ctx.Result with
            | Ok response ->
                return response.ToFileEntity ()
            | Error error ->
                return raise (error.ToException ())
        }
