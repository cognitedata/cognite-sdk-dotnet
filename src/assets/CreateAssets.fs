namespace Fusion

open System.IO
open System.Net.Http
open System.Runtime.CompilerServices
open System.Threading.Tasks

open FSharp.Control.Tasks.V2
open Thoth.Json.Net

open Fusion
open Fusion.Api
open Fusion.Common
open Fusion.Assets


[<RequireQualifiedAccess>]
module CreateAssets =
    [<Literal>]
    let Url = "/assets"

    type AssetsCreateRequest = {
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

    let createAssets (assets: AssetWriteDto seq) (fetch: HttpHandler<HttpResponseMessage,Stream,'a>)  =
        let decoder = decodeResponse AssetResponse.Decoder (fun res -> res.Items)
        let request : AssetsCreateRequest = { Items = assets }

        POST
        >=> setVersion V10
        >=> setBody request.Encoder
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
    let createAssets (assets: AssetWriteDto seq) (next: NextHandler<AssetReadDto seq,'a>) : HttpContext -> Async<Context<'a>> =
        CreateAssets.createAssets assets fetch next

    let createAssetsAsync (assets: AssetWriteDto seq) : HttpContext -> Async<Context<AssetReadDto seq>> =
        CreateAssets.createAssets assets fetch Async.single

[<Extension>]
type CreateAssetsExtensions =
    /// <summary>
    /// Create assets.
    /// </summary>
    /// <param name="assets">The assets to create.</param>
    /// <returns>List of created assets.</returns>
    [<Extension>]
    static member CreateAssetsAsync (this: Client, assets: AssetWritePoco seq) : Task<AssetReadPoco seq> =
        task {
            let assets' = assets |> Seq.map AssetWriteDto.FromPoco
            let! ctx = createAssetsAsync assets' this.Ctx
            match ctx.Result with
            | Ok response ->
                return response |> Seq.map (fun asset -> asset.ToPoco ())
            | Error error ->
                let err = error2Exception error
                return raise err
        }
