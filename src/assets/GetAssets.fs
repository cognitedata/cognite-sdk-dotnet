namespace Cognite.Sdk

open System.Net.Http
open System.Runtime.CompilerServices
open System.Threading.Tasks

open FSharp.Control.Tasks.V2
open Thoth.Json.Net

open Cognite.Sdk
open Cognite.Sdk.Common
open Cognite.Sdk.Api

[<RequireQualifiedAccess>]
module GetAssets =
    [<Literal>]
    let Url = "/assets"

    /// Asset type for responses.
    type Asset = {
        /// External Id provided by client. Should be unique within the project.
        ExternalId: string option
        /// The name of the asset.
        Name: string
        /// The parent ID of the asset.
        ParentId: int64 option
        /// The description of the asset.
        Description: string option
        /// Custom, application specific metadata. String key -> String value
        MetaData: Map<string, string>
        /// The source of this asset
        Source: string option
        /// The Id of the asset.
        Id: int64
        ///IDs of assets on the path to the asset.
        Path: int64 seq
        /// Asset path depth (number of levels below root node).
        Depth: int
        /// Time when this asset was created in CDF in milliseconds since Jan 1, 1970.
        CreatedTime: int64
        /// The last time this asset was updated in CDF, in milliseconds since Jan 1, 1970.
        LastUpdatedTime: int64 } with

        member this.Poco () = {|
            ExternalId = if this.ExternalId.IsSome then this.ExternalId.Value else Unchecked.defaultof<string>
            Name = this.Name
            ParentId = if this.ParentId.IsSome then this.ParentId.Value else Unchecked.defaultof<int64>
            Description = if this.Description.IsSome then this.Description.Value else Unchecked.defaultof<string>
            MetaData = this.MetaData |> Map.toSeq |> dict
            Source = if this.Source.IsSome then this.Source.Value else Unchecked.defaultof<string>
            Id = this.Id
            Path = this.Path
            CreatedTime = this.CreatedTime
            LastUpdatedTime = this.LastUpdatedTime
        |}

        static member Decoder : Decoder<Asset> =
            Decode.object (fun get ->
                let metadata = get.Optional.Field "metadata" (Decode.dict Decode.string)
                {
                    ExternalId = get.Optional.Field "externalId" Decode.string
                    Id = get.Required.Field "id" Decode.int64
                    Name = get.Required.Field "name" Decode.string
                    Description = get.Optional.Field "description" Decode.string
                    ParentId = get.Optional.Field "parentId" Decode.int64
                    Path = get.Required.Field "path" (Decode.list Decode.int64)
                    Source = get.Optional.Field "source" Decode.string
                    Depth = get.Required.Field "depth" Decode.int
                    MetaData = if metadata.IsSome then metadata.Value else Map.empty
                    CreatedTime = get.Required.Field "createdTime" Decode.int64
                    LastUpdatedTime = get.Required.Field "lastUpdatedTime" Decode.int64
                })

    type Assets = {
        Items: Asset seq
        NextCursor : string option } with

        static member Decoder : Decoder<Assets> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list Asset.Decoder |> Decode.map seq)
                NextCursor = get.Optional.Field "nextCursor" Decode.string
            })

    // Get parameters
    type Option =
        private // Expose members instead for C# interoperability
        | CaseLimit of int
        | CaseCursor of string
        | CaseName of string
        | CaseParentIds of int64 seq
        | CaseSource of string
        | CaseRoot of bool
        | CaseMinCreatedTime of int64
        | CaseMaxCreatedTime of int64
        | CaseMinLastUpdatedTime of int64
        | CaseMaxLastUpdatedTime of int64
        | CaseExternalIdPrefix of string

        /// Limits the number of results to be returned. The maximum results
        /// returned by the server is 1000 even if the limit specified is
        /// larger.
        static member Limit limit =
            if limit > MaxLimitSize || limit < 1 then
                failwith "Limit must be set to 1000 or less"
            CaseLimit limit

        /// Cursor for paging through results
        static member Cursor cursor =
            CaseCursor cursor

        /// Name of asset. Often referred to as tag.
        static member Name name =
            CaseName name

        /// Filter out assets that have one of the ids listed as parent. The
        /// parentId is set to null if the asset is a root asset.
        static member ParentIds ids =
            CaseParentIds ids

        /// The source of this asset.
        static member Source source =
            CaseSource source

        /// Filtered assets are root assets or not
        static member Root root =
            CaseRoot root

        /// It is the number of seconds that have elapsed since 00:00:00
        /// Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus
        /// leap seconds.
        static member MinCreatedTime time =
            CaseMinCreatedTime time

        /// It is the number of seconds that have elapsed since 00:00:00
        /// Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus
        /// leap seconds.
        static member MaxCreatedTime time =
            CaseMaxCreatedTime time

        /// It is the number of seconds that have elapsed since 00:00:00
        /// Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus
        /// leap seconds.
        static member MinLastUpdatedTime time =
            CaseMinLastUpdatedTime time

        /// It is the number of seconds that have elapsed since 00:00:00
        /// Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus
        /// leap seconds.
        static member MaxLastUpdatedTime time =
            CaseMaxLastUpdatedTime time

        /// External Id provided by client. Should be unique within the
        /// project.
        static member ExternalIdPrefix prefix =
            CaseExternalIdPrefix prefix

        static member Render (this: Option) =
            match this with
            | CaseLimit limit -> "limit", limit.ToString ()
            | CaseCursor cursor -> "cursor", cursor
            | CaseName name -> "name", name
            | CaseParentIds ids -> "parentIds", Encode.int53seq ids |> Encode.stringify
            | CaseSource source -> "source", source
            | CaseRoot root -> "root", root.ToString().ToLower()
            | CaseMinCreatedTime value -> "minCreatedTime", value.ToString ()
            | CaseMaxCreatedTime value -> "maxCreatedTime", value.ToString ()
            | CaseMinLastUpdatedTime value -> "minLastUpdatedTime", value.ToString ()
            | CaseMaxLastUpdatedTime value -> "maxLastUpdatedTime", value.ToString ()
            | CaseExternalIdPrefix externalId -> "externalIdPrefix", externalId

    let getAssets (options: Option seq) (fetch: HttpHandler<HttpResponseMessage,string, 'a>) =
        let decoder = decodeResponse Assets.Decoder id
        let query = options |> Seq.map Option.Render |> List.ofSeq

        GET
        >=> setVersion V10
        >=> addQuery query
        >=> setResource Url
        >=> fetch
        >=> decoder

[<AutoOpen>]
module GetAssetsApi =
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
    let getAssets (options: GetAssets.Option seq) (next: NextHandler<GetAssets.Assets,'a>): HttpContext -> Async<Context<'a>> =
        GetAssets.getAssets options fetch next

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
    let getAssetsAsync (options: GetAssets.Option seq) : HttpContext -> Async<Context<GetAssets.Assets>> =
        GetAssets.getAssets options fetch Async.single

[<Extension>]
type GetAssetsExtensions =
    /// <summary>
    /// Retrieve a list of all assets in the given project. The list is sorted alphabetically by name. This operation
    /// supports pagination.
    ///
    /// You can retrieve a subset of assets by supplying additional fields; Only assets satisfying all criteria will be
    /// returned. Names and descriptions are fuzzy searched using edit distance. The fuzziness parameter controls the
    /// maximum edit distance when considering matches for the name and description fields.
    /// </summary>
    /// <param name="args">The asset argument object containing parameters to get used for the asset query.</param>
    /// <returns>List of assets.</returns>
    [<Extension>]
    static member GetAssetsAsync (this: Client, args: GetAssets.Option seq) : Task<GetAssets.Assets> =
        task {
            let! ctx = getAssetsAsync args this.Ctx
            match ctx.Result with
            | Ok assets ->
                return assets
            | Error error ->
                return raise (Error.error2Exception error)
        }
