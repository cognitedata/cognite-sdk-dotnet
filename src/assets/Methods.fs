namespace Cognite.Sdk.Assets

open Thoth.Json.Net

open Cognite.Sdk
open Cognite.Sdk.Common
open Cognite.Sdk.Request
open Cognite.Sdk.Assets

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
    let getAssets (fetch: HttpHandler) (args: GetParams list) (ctx: HttpContext) : Async<Context<AssetReadDto list>> = async {
        let query = args |> List.map renderParams
        let url = Url

        let! result =
            ctx
            |> setMethod GET
            |> addQuery query
            |> setResource url
            |> fetch

        return
            result
            |> decodeResponse AssetResponse.Decoder (fun res -> res.Data.Items)
    }

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
    let getAsset (fetch: HttpHandler) (assetId: int64) (ctx: HttpContext) : Async<Context<AssetReadDto>> = async {
        let url = Url + sprintf "/%d" assetId

        let! result =
            ctx
            |> setMethod GET
            |> setResource url
            |> fetch
        return
            result
            |> decodeResponse AssetResponse.Decoder (fun res -> res.Data.Items.Head)
    }

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
    let createAssets (fetch: HttpHandler) (assets: AssetCreateDto list) (ctx: HttpContext) = async {
        let request : AssetsCreateRequest = { Items = assets }
        let body = Encode.toString 0 request.Encoder
        let url = Url

        let! response =
            ctx
            |> setMethod POST
            |> setBody body
            |> setResource url
            |> fetch

        return response
            |> decodeResponse AssetResponse.Decoder (fun res -> res.Data.Items)
    }

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
    let deleteAssets (fetch: HttpHandler) (assets: int64 list) (ctx: HttpContext) = async {
        let request : AssetsDeleteRequest = { Items = assets }
        let body = Encode.toString 0 request.Encoder
        let url = Url

        let! response =
            ctx
            |> setMethod POST
            |> setBody body
            |> setResource url
            |> fetch

        return response
    }

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
    let updateAsset (fetch: HttpHandler) (assetId: int64) (args: UpdateParams list) (ctx: HttpContext) = async {
        let request = { Id = assetId; Params = args }
        let body = Encode.toString 0 request.Encoder
        let url = Url + sprintf "/%d/update" assetId

        let! response =
            ctx
            |> setMethod POST
            |> setBody body
            |> setResource url
            |> fetch
        return response
    }

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
    let updateAssets (fetch: HttpHandler) (args: (int64*UpdateParams list) list) (ctx: HttpContext) = async {
        let request : AssetsUpdateRequest = {
            Items = [
                for (assetId, args) in args do
                    yield { Id = assetId; Params = args }
            ]
        }

        let body = Encode.toString 0 request.Encoder
        let url = Url + sprintf "/update"

        let! response =
            ctx
            |> setMethod POST
            |> setBody body
            |> setResource url
            |> fetch
        return response
    }