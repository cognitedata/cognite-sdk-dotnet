namespace Cognite.Sdk

open System.Collections.Generic
open System.Net.Http
open System.Runtime.CompilerServices
open System.Threading.Tasks

open FSharp.Control.Tasks.V2
open Thoth.Json.Net

open Cognite.Sdk
open Cognite.Sdk.Api
open Cognite.Sdk.Assets
open Cognite.Sdk.Common

[<RequireQualifiedAccess>]
module SearchAssets =
    [<Literal>]
    let Url = "/assets/search"

    type Option =
        private
        | CaseName of string
        | CaseDescription of string

        static member Name name = CaseName name
        static member Description description = CaseDescription description

        static member Encode (options: Option seq) =
            Encode.object [
                for option in options do
                    match option with
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
            metaData |> Seq.map (|KeyValue|) |> Map.ofSeq |> CaseMetaData
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
        Items: AssetReadDto seq } with

        static member Decoder : Decoder<Assets> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list AssetReadDto.Decoder |> Decode.map seq)
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
        let decoder = decodeResponse GetAssets.Assets.Decoder (fun assets -> assets.Items)
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
    let searchAssets (limit: int) (options: SearchAssets.Option seq) (filters: SearchAssets.Filter seq) (next: NextHandler<AssetReadDto seq,'a>) : HttpContext -> Async<Context<'a>> =
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
    let searchAssetsAsync (limit: int) (options: SearchAssets.Option seq) (filters: SearchAssets.Filter seq): HttpContext -> Async<Context<AssetReadDto seq>> =
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
    static member SearchAssetsAsync (this: Client, limit : int, options: SearchAssets.Option seq, filters: SearchAssets.Filter seq) : Task<Asset seq> =
        task {
            let! ctx = searchAssetsAsync limit options filters this.Ctx
            match ctx.Result with
            | Ok assets ->
                return assets |> Seq.map (fun asset -> asset.Asset ())
            | Error error ->
                return raise (Error.error2Exception error)
        }
