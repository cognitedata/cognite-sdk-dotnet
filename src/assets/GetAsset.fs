namespace Cognite.Sdk

open System.Net.Http
open System.Runtime.CompilerServices
open System.Threading.Tasks

open FSharp.Control.Tasks.V2.ContextInsensitive

open Cognite.Sdk
open Cognite.Sdk.Common
open Cognite.Sdk.Api
open Cognite.Sdk.Assets

[<RequireQualifiedAccess>]
module GetAsset =
    [<Literal>]
    let Url = "/assets"

    let getAsset (assetId: int64) (fetch: HttpHandler<HttpResponseMessage, string, 'a>) =
        let decoder = decodeResponse AssetReadDto.Decoder id
        let url = Url + sprintf "/%d" assetId

        GET
        >=> setVersion V10
        >=> setResource url
        >=> fetch
        >=> decoder

[<AutoOpen>]
module GetAssetApi =
    /// **Description**
    ///
    /// Retrieves information about an asset in a certain project given an asset id.
    ///
    /// **Parameters**
    ///   * `assetId` - The id of the attet to retrieve.
    ///   * `ctx` - The request HTTP context to use.
    ///
    /// **Output Type**
    ///   * `Async<Result<Response,exn>>`
    ///
    let getAsset (assetId: int64) (next: NextHandler<AssetReadDto,'a>) : HttpContext -> Async<Context<'a>> =
        GetAsset.getAsset assetId fetch next

    let getAssetAsync (assetId: int64) : HttpContext -> Async<Context<AssetReadDto>> =
        GetAsset.getAsset assetId fetch Async.single

[<Extension>]
type GetAssetExtensions =
    /// <summary>
    /// Retrieves information about an asset in a certain project given an asset id.
    /// </summary>
    /// <param name="assetId">The id of the asset to get.</param>
    /// <returns>Asset with the given id.</returns>
    [<Extension>]
    static member GetAssetAsync (this: Client, assetId: int64) : Task<AssetReadDto> =
        task {
            let! ctx = getAssetAsync assetId this.Ctx
            match ctx.Result with
            | Ok asset ->
                return asset
            | Error error ->
                return raise (Error.error2Exception error)
        }