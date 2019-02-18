namespace Cognite.Sdk

open System
open Thoth.Json.Net

open Cognite.Sdk.Context

module Assets =
    [<Literal>]
    let MaxLimitSize = 10000

    type Asset = {
        Id: int64
        Path: int64 list
        Depth: int
        Name: string
        Description: string
        ParentId: int64 option
        MetaData: Map<string, string>
        Source: string option
        SourceId: string option
        CreatedTime: int64
        LastUpdatedTime: int64 } with

        static member Decoder : Decode.Decoder<Asset> =
            Decode.object (fun get ->
                {
                    Id = get.Required.Field "id" Decode.int64
                    Name = get.Required.Field "name" Decode.string
                    Description = get.Required.Field "description" Decode.string
                    ParentId = get.Optional.Field "parentId" Decode.int64
                    Path = get.Required.Field "path" (Decode.list Decode.int64)
                    Source = get.Optional.Field "source" Decode.string
                    SourceId = get.Optional.Field "sourceId" Decode.string
                    Depth = get.Required.Field "depth" Decode.int
                    MetaData = get.Required.Field "metadata" (Decode.dict Decode.string)
                    CreatedTime = get.Required.Field "createdTime" Decode.int64
                    LastUpdatedTime = get.Required.Field "lastUpdatedTime" Decode.int64
                })

    type Data = {
        Items: Asset list
        PreviousCursor: string option
        NextCursor : string option } with

        static member Decoder : Decode.Decoder<Data> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list Asset.Decoder)
                PreviousCursor = get.Optional.Field "previousCursor" Decode.string
                NextCursor = get.Optional.Field "nextCursor" Decode.string
            })

    type AssetResponse = { Data: Data } with
        static member Decoder : Decode.Decoder<AssetResponse> =
            Decode.object (fun get -> {
                Data = get.Required.Field "data" Data.Decoder
            })

    // Get arguments
    type Args =
        | Id of int64
        | Name of string
        | Description of string
        | Path of string
        | MetaData of Map<string, string> option
        | Depth of int
        | Fuzziness of int
        | AutoPaging of bool
        | NotLimit of int
        | Cursor of string

        static member Limit limit =
            if limit > MaxLimitSize || limit < 1 then
                failwith "Limit must be set to 1000 or less"
            NotLimit limit

    let Limit = Args.Limit

    /// Update arguments
    type UpdateArgs =
        | SetName of string // Name cannot be null
        | SetDescription of string option
        | SetMetaData of Map<string, string> option
        | SetSource of string option
        | SetSourceId of string option

    type AssetRequest = {
        Name: string
        Description: string
        MetaData: Map<string, string>
        Source: string option
        SourceId: string option
        CreatedTime: int64
        LastUpdatedTime: int64

        RefId: string option
        ParentRef: ParentRef option } with

        member this.Encoder =
            Encode.object [
                yield ("name", Encode.string this.Name)
                yield ("description", Encode.string this.Description)
                yield ("createdTime", Encode.int64 this.CreatedTime)

                if this.Source.IsSome then
                    yield ("source", Encode.string this.Source.Value)
                if this.SourceId.IsSome then
                    yield ("sourceId", Encode.string this.SourceId.Value)
            ]

    type AssetsRequest = {
        Items: AssetRequest list
        } with

        member this.Encoder =
            Encode.object [
                yield ("items", List.map (fun (it: AssetRequest) -> it.Encoder) this.Items |> Encode.list)
            ]

    let renderArgs (arg: Args) =
        match arg with
        | Id id -> ("id", id.ToString())
        | Name name -> ("name", name)
        | Description desc -> ("desc", desc)
        | Path path -> ("path", path)
        | MetaData md -> ("metadata", "fixme")
        | Depth depth -> ("depth", depth.ToString())
        | Fuzziness fuzz -> ("fuzziness", fuzz.ToString ())
        | AutoPaging value -> ("autopaging", value.ToString().ToLower())
        | NotLimit limit -> ("limit", limit.ToString ())
        | Cursor cursor -> ("cursor", cursor)

    let renderUpdateArgs (arg: UpdateArgs) =
        match arg with
        | SetName name ->
            ("name", Encode.object [
                ("set", Encode.string name)
            ])
        | SetDescription optdesc ->
            ("description", Encode.object [
                match optdesc with
                | Some desc -> yield ("set", Encode.string desc)
                | None -> yield ("setNull", Encode.bool true)
            ])

    [<Literal>]
    let Url = "/assets"

    /// JSON decode response and map decode error string to exception so we don't get more response error types.
    let decodeResponse decoder result =
        result
        |> Result.bind (fun res ->
            let ret = Decode.fromString decoder res
            match ret with
            | Error str -> Error (DecodeException str)
            | Ok value -> Ok value
        )

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
    let getAssets (ctx: Context) (args: Args list) : Async<Result<AssetResponse, exn>> = async {
        let query = args |> List.map renderArgs
        let url = Resource Url
        let ctx' =
            ctx
            |> setMethod Get
            |> addQueries query
            |> setResource url

        let! result =
            ctx.Fetch ctx'

        return result |> decodeResponse AssetResponse.Decoder
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
    let getAsset (ctx: Context) (assetId: int64) : Async<Result<AssetResponse, exn>> = async {
        let url = Url + sprintf "/%d" assetId |> Resource

        let ctx' =
            ctx
            |> setMethod Get
            |> setResource url

        let! result = ctx.Fetch ctx'
        return result |> decodeResponse AssetResponse.Decoder
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
    let createAsset (ctx: Context) (assets: AssetsRequest) = async {
        let body = Encode.toString 0 assets.Encoder
        let url = Resource Url
        let ctx' =
            ctx
            |> setMethod Post
            |> setBody body
            |> setResource url

        return! ctx.Fetch ctx'
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
        let url = Resource Url
        let ctx' =
            ctx
            |> setMethod Post
            |> setBody body
            |> setResource url

        return! ctx.Fetch ctx'
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
    let updateAsset (ctx: Context) (assetId: int64) (args: UpdateArgs list) = async {
        let encoder = Encode.object [
            yield ("id", Encode.int64 assetId)
            for arg in args do
                yield renderUpdateArgs arg
        ]

        let body = Encode.toString 0 encoder
        let url = Url + sprintf "/%d/update" assetId |> Resource
        let ctx' =
            ctx
            |> setMethod Post
            |> setBody body
            |> setResource url

        return! ctx.Fetch ctx'
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
    let updateAssets (ctx: Context) (args: (int64*UpdateArgs list) list) = async {
        let encoder = Encode.object [
            for (assetId, args) in args do
                yield ("id", Encode.int64 assetId)
                for arg in args do
                    yield renderUpdateArgs arg
        ]

        let body = Encode.toString 0 encoder
        let url = Url + sprintf "/update" |> Resource
        let ctx' =
            ctx
            |> setMethod Post
            |> setBody body
            |> setResource url

        return! ctx.Fetch ctx'
    }