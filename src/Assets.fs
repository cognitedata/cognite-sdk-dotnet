namespace Cognite.Sdk

open System
open Thoth.Json.Net

open Cognite.Sdk.Context

module Assets =
    [<Literal>]
    let MaxLimitSize = 10000

    [<Literal>]
    let Url = "/assets"

    module Response =
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

    // Get parameters
    type GetParams =
        | Id of int64
        | Name of string
        | Description of string
        | Path of string
        | MetaData of Map<string, string>
        | Depth of int
        | Fuzziness of int
        | AutoPaging of bool
        | NotLimit of int
        | Cursor of string

        static member Limit limit =
            if limit > MaxLimitSize || limit < 1 then
                failwith "Limit must be set to 1000 or less"
            NotLimit limit

    let Limit = GetParams.Limit

    /// Update parameters
    type UpdateParams =
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
            Items: AssetRequest list } with

        member this.Encoder =
            Encode.object [
                yield ("items", List.map (fun (it: AssetRequest) -> it.Encoder) this.Items |> Encode.list)
            ]

    let renderParams (arg: GetParams) =
        match arg with
        | Id id -> ("id", id.ToString())
        | Name name -> ("name", name)
        | Description desc -> ("desc", desc)
        | Path path -> ("path", path)
        | MetaData meta ->
            let metaString = Encode.dict (Map.map (fun key value -> Encode.string value) meta) |> Encode.toString 0
            ("metadata", metaString)
        | Depth depth -> ("depth", depth.ToString())
        | Fuzziness fuzz -> ("fuzziness", fuzz.ToString ())
        | AutoPaging value -> ("autopaging", value.ToString().ToLower())
        | NotLimit limit -> ("limit", limit.ToString ())
        | Cursor cursor -> ("cursor", cursor)

    let renderUpdateFields (arg: UpdateParams) =
        match arg with
        | SetName name ->
            ("name", Encode.object [
                ("set", Encode.string name)
            ])
        | SetDescription optDesc ->
            ("description", Encode.object [
                match optDesc with
                | Some desc -> yield ("set", Encode.string desc)
                | None -> yield ("setNull", Encode.bool true)
            ])
        | SetMetaData optMeta ->
            ("metadata", Encode.object [
                match optMeta with
                | Some meta -> yield ("set", Encode.dict (Map.map (fun key value -> Encode.string value) meta))
                | None -> yield ("setNull", Encode.bool true)
            ])
        | SetSource optSource ->
            ("source", Encode.object [
                match optSource with
                | Some source -> yield ("set", Encode.string source)
                | None -> yield ("setNull", Encode.bool true)
            ])
        | SetSourceId optSourceId ->
            ("sourceId", Encode.object [
                match optSourceId with
                | Some sourceId -> yield ("set", Encode.string sourceId)
                | None -> yield ("setNull", Encode.bool true)
            ])

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
    let getAssets (ctx: Context) (args: GetParams list) : Async<Result<Response.Asset list, exn>> = async {
        let query = args |> List.map renderParams
        let url = Resource Url
        let! result =
            ctx
            |> setMethod Get
            |> addQuery query
            |> setResource url
            |> ctx.Fetch

        return result
            |> decodeResponse Response.AssetResponse.Decoder
            |> Result.map (fun res -> res.Data.Items)
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
    let getAsset (ctx: Context) (assetId: int64) : Async<Result<Response.Asset, exn>> = async {
        let url = Url + sprintf "/%d" assetId |> Resource

        let! result =
            ctx
            |> setMethod Get
            |> setResource url
            |> ctx.Fetch

        return result
            |> decodeResponse Response.AssetResponse.Decoder 
            |> Result.map (fun res -> res.Data.Items.Head)
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
    let createAsset (ctx: Context) (assets: AssetRequest list) = async {
        let request = { Items = assets }
        let body = Encode.toString 0 request.Encoder
        let url = Resource Url

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
        let url = Url + sprintf "/%d/update" assetId |> Resource

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
        let url = Url + sprintf "/update" |> Resource

        let! response =
            ctx
            |> setMethod Post
            |> setBody body
            |> setResource url
            |> ctx.Fetch
        return response
    }