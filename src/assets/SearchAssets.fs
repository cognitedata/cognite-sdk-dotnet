namespace Fusion

open System.IO
open System.Net.Http
open System.Runtime.CompilerServices
open System.Threading.Tasks

open FSharp.Control.Tasks.V2
open Thoth.Json.Net

open Fusion
open Fusion.Assets
open Fusion.Api
open Fusion.Common

[<RequireQualifiedAccess>]
module SearchAssets =
    [<Literal>]
    let Url = "/assets/search"

    type Option =
        private
        | CaseName of string
        | CaseDescription of string

        /// Fuzzy search on name
        static member Name name = CaseName name
        /// Fuzzy search on description
        static member Description description = CaseDescription description

        static member Render (this: Option) =
            match this with
            | CaseName name -> "name", Encode.string name
            | CaseDescription desc -> "description", Encode.string desc

    type Assets = {
        Items: AssetReadDto seq
    } with
        static member Decoder : Decoder<Assets> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list AssetReadDto.Decoder |> Decode.map seq)
            })
    
    type SearchAssetsRequest = {
        Limit: int
        Filters: AssetFilter seq
        Options: Option seq 
    } with
        member this.Encoder =
            Encode.object [
                yield "filter", Encode.object [
                    yield! this.Filters |> Seq.map AssetFilter.Render
                ]
                yield "search", Encode.object [
                    yield! this.Options |> Seq.map Option.Render
                ]
                if this.Limit > 0 then
                    yield "limit", Encode.int this.Limit
            ]
            
    let searchAssets (limit: int) (options: Option seq) (filters: AssetFilter seq)(fetch: HttpHandler<HttpResponseMessage, Stream, 'a>) =
        let decoder = decodeResponse GetAssets.Assets.Decoder (fun assets -> assets.Items)
        let request : SearchAssetsRequest = {
            Limit = limit
            Filters = filters
            Options = options
        }

        POST
        >=> setVersion V10
        >=> setContent (Content.JsonValue request.Encoder)
        >=> setResource Url
        >=> fetch
        >=> decoder

[<AutoOpen>]
module SearchAssetsApi =
    /// <summary>
    /// Retrieves a list of assets matching the given criteria. This operation does not support pagination.
    /// </summary>
    ///
    /// <param name="limit">Limits the maximum number of results to be returned by single request.</param>
    /// <param name="options">Search options.</param>
    /// <param name="filters">Search filters.</param>
    ///
    /// <returns>List of assets matching given criteria.</returns>
    let searchAssets (limit: int) (options: SearchAssets.Option seq) (filters: AssetFilter seq) (next: NextHandler<AssetReadDto seq,'a>) : HttpContext -> Async<Context<'a>> =
        SearchAssets.searchAssets limit options filters fetch next

    /// <summary>
    /// Retrieves a list of assets matching the given criteria. This operation does not support pagination.
    /// </summary>
    ///
    /// <param name="limit">Limits the maximum number of results to be returned by single request.</param>
    /// <param name="options">Search options.</param>
    /// <param name="filters">Search filters.</param>
    ///
    /// <returns>List of assets matching given criteria.</returns>
    let searchAssetsAsync (limit: int) (options: SearchAssets.Option seq) (filters: AssetFilter seq): HttpContext -> Async<Context<AssetReadDto seq>> =
        SearchAssets.searchAssets limit options filters fetch Async.single

[<Extension>]
type SearchAssetsExtensions =
    /// <summary>
    /// Retrieves a list of assets matching the given criteria. This operation does not support pagination.
    /// </summary>
    ///
    /// <param name="limit">Limits the maximum number of results to be returned by single request.</param>
    /// <param name="options">Search options.</param>
    /// <param name="filters">Search filters.</param>
    ///
    /// <returns>List of assets matching given criteria.</returns>
    [<Extension>]
    static member SearchAssetsAsync (this: Client, limit : int, options: SearchAssets.Option seq, filters: AssetFilter seq) : Task<_ seq> =
        task {
            let! ctx = searchAssetsAsync limit options filters this.Ctx
            match ctx.Result with
            | Ok assets ->
                return assets |> Seq.map (fun asset -> asset.ToPoco ())
            | Error error ->
                let err = error2Exception error
                return raise err
        }
