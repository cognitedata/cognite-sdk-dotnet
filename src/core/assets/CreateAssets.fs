// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite.Assets

open System.Net.Http
open System.Runtime.InteropServices
open System.Runtime.CompilerServices
open System.Threading
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive
open Oryx
open Oryx.ResponseReaders
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

    let pipeline fetch =
        POST
        >=> setVersion V10
        >=> setResource Url
        >=> fetch
        >=> withError decodeError
        >=> json AssetResponse.Decoder
        >=> map (fun res -> res.Items)

    let pipeline' next ctx = pipeline fetch next ctx

    let createCore (fetch: HttpHandler<HttpResponseMessage,HttpResponseMessage,'a>) (assets: AssetWriteDto seq)  =
        let request : Request = { Items = assets }

        setContent (Content.JsonValue request.Encoder)
        >=> pipeline fetch

    /// <summary>
    /// Create new assets in the given project.
    /// </summary>
    /// <param name="assets">The assets to create.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>List of created assets.</returns>
    let create (assets: AssetWriteDto seq) =
        let request : Request = { Items = assets }

        setContent (Content.JsonValue request.Encoder)
        >=> pipeline'

    let createCoreFetch = createCore fetch
    /// <summary>
    /// Create new assets in the given project.
    /// </summary>
    /// <param name="assets">The assets to create.</param>
    /// <returns>List of created assets.</returns>
    let createAsync (assets: AssetWriteDto seq) =
        createCoreFetch assets finishEarly
