namespace Cognite.Sdk.Assets

open Cognite.Sdk
open Cognite.Sdk.Assets
open Cognite.Sdk.Common
open Cognite.Sdk.Request

[<RequireQualifiedAccess>]
module Internal =

    let getAssets (args: GetParams seq) (fetch: HttpHandler) =
        let decoder = decodeResponse AssetResponse.Decoder (fun res -> res.Data)
        let query = args |> Seq.map renderParams |> List.ofSeq

        GET
        >=> addQuery query
        >=> setResource Url
        >=> fetch
        >=> decoder

    let getAssetsResult (args: GetParams seq) (fetch: HttpHandler) (ctx: HttpContext) =
        getAssets args fetch ctx
        |> Async.map (fun decoded -> decoded.Result)

    let getAsset (assetId: int64) (fetch: HttpHandler) =
        let decoder = decodeResponse AssetResponse.Decoder (fun res -> Seq.head res.Data.Items)
        let url = Url + sprintf "/%d" assetId

        GET
        >=> setResource url
        >=> fetch
        >=> decoder

    let getAssetResult (assetId: int64) (fetch: HttpHandler) (ctx: HttpContext) =
        getAsset assetId fetch ctx
        |> Async.map (fun decoded -> decoded.Result)

    let createAssets (assets: AssetCreateDto seq) (fetch: HttpHandler)  =
        let decoder = decodeResponse AssetResponse.Decoder (fun res -> res.Data.Items)
        let request : AssetsCreateRequest = { Items = assets }
        let body = encodeToString  request.Encoder

        POST
        >=> setBody body
        >=> setResource Url
        >=> fetch
        >=> decoder

    let createAssetsResult (assets: AssetCreateDto seq) (fetch: HttpHandler) (ctx: HttpContext) =
        createAssets assets fetch ctx
        |> Async.map (fun context -> context.Result)

    let deleteAssets (assets: int64 seq) (fetch: HttpHandler) =
        let request : AssetsDeleteRequest = { Items = assets }
        let body = encodeToString  request.Encoder

        POST
        >=> setBody body
        >=> setResource Url
        >=> fetch

    let deleteAssetsResult (assets: int64 seq) (fetch: HttpHandler) (ctx: HttpContext) =
        deleteAssets assets fetch ctx
        |> Async.map (fun ctx -> ctx.Result)

    let updateAsset (assetId: int64) (args: UpdateParams seq) (fetch: HttpHandler) =
        let request = { Id = assetId; Params = args }
        let body = encodeToString request.Encoder
        let url = Url + sprintf "/%d/update" assetId

        POST
        >=> setBody body
        >=> setResource url
        >=> fetch

    let updateAssetResult (assetId: int64) (args: UpdateParams seq) (fetch: HttpHandler) (ctx: HttpContext) =
        updateAsset assetId args fetch ctx
        |> Async.map (fun ctx -> ctx.Result)

    let updateAssets (args: (int64*UpdateParams seq) seq) (fetch: HttpHandler) =
        let request : AssetsUpdateRequest = {
            Items = [
                for (assetId, args) in args do
                    yield { Id = assetId; Params = args }
            ]
        }

        let body = encodeToString request.Encoder
        let url = Url + sprintf "/update"

        POST
        >=> setBody body
        >=> setResource url
        >=> fetch

    let updateAssetsResult (args: (int64*UpdateParams seq) seq) (fetch: HttpHandler) (ctx: HttpContext) =
        updateAssets args fetch ctx
        |> Async.map (fun ctx -> ctx.Result)

[<AutoOpen>]
module Methods =

    /// **Description**
    ///
    /// Retrieve a list of all assets in the given project. The list is sorted alphabetically by name. This operation
    /// supports pagination.
    ///
    /// You can retrieve a subset of assets by supplying additional fields; Only assets satisfying all criteria will be
    /// returned. Names and descriptions are fuzzy searched using edit distance. The fuzziness parameter controls the
    /// maximum edit distance when considering matches for the name and description fields.
    ///
    /// **Parameters**
    ///   * `args` - list of parameters for getting assets.
    ///   * `ctx` - The request HTTP context to use.
    ///
    /// **Output Type**
    ///   * `Async<Result<Response,exn>>`
    ///
    let getAssets (args: seq<GetParams>) (ctx: HttpContext) : Async<Context<AssetResponseData>> =
        Internal.getAssets args Request.fetch ctx

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
    let getAsset (assetId: int64) (ctx: HttpContext) : Async<Context<AssetReadDto>> =
        Internal.getAsset assetId Request.fetch ctx

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
    let createAssets (assets: AssetCreateDto seq) (ctx: HttpContext) =
        Internal.createAssets assets Request.fetch ctx

    /// **Description**
    ///
    /// Deletes multiple assets in the same project, along with all their descendants in the asset hierarchy.
    ///
    /// **Parameters**
    ///   * `assets` - The list of asset ids to delete.
    ///   * `ctx` - The request HTTP context to use.
    ///
    /// **Output Type**
    ///   * `Async<Result<HttpResponse,ResponseError>>`
    ///
    let deleteAssets (assets: int64 seq) (ctx: HttpContext) =
        Internal.deleteAssets assets Request.fetch ctx

    /// **Description**
    ///
    /// Updates an asset object. Supports partial updates i.e. updating only some fields but leaving the rest unchanged.
    ///
    /// **Parameters**
    ///   * `assetId` - The id of the asset to update.
    ///   * `args` - The list of arguments for updating the asset.
    ///   * `ctx` - The request HTTP context to use.
    ///
    /// **Output Type**
    ///   * `Async<Result<HttpResponse,ResponseError>>`
    ///
    let updateAsset (assetId: int64) (args: UpdateParams seq) (ctx: HttpContext) =
        Internal.updateAsset assetId args Request.fetch ctx

    /// **Description**
    ///
    /// Updates multiple assets within the same project. Updating assets does not replace the existing asset hierarchy.
    /// This operation supports partial updates, meaning that fields omitted from the requests are not changed.
    ///
    /// **Parameters**
    ///   * `args` - parameter of type `(int64 * UpdateArgs seq) seq`
    ///   * `ctx` - The request HTTP context to use.
    ///
    /// **Output Type**
    ///   * `Async<Result<string,exn>>`
    ///
    let updateAssets (args: (int64*UpdateParams seq) seq) (fetch: HttpHandler) (ctx: HttpContext) =
        Internal.updateAssets args
