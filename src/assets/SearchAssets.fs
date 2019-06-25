namespace Cognite.Sdk

open System.Collections.Generic
open System.Net.Http
open System.Runtime.CompilerServices
open System.Threading.Tasks

open FSharp.Control.Tasks.V2
open Thoth.Json.Net

open Cognite.Sdk
open Cognite.Sdk.Common
open Cognite.Sdk.Api

[<RequireQualifiedAccess>]
module SearchAssets =
    [<Literal>]
    let Url = "/assets/search"

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

    type Option =
        private
        | CaseName of string
        | CaseDescription of string

        static member Name name = CaseName name
        static member Description description = CaseDescription description

        static member Encode (filters: Option seq) =
            Encode.object [
                for filter in filters do
                    match filter with
                    | CaseName name -> yield "name", Encode.string name
                    | CaseDescription desc -> yield "description", Encode.string desc
            ]

    type Filter =
        private
        | CaseName of string
        | CaseParentIds of int64 seq
        | CaseMetaData of Map<string, string>
        | CaseSource of string

        /// Name of asset. Often referred to as tag.
        static member Name name = CaseName name
        /// Filter out assets that have one of the ids listed as parent. The
        static member ParentIds ids = CaseParentIds ids
        static member MetaData (metaData : IDictionary<string, string>) =
            CaseMetaData (metaData |> Seq.map (|KeyValue|) |> Map.ofSeq)
        /// The source of this asset.
        static member Source source = CaseSource source

        static member Encode (filters: Filter seq) =
            Encode.object [
                for filter in filters do
                    match filter with
                    | CaseName name -> yield "name", Encode.string name
                    | CaseParentIds ids -> yield "parentIds", Encode.int53seq ids
                    | CaseSource source -> yield "source", Encode.string source
                    | CaseMetaData md -> yield "metaData", Encode.propertyBag md
            ]

    type Assets = {
        Items: Asset seq } with

        static member Decoder : Decoder<Assets> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list Asset.Decoder |> Decode.map seq)
            })

    let encodeRequest limit options filters =
        Encode.object [
            if limit > 0 then
                yield "limit", Encode.int limit
            if not (Seq.isEmpty filters) then
                yield "filter", Filter.Encode filters
            if not (Seq.isEmpty options) then
                yield "search", Option.Encode options
        ]

    let searchAssets (limit: int) (options: Option seq) (filters: Filter seq)(fetch: HttpHandler<HttpResponseMessage,string, 'a>) =
        let decoder = decodeResponse GetAssets.Assets.Decoder id
        let body = encodeRequest limit options filters |> Encode.stringify

        POST
        >=> setVersion V10
        >=> setBody body
        >=> setResource Url
        >=> fetch
        >=> decoder

[<AutoOpen>]
module SearchAssetsApi =

    /// **Description**
    ///
    /// Retrieves a list of assets matching the given criteria. This operation does not support pagination.
    ///
    /// **Parameters**
    ///
    ///   * `limit` - Limits the maximum number of results to be returned by single request. In case there are more
    ///   results to the request 'nextCursor' attribute will be provided as part of response. Request may contain less
    ///   results than request limit.
    ///   * `options` - Search options.
    ///   * `filters` - Search filters.
    ///
    /// <returns>Context of assets.</returns>
    let searchAssets (limit: int) (options: SearchAssets.Option seq) (filters: SearchAssets.Filter seq) (next: NextHandler<GetAssets.Assets,'a>) : HttpContext -> Async<Context<'a>> =
        SearchAssets.searchAssets limit options filters fetch next

    /// **Description**
    ///
    /// Retrieves a list of assets matching the given criteria. This operation does not support pagination.
    ///
    /// **Parameters**
    ///
    ///   * `limit` - Limits the maximum number of results to be returned by single request. In case there are more
    ///   results to the request 'nextCursor' attribute will be provided as part of response. Request may contain less
    ///   results than request limit.
    ///   * `options` - Search options.
    ///   * `filters` - Search filters.
    ///
    /// <returns>Context of assets.</returns>
    let searchAssetsAsync (limit: int) (options: SearchAssets.Option seq) (filters: SearchAssets.Filter seq): HttpContext -> Async<Context<GetAssets.Assets>> =
        SearchAssets.searchAssets limit options filters fetch Async.single

[<Extension>]
type SearchAssetsExtensions =
    /// <summary>
    /// Retrieves a list of assets matching the given criteria. This operation does not support pagination.
    /// </summary>
    ///
    ///   * `limit` - Limits the maximum number of results to be returned by single request. In case there are more
    ///   results to the request 'nextCursor' attribute will be provided as part of response. Request may contain less
    ///   results than request limit.
    ///   * `options` - Search options.
    ///   * `filters` - Search filters.
    ///
    /// <returns>Assets.</returns>
    [<Extension>]
    static member SearchAssetsAsync (this: Client, limit : int, options: SearchAssets.Option seq, filters: SearchAssets.Filter seq) : Task<GetAssets.Assets> =
        task {
            let! ctx = searchAssetsAsync limit options filters this.Ctx
            match ctx.Result with
            | Ok assets ->
                return assets
            | Error error ->
                return raise (Error.error2Exception error)
        }
