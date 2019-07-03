namespace Cognite.Sdk

open System
open System.Collections.Generic
open System.Net.Http
open System.Runtime.CompilerServices
open System.Threading.Tasks

open FSharp.Control.Tasks.V2
open Thoth.Json.Net

open Cognite.Sdk
open Cognite.Sdk.Api
open Cognite.Sdk.Common
open Cognite.Sdk.Assets

[<RequireQualifiedAccess>]
module CreateAssets =
    [<Literal>]
    let Url = "/assets"

    type AssetsCreateRequest = {
        Items: AssetCreateDto seq
    } with
         member this.Encoder =
            Encode.object [
                yield "items", Seq.map (fun (it: AssetCreateDto) -> it.Encoder) this.Items |> Encode.seq
            ]

    type AssetResponse = {
        Items: AssetReadDto seq
        NextCursor : string option
    } with
         static member Decoder : Decoder<AssetResponse> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list AssetReadDto.Decoder |> Decode.map seq)
                NextCursor = get.Optional.Field "nextCursor" Decode.string
            })

    let createAssets (assets: AssetCreateDto seq) (fetch: HttpHandler<HttpResponseMessage,string,'a>)  =
        let decoder = decodeResponse AssetResponse.Decoder (fun res -> res.Items)
        let request : AssetsCreateRequest = { Items = assets }
        let body = Encode.stringify  request.Encoder

        POST
        >=> setVersion V10
        >=> setBody body
        >=> setResource Url
        >=> fetch
        >=> decoder

[<AutoOpen>]
module CreateAssetsApi =
    /// **Description**
    ///
    /// Creates new assets in the given project
    ///
    /// **Parameters**
    ///   * `assets` - The list of assets to create.
    ///   * `ctx` - The request HTTP context to use.
    ///
    /// **Output Type**
    ///   * `Async<Result<Response,exn>>`
    ///
    let createAssets (assets: AssetCreateDto seq) (next: NextHandler<AssetReadDto seq,'a>) : HttpContext -> Async<Context<'a>> =
        CreateAssets.createAssets assets fetch next

    let createAssetsAsync (assets: AssetCreateDto seq) : HttpContext -> Async<Context<AssetReadDto seq>> =
        CreateAssets.createAssets assets fetch Async.single

[<Extension>]
type CreateAssetsExtensions =
        /// <summary>
    /// Create assets.
    /// </summary>
    /// <param name="assets">The assets to create.</param>
    /// <returns>List of created assets.</returns>
    [<Extension>]
    static member CreateAssetsAsync (this: Client, assets: Asset seq) : Task<AssetReadDto seq> =
        task {
            let assets' = assets |> Seq.map AssetCreateDto.Create
            let! ctx = createAssetsAsync assets' this.Ctx
            match ctx.Result with
            | Ok response ->
                return response
            | Error error ->
               return raise (Error.error2Exception error)
        }
