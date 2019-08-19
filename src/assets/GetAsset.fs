namespace CogniteSdk.Assets

open System.IO
open System.Net.Http

open Oryx
open CogniteSdk


[<RequireQualifiedAccess>]
module Single =
    [<Literal>]
    let Url = "/assets"

    let getCore (assetId: int64) (fetch: HttpHandler<HttpResponseMessage, Stream, 'a>) =
        let decoder = Encode.decodeResponse AssetReadDto.Decoder id
        let url = Url + sprintf "/%d" assetId

        GET
        >=> setVersion V10
        >=> setResource url
        >=> fetch
        >=> decoder

    /// <summary>
    /// Retrieves information about an asset given an asset id. Expects a next continuation handler.
    /// </summary>
    /// <param name="assetId">The id of the asset to get.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>Asset with the given id.</returns>
    let get (assetId: int64) (next: NextHandler<AssetReadDto,'a>) : HttpContext -> Async<Context<'a>> =
        getCore assetId fetch next

    /// <summary>
    /// Retrieves information about an asset given an asset id.
    /// </summary>
    /// <param name="assetId">The id of the asset to get.</param>
    /// <returns>Asset with the given id.</returns>
    let getAsync (assetId: int64) : HttpContext -> Async<Context<AssetReadDto>> =
        getCore assetId fetch Async.single

namespace CogniteSdk

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open Oryx
open CogniteSdk.Assets

[<Extension>]
type GetAssetClientExtensions =
    /// <summary>
    /// Retrieves information about an asset given an asset id.
    /// </summary>
    /// <param name="assetId">The id of the asset to get.</param>
    /// <returns>Asset with the given id.</returns>
    [<Extension>]
    static member GetAsync (this: ClientExtensions.Assets, assetId: int64, [<Optional>] token: CancellationToken) : Task<AssetReadDto> =
        async {
            let! ctx = Single.getAsync assetId this.Ctx
            match ctx.Result with
            | Ok asset ->
                return asset
            | Error error ->
                let err = error2Exception error
                return raise err
        } |> fun op -> Async.StartAsTask(op, cancellationToken = token)
