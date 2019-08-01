namespace Fusion

open System.IO
open System.Net.Http
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading.Tasks

open Fusion
open Fusion.Common
open Fusion.Api
open Fusion.Assets
open System.Threading

[<RequireQualifiedAccess>]
module GetAsset =
    [<Literal>]
    let Url = "/assets"

    let getAsset (assetId: int64) (fetch: HttpHandler<HttpResponseMessage, Stream, 'a>) =
        let decoder = decodeResponse AssetReadDto.Decoder id
        let url = Url + sprintf "/%d" assetId

        GET
        >=> setVersion V10
        >=> setResource url
        >=> fetch
        >=> decoder

[<AutoOpen>]
module GetAssetApi =
    /// <summary>
    /// Retrieves information about an asset given an asset id.
    /// </summary>
    /// <param name="assetId">The id of the asset to get.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>Asset with the given id.</returns>
    let getAsset (assetId: int64) (next: NextHandler<AssetReadDto,'a>) : HttpContext -> Async<Context<'a>> =
        GetAsset.getAsset assetId fetch next
    
    /// <summary>
    /// Retrieves information about an asset given an asset id.
    /// </summary>
    /// <param name="assetId">The id of the asset to get.</param>
    /// <returns>Asset with the given id.</returns>
    let getAssetAsync (assetId: int64) : HttpContext -> Async<Context<AssetReadDto>> =
        GetAsset.getAsset assetId fetch Async.single

[<Extension>]
type GetAssetExtensions =
    /// <summary>
    /// Retrieves information about an asset given an asset id.
    /// </summary>
    /// <param name="assetId">The id of the asset to get.</param>
    /// <returns>Asset with the given id.</returns>
    [<Extension>]
    static member GetAssetAsync (this: Client, assetId: int64, [<Optional>] token: CancellationToken) : Task<AssetReadDto> =
        async {
            let! ctx = getAssetAsync assetId this.Ctx
            match ctx.Result with
            | Ok asset ->
                return asset
            | Error error ->
                let err = error2Exception error
                return raise err
        } |> fun op -> Async.StartAsTask(op, cancellationToken = token)
