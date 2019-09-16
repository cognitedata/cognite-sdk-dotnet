// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk.Assets

open System.IO
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
    let Url = "/assets"

    type Request = {
        Items: AssetWriteDto seq
    } with
         member this.Encoder =
            Encode.object [
                yield "items", Seq.map (fun (it: AssetWriteDto) -> it.Encoder) this.Items |> Encode.seq
            ]

    type AssetResponse = {
        Items: AssetReadDto seq
    } with
         static member Decoder : Decoder<AssetResponse> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list AssetReadDto.Decoder |> Decode.map seq)
            })

    let createCore (assets: AssetWriteDto seq) (fetch: HttpHandler<HttpResponseMessage,HttpResponseMessage,'a>)  =
        let decodeResponse = Decode.decodeResponse AssetResponse.Decoder (fun res -> res.Items)
        let request : Request = { Items = assets }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> decodeResponse

    /// <summary>
    /// Create new assets in the given project.
    /// </summary>
    /// <param name="assets">The assets to create.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>List of created assets.</returns>
    let create (assets: AssetWriteDto seq) (next: NextFunc<AssetReadDto seq, 'a>) =
        createCore assets fetch next

    /// <summary>
    /// Create new assets in the given project.
    /// </summary>
    /// <param name="assets">The assets to create.</param>
    /// <returns>List of created assets.</returns>
    let createAsync (assets: AssetWriteDto seq) =
        createCore assets fetch Task.FromResult

[<Extension>]
type CreateAssetsExtensions =
    /// <summary>
    /// Create new assets in the given project.
    /// </summary>
    /// <param name="assets">The assets to create.</param>
    /// <returns>List of created assets.</returns>
    [<Extension>]
    static member CreateAsync (this: ClientExtension, assets: AssetEntity seq, [<Optional>] token: CancellationToken) : Task<AssetEntity seq> =
        task {
            let assets' = assets |> Seq.map AssetWriteDto.FromAssetEntity
            let! ctx = Create.createAsync assets' this.Ctx
            match ctx.Result with
            | Ok response ->
                return response |> Seq.map (fun asset -> asset.ToAssetEntity ())
            | Error error ->
                return raise (error.ToException ())
        }
