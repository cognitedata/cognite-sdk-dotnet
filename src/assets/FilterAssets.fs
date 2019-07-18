namespace Cognite.Sdk

open System
open System.IO
open System.Collections.Generic
open System.Net.Http
open System.Runtime.CompilerServices
open System.Threading.Tasks

open FSharp.Control.Tasks.V2
open Thoth.Json.Net

open Cognite.Sdk
open Cognite.Sdk.Assets
open Cognite.Sdk.Api
open Cognite.Sdk.Common

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


    let encodeRequest options filters =
        Encode.object [
            if not (Seq.isEmpty filters) then
                yield "filter", SearchAssets.Filter.Encode filters
            yield! options |> Seq.map Option.Render
        ]
    let filterAssets (options: Option seq) (filters: SearchAssets.Filter seq)(fetch: HttpHandler<HttpResponseMessage, Stream, 'a>) =
        let decoder = decodeResponse GetAssets.Assets.Decoder id
        let body = encodeRequest options filters |> Encode.stringify

        POST
        >=> setVersion V10
        >=> setBody body
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
    let filterAssets (options: FilterAssets.Option seq) (filters: SearchAssets.Filter seq) (next: NextHandler<GetAssets.Assets,'a>)
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
    let filterAssetsAsync (options: FilterAssets.Option seq) (filters: SearchAssets.Filter seq)
        : HttpContext -> Async<Context<GetAssets.Assets>> =
            FilterAssets.filterAssets options filters fetch Async.single
[<Extension>]
type FilterAssetsExtensions =
    /// <summary>
    /// Retrieves list of assets matching filter. Supports pagination
    /// </summary>
    ///
    ///   * `options` - optional limit and cursor
    ///   * `filters` - Search filters
    ///
    ///<returns>Assets</returns>
    [<Extension>]
    static member FilterAssetsAsync (this: Client, options: FilterAssets.Option seq, filters: SearchAssets.Filter seq) =
        task {
            let! ctx = filterAssetsAsync options filters this.Ctx
            match ctx.Result with
            | Ok assets ->
                return {|
                        NextCursor = assets.NextCursor
                        Items = assets.Items |> Seq.map (fun asset -> asset.ToPoco ())
                    |}
            | Error error ->
                let! err = error2Exception error
                return raise err
        }