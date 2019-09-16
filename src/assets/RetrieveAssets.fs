// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Assets

open System.IO
open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open Oryx
open Thoth.Json.Net

open CogniteSdk
open FSharp.Control.Tasks.V2.ContextInsensitive

[<AutoOpen>]
module Retrieve =
    [<Literal>]
    let Url = "/assets/byids"
    type AssetRequest = {
        Items: Identity seq
    } with
        member this.Encoder  =
            Encode.object [
                yield "items", this.Items |> Seq.map(fun id -> id.Encoder) |> Encode.seq
            ]

    type AssetResponse = {
        Items: AssetReadDto seq
    } with
         static member Decoder : Decoder<AssetResponse> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list AssetReadDto.Decoder |> Decode.map seq)
            })

    let getByIdsCore (ids: Identity seq) (fetch: HttpHandler<HttpResponseMessage, 'a>) =
        let decodeResponse = Decode.decodeResponse AssetResponse.Decoder (fun response -> response.Items)
        let request : AssetRequest = { Items = ids }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> decodeResponse

    /// <summary>
    /// Retrieves information about multiple assets in the same project.
    /// A maximum of 1000 assets IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="assetId">The ids of the assets to get.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>Assets with given ids.</returns>
    let getByIds (ids: Identity seq) (next: NextFunc<AssetReadDto seq,'a>) : HttpContext -> Task<Context<'a>> =
        getByIdsCore ids fetch next

    /// <summary>
    /// Retrieves information about multiple assets in the same project.
    /// A maximum of 1000 assets IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="assetId">The ids of the assets to get.</param>
    /// <returns>Assets with given ids.</returns>
    let getByIdsAsync (ids: Identity seq) =
        getByIdsCore ids fetch Task.FromResult


[<Extension>]
type GetAssetsByIdsClientExtensions =
    /// <summary>
    /// Retrieves information about multiple assets in the same project.
    /// A maximum of 1000 assets IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="assetId">The ids of the assets to get.</param>
    /// <returns>Assets with given ids.</returns>
    [<Extension>]
    static member GetByIdsAsync (this: ClientExtension, ids: seq<Identity>, [<Optional>] token: CancellationToken) : Task<_ seq> =
        task {
            let ctx = this.Ctx |> Context.setCancellationToken token
            let! ctx = Retrieve.getByIdsAsync ids this.Ctx
            match ctx.Result with
            | Ok assets ->
                return assets |> Seq.map (fun asset -> asset.ToAssetEntity ())
            | Error error ->
                return raise (error.ToException ())
        }

    /// <summary>
    /// Retrieves information about multiple assets in the same project.
    /// A maximum of 1000 assets IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="assetId">The ids of the assets to get.</param>
    /// <returns>Assets with given ids.</returns>
    [<Extension>]
    static member GetByIdsAsync (this: ClientExtension, ids: int64 seq, [<Optional>] token: CancellationToken) : Task<_ seq> =
        this.GetByIdsAsync(ids |> Seq.map Identity.Id, token)

    /// <summary>
    /// Retrieves information about multiple assets in the same project.
    /// A maximum of 1000 assets IDs may be listed per request and all
    /// of them must be unique.
    /// </summary>
    /// <param name="assetId">The ids of the assets to get.</param>
    /// <returns>Assets with given ids.</returns>
    [<Extension>]
    static member GetByIdsAsync (this: ClientExtension, ids: string seq, [<Optional>] token: CancellationToken) : Task<_ seq> =
        this.GetByIdsAsync(ids |> Seq.map Identity.ExternalId, token)


