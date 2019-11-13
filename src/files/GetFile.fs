// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Files

open System.IO
open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open Oryx
open Oryx.Decode
open CogniteSdk
open FSharp.Control.Tasks.V2.ContextInsensitive

[<RequireQualifiedAccess>]
module Entity =
    [<Literal>]
    let Url = "/files"

    let getCore (fileId: int64) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let url = Url + sprintf "/%d" fileId

        GET
        >=> setVersion V10
        >=> setResource url
        >=> fetch
        >=> withError decodeError
        >=> json FileReadDto.Decoder

    /// <summary>
    /// Retrieves information about an file given an file id. Expects a next continuation handler.
    /// </summary>
    /// <param name="assetId">The id of the file to get.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>File with the given id.</returns>
    let get (fileId: int64) (next: NextFunc<FileReadDto,'a>) : HttpContext -> HttpFuncResult<'a> =
        getCore fileId fetch next

    /// <summary>
    /// Retrieves information about an file given an file id.
    /// </summary>
    /// <param name="assetId">The id of the file to get.</param>
    /// <returns>File with the given id.</returns>
    let getAsync (fileId: int64) : HttpContext -> HttpFuncResult<FileReadDto> =
        getCore fileId fetch finishEarly

[<Extension>]
type GetFileClientExtensions =
    /// <summary>
    /// Retrieves information about an file given an file id.
    /// </summary>
    /// <param name="FileId">The id of the file to get.</param>
    /// <param name="token">Propagates notification that operations should be canceled</param>
    /// <returns>File with the given id.</returns>
    [<Extension>]
    static member GetAsync (this: ClientExtension, fileId: int64, [<Optional>] token: CancellationToken) : Task<FileReadDto> =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Entity.getAsync fileId ctx
            match result with
            | Ok ctx ->
                return ctx.Response
            | Error (ApiError error) -> return raise (error.ToException ())
            | Error (Panic error) -> return raise error
        }
