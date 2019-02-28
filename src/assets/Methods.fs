namespace Cognite.Sdk.Assets

open Thoth.Json.Net

open Cognite.Sdk
open Cognite.Sdk.Context
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
    let getAssets (ctx: Context) (args: GetParams list) : Async<Result<ResponseAsset list, exn>> = async {
        let query = args |> List.map renderParams
        let url = Url
        let! result =
            ctx
            |> setMethod Get
            |> addQuery query
            |> setResource url
            |> ctx.Fetch

        return result
            |> decodeResponse AssetResponse.Decoder
            |> Result.map (fun res -> res.ResponseData.Items)
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
    let getAsset (ctx: Context) (assetId: int64) : Async<Result<ResponseAsset, exn>> = async {
        let url = Url + sprintf "/%d" assetId

        let! result =
            ctx
            |> setMethod Get
            |> setResource url
            |> ctx.Fetch

        return result
            |> decodeResponse AssetResponse.Decoder
            |> Result.map (fun res -> res.ResponseData.Items.Head)
    }

    /// **Description**
    ///
    /// Creates new assets in the given project.
    ///
    /// **Parameters**
    ///   * `ctx` - parameter of type `Context`
    ///   * `assets` - parameter of type `AssetsRequest`
    ///
    /// **Output Type**
    ///   * `Async<Result<Response,exn>>`
    ///
    let createAssets (ctx: Context) (assets: RequestAsset list) = async {
        let request = { Items = assets }
        let body = Encode.toString 0 request.Encoder
        let url = Url

        let! response =
            ctx
            |> setMethod Post
            |> setBody body
            |> setResource url
            |> ctx.Fetch
        return response
            |> decodeResponse AssetResponse.Decoder
            |> Result.map (fun res -> res.ResponseData.Items)
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
    let deleteAssets (ctx: Context) (assets: int64 list) = async {
        let encoder = Encode.object [
            ("items", List.map Encode.int64 assets |> Encode.list)
        ]
        let body = Encode.toString 0 encoder
        let url = Url

        let! response =
            ctx
            |> setMethod Post
            |> setBody body
            |> setResource url
            |> ctx.Fetch
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
    let updateAsset (ctx: Context) (assetId: int64) (args: UpdateParams list) = async {
        let encoder = Encode.object [
            yield ("id", Encode.int64 assetId)
            for arg in args do
                yield renderUpdateFields arg
        ]

        let body = Encode.toString 0 encoder
        let url = Url + sprintf "/%d/update" assetId

        let! response =
            ctx
            |> setMethod Post
            |> setBody body
            |> setResource url
            |> ctx.Fetch
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
    let updateAssets (ctx: Context) (args: (int64*UpdateParams list) list) = async {
        let encoder = Encode.object [
            for (assetId, args) in args do
                yield ("id", Encode.int64 assetId)
                for arg in args do
                    yield renderUpdateFields arg
        ]

        let body = Encode.toString 0 encoder
        let url = Url + sprintf "/update"

        let! response =
            ctx
            |> setMethod Post
            |> setBody body
            |> setResource url
            |> ctx.Fetch
        return response
    }