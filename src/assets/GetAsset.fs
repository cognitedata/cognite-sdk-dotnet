// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Assets

open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive
open Oryx

open CogniteSdk.Assets
open CogniteSdk


[<RequireQualifiedAccess>]
module Entity =
    [<Literal>]
    let Url = "/assets"

    let getCore (assetId: int64) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let url = Url + sprintf "/%d" assetId

        GET
        >=> setVersion V10
        >=> setResource url
        >=> fetch
        >=> withError decodeError
        >=> json AssetReadDto.Decoder

    /// <summary>
    /// Retrieves information about an asset given an asset id. Expects a next continuation handler.
    /// </summary>
    /// <param name="assetId">The id of the asset to get.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>Asset with the given id.</returns>
    let get (assetId: int64) (next: NextFunc<AssetReadDto,'a>) : HttpContext -> HttpFuncResult<'a> =
        getCore assetId fetch next

    /// <summary>
    /// Retrieves information about an asset given an asset id.
    /// </summary>
    /// <param name="assetId">The id of the asset to get.</param>
    /// <returns>Asset with the given id.</returns>
    let getAsync (assetId: int64) : HttpContext -> HttpFuncResult<AssetReadDto> =
        getCore assetId fetch finishEarly

[<Extension>]
type GetAssetClientExtensions =
    /// <summary>
    /// Retrieves information about an asset given an asset id.
    /// </summary>
    /// <param name="assetId">The id of the asset to get.</param>
    /// <returns>Asset with the given id.</returns>
    [<Extension>]
    static member GetAsync (this: ClientExtension, assetId: int64, [<Optional>] token: CancellationToken) : Task<AssetReadDto> =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! result = Entity.getAsync assetId ctx
            match result with
            | Ok ctx -> return ctx.Response
            | Error (ApiError error) -> return raise (error.ToException ())
            | Error (Panic error) -> return raise error
        }
