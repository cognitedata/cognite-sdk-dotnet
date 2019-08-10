namespace Fusion.Assets

open System.IO
open System.Net.Http

open Thoth.Json.Net

open Fusion
open Fusion.Common

[<RequireQualifiedAccess>]
module Search =
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
        Items: ReadDto seq
    } with
        static member Decoder : Decoder<Assets> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list ReadDto.Decoder |> Decode.map seq)
            })

    type SearchAssetsRequest = {
        Limit: int
        Filters: FilterOption seq
        Options: Option seq
    } with
        member this.Encoder =
            Encode.object [
                yield "filter", Encode.object [
                    yield! this.Filters |> Seq.map FilterOption.Render
                ]
                yield "search", Encode.object [
                    yield! this.Options |> Seq.map Option.Render
                ]
                if this.Limit > 0 then
                    yield "limit", Encode.int this.Limit
            ]

    let searchCore (limit: int) (options: Option seq) (filters: FilterOption seq)(fetch: HttpHandler<HttpResponseMessage, Stream, 'a>) =
        let decoder = decodeResponse List.Assets.Decoder (fun assets -> assets.Items)
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

    /// <summary>
    /// Retrieves a list of assets matching the given criteria. This operation does not support pagination.
    /// </summary>
    ///
    /// <param name="limit">Limits the maximum number of results to be returned by single request.</param>
    /// <param name="options">Search options.</param>
    /// <param name="filters">Search filters.</param>
    ///
    /// <returns>List of assets matching given criteria.</returns>
    let search (limit: int) (options: Option seq) (filters: FilterOption seq) (next: NextHandler<ReadDto seq,'a>) : HttpContext -> Async<Context<'a>> =
        searchCore limit options filters fetch next

    /// <summary>
    /// Retrieves a list of assets matching the given criteria. This operation does not support pagination.
    /// </summary>
    ///
    /// <param name="limit">Limits the maximum number of results to be returned by single request.</param>
    /// <param name="options">Search options.</param>
    /// <param name="filters">Search filters.</param>
    ///
    /// <returns>List of assets matching given criteria.</returns>
    let searchAsync (limit: int) (options: Option seq) (filters: FilterOption seq): HttpContext -> Async<Context<ReadDto seq>> =
        searchCore limit options filters fetch Async.single

namespace Fusion

open System.Runtime.CompilerServices
open System.Threading.Tasks
open System.Runtime.InteropServices
open System.Threading

open Fusion.Assets
open Fusion.Common

[<Extension>]
type SearchAssetsClientExtensions =
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
    static member SearchAsync (this: ClientExtensions.Assets, limit : int, options: Search.Option seq, filters: FilterOption seq, [<Optional>] token: CancellationToken) : Task<_ seq> =
        async {
            let! ctx = Search.searchAsync limit options filters this.Ctx
            match ctx.Result with
            | Ok assets ->
                return assets |> Seq.map (fun asset -> asset.ToPoco ())
            | Error error ->
                let err = error2Exception error
                return raise err
        } |> fun op -> Async.StartAsTask (op, cancellationToken = token)
