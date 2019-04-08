namespace Cognite.Sdk.Assets

open Thoth.Json.Net

open Cognite.Sdk
open Cognite.Sdk.Assets
open Cognite.Sdk.Common
open Cognite.Sdk.Request

[<RequireQualifiedAccess>]
module Internal =

    let getAssets (args: GetParams list) (fetch: HttpHandler) =
        let decoder = decodeResponse AssetResponse.Decoder (fun res -> res.Data.Items)
        let query = args |> List.map renderParams

        GET
        >=> addQuery query
        >=> setResource Url
        >=> fetch
        >=> decoder

    let getAssetsResult (args: GetParams list) (fetch: HttpHandler) (ctx: HttpContext) =
        getAssets args fetch ctx
        |> Async.map (fun decoded -> decoded.Result)

    let getAsset (assetId: int64) (fetch: HttpHandler) =
        let decoder = decodeResponse AssetResponse.Decoder (fun res -> res.Data.Items.Head)
        let url = Url + sprintf "/%d" assetId

        GET
        >=> setResource url
        >=> fetch
        >=> decoder

    let getAssetResult (assetId: int64) (fetch: HttpHandler) (ctx: HttpContext) =
        getAsset assetId fetch ctx
        |> Async.map (fun decoded -> decoded.Result)

    let createAssets (assets: AssetCreateDto list) (fetch: HttpHandler)  =
        let decoder = decodeResponse AssetResponse.Decoder (fun res -> res.Data.Items)
        let request : AssetsCreateRequest = { Items = assets }
        let body = Encode.toString 0 request.Encoder

        POST
        >=> setBody body
        >=> setResource Url
        >=> fetch
        >=> decoder

    let createAssetsResult (assets: AssetCreateDto list) (fetch: HttpHandler) (ctx: HttpContext) =
        createAssets assets fetch ctx
        |> Async.map (fun context -> context.Result)

    let deleteAssets (assets: int64 list) (fetch: HttpHandler) =
        let request : AssetsDeleteRequest = { Items = assets }
        let body = Encode.toString 0 request.Encoder

        POST
        >=> setBody body
        >=> setResource Url
        >=> fetch

    let deleteAssetsResult (assets: int64 list) (fetch: HttpHandler) (ctx: HttpContext) =
        deleteAssets assets fetch ctx
        |> Async.map (fun ctx -> ctx.Result)

    let updateAsset (assetId: int64) (args: UpdateParams list) (fetch: HttpHandler) =
        let request = { Id = assetId; Params = args }
        let body = Encode.toString 0 request.Encoder
        let url = Url + sprintf "/%d/update" assetId

        POST
        >=> setBody body
        >=> setResource url
        >=> fetch

    let updateAssetResult (assetId: int64) (args: UpdateParams list) (fetch: HttpHandler) (ctx: HttpContext) =
        updateAsset assetId args fetch ctx
        |> Async.map (fun ctx -> ctx.Result)

    let updateAssets (args: (int64*UpdateParams list) list) (fetch: HttpHandler) =
        let request : AssetsUpdateRequest = {
            Items = [
                for (assetId, args) in args do
                    yield { Id = assetId; Params = args }
            ]
        }

        let body = Encode.toString 0 request.Encoder
        let url = Url + sprintf "/update"

        POST
        >=> setBody body
        >=> setResource url
        >=> fetch

    let updateAssetsResult (args: (int64*UpdateParams list) list) (fetch: HttpHandler) (ctx: HttpContext) =
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
    ///   * `ctx` - parameter of type `Context`
    ///   * `args` - parameter of type `Args list`
    ///
    /// **Output Type**
    ///   * `Async<Result<Response,exn>>`
    ///
    let getAssets (args: GetParams list) (ctx: HttpContext) : Async<Result<AssetReadDto list, ResponseError>> =
        Internal.getAssetsResult args Request.fetch ctx

    /// **Description**
    ///
    /// Retrieves information about an asset in a certain project given an asset id.
    ///
    /// **Parameters**
    ///   * `ctx` - parameter of type `Context`
    ///   * `assetId` - parameter of type `int64`
    ///
    /// **Output Type**
    ///   * `Async<Result<Response,exn>>`
    ///
    let getAsset (assetId: int64) (ctx: HttpContext) : Async<Result<AssetReadDto, ResponseError>> =
        Internal.getAssetResult assetId Request.fetch ctx

    /// **Description**
    ///
    /// Creates new assets in the given project
    ///
    /// **Parameters**
    ///   * `ctx` - parameter of type `Context`
    ///   * `assets` - parameter of type `AssetsRequest`
    ///
    /// **Output Type**
    ///   * `Async<Result<Response,exn>>`
    ///
    let createAssets (assets: AssetCreateDto list) (ctx: HttpContext) =
        Internal.createAssetsResult assets Request.fetch ctx

    /// **Description**
    ///
    /// Deletes multiple assets in the same project, along with all their descendants in the asset hierarchy.
    ///
    /// **Parameters**
    ///   * `ctx` - parameter of type `Context`
    ///   * `assets` - parameter of type `int64 list`
    ///
    /// **Output Type**
    ///   * `Async<Result<Response,exn>>`
    ///
    let deleteAssets (assets: int64 list) (ctx: HttpContext) =
        Internal.deleteAssetsResult assets Request.fetch ctx

    /// **Description**
    ///
    /// Updates an asset object. Supports partial updates i.e. updating only some fields but leaving the rest unchanged.
    ///
    /// **Parameters**
    ///   * `ctx` - parameter of type `Context`
    ///   * `assetId` - parameter of type `int64`
    ///   * `args` - parameter of type `UpdateArgs list`
    ///
    /// **Output Type**
    ///   * `Async<Result<string,exn>>`
    ///
    let updateAsset (assetId: int64) (args: UpdateParams list) (ctx: HttpContext) =
        Internal.updateAssetResult assetId args Request.fetch ctx

    /// **Description**
    ///
    /// Updates multiple assets within the same project. Updating assets does not replace the existing asset hierarchy.
    /// This operation supports partial updates, meaning that fields omitted from the requests are not changed.
    ///
    /// **Parameters**
    ///   * `ctx` - parameter of type `Context`
    ///   * `args` - parameter of type `(int64 * UpdateArgs list) list`
    ///
    /// **Output Type**
    ///   * `Async<Result<string,exn>>`
    ///
    let updateAssets (args: (int64*UpdateParams list) list) (fetch: HttpHandler) (ctx: HttpContext) =
        Internal.updateAssetsResult args
