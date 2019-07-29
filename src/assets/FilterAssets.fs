namespace Fusion

open System.IO
open System.Net.Http
open System.Runtime.CompilerServices

open FSharp.Control.Tasks.V2
open Thoth.Json.Net

open Fusion
open Fusion.Api
open Fusion.Common
open Fusion.Assets

[<RequireQualifiedAccess>]
module FilterAssets =
    [<Literal>]
    let Url = "/assets/list"

    type Option =
        private
        | CaseLimit of int
        | CaseCursor of string

        /// Max number of results to return
        static member Limit limit =
            if limit > MaxLimitSize || limit < 1 then
                failwith "Limit must be set to 1000 or less"
            CaseLimit limit
        /// Cursor return from previous request
        static member Cursor cursor = CaseCursor cursor

        static member Render (this: Option) =
            match this with
            | CaseLimit limit -> "limit", Encode.int limit
            | CaseCursor cursor -> "cursor", Encode.string cursor

    type FilterAssetsRequest = {
        Filters : AssetFilter seq
        Options : Option seq
    } with 
        member this.Encoder =
            Encode.object [
                yield "filter", Encode.object [
                    yield! this.Filters |> Seq.map AssetFilter.Render
                ]
                yield! this.Options |> Seq.map Option.Render
            ]

    let filterAssets (options: Option seq) (filters: AssetFilter seq)(fetch: HttpHandler<HttpResponseMessage, Stream, 'a>) =
        let decoder = decodeResponse GetAssets.Assets.Decoder id
        let request : FilterAssetsRequest = {
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
module FilterAssetsApi =
    /// <summary>
    /// Retrieves list of assets matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="options">Optional limit and cursor</param>
    /// <param name="filters">Search filters</param>
    /// <param name="next">Async handler to use</param>
    /// <returns>List of assets matching given filters and optional cursor</returns>
    let filterAssets (options: FilterAssets.Option seq) (filters: AssetFilter seq) (next: NextHandler<GetAssets.Assets,'a>)
        : HttpContext -> Async<Context<'a>> =
            FilterAssets.filterAssets options filters fetch next

    /// <summary>
    /// Retrieves list of assets matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="options">Optional limit and cursor</param>
    /// <param name="filters">Search filters</param>
    /// <returns>List of assets matching given filters and optional cursor</returns>
    let filterAssetsAsync (options: FilterAssets.Option seq) (filters: AssetFilter seq)
        : HttpContext -> Async<Context<GetAssets.Assets>> =
            FilterAssets.filterAssets options filters fetch Async.single
[<Extension>]
type FilterAssetsExtensions =
    /// <summary>
    /// Retrieves list of assets matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="options">Optional limit and cursor</param>
    /// <param name="filters">Search filters</param>
    /// <returns>List of assets matching given filters and optional cursor</returns>
    [<Extension>]
    static member FilterAssetsAsync (this: Client, options: FilterAssets.Option seq, filters: AssetFilter seq) =
        task {
            let! ctx = filterAssetsAsync options filters this.Ctx
            match ctx.Result with
            | Ok assets ->
                return {|
                        NextCursor = assets.NextCursor
                        Items = assets.Items |> Seq.map (fun asset -> asset.ToPoco ())
                    |}
            | Error error ->
                let err = error2Exception error
                return raise err
        }
