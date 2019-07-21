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
    /// **Description**
    ///
    /// Retrieves a list of assets matching the filter, and a cursor if given limit is exceeded
    ///
    /// **Parameters**
    ///
    ///   * `options` - optional limit and cursor
    ///   * `filters` - Search filters
    ///
    ///<returns>Assets.</return>
    let filterAssets (options: FilterAssets.Option seq) (filters: AssetFilter seq) (next: NextHandler<GetAssets.Assets,'a>)
        : HttpContext -> Async<Context<'a>> =
            FilterAssets.filterAssets options filters fetch next
    /// **Description**
    ///
    /// Retrieves a list of assets matching the filter, and a cursor if given limit is exceeded
    ///
    /// **Parameters**
    ///
    ///   * `options` - optional limit and cursor
    ///   * `filters` - Search filters
    ///
    ///<returns>Assets.</return>
    let filterAssetsAsync (options: FilterAssets.Option seq) (filters: AssetFilter seq)
        : HttpContext -> Async<Context<GetAssets.Assets>> =
            FilterAssets.filterAssets options filters fetch Async.single
[<Extension>]
type FilterAssetsExtensions =
    /// <summary>
    /// Retrieves list of assets matching filter. Supports pagination
    /// </summary>
    /// <param name="options">Optional limit and cursor</param>
    /// <param name="filters">Search filters</param>
    ///<returns>Assets</returns>
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
