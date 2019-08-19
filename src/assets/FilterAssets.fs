namespace CogniteSdk.Assets

open System.IO
open System.Net.Http


open Oryx
open Thoth.Json.Net

open CogniteSdk


type AssetFilterQuery =
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

    static member Render (this: AssetFilterQuery) =
        match this with
        | CaseLimit limit -> "limit", Encode.int limit
        | CaseCursor cursor -> "cursor", Encode.string cursor

[<RequireQualifiedAccess>]
module Filter =
    [<Literal>]
    let Url = "/assets/list"

    type Request = {
        Filters : AssetFilter seq
        Options : AssetFilterQuery seq
    } with
        member this.Encoder =
            Encode.object [
                yield "filter", Encode.object [
                    yield! this.Filters |> Seq.map AssetFilter.Render
                ]
                yield! this.Options |> Seq.map AssetFilterQuery.Render
            ]

    let filterCore (options: AssetFilterQuery seq) (filters: AssetFilter seq)(fetch: HttpHandler<HttpResponseMessage, Stream, 'a>) =
        let decoder = Encode.decodeResponse Assets.AssetListResponse.Decoder id
        let request : Request = {
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
    /// Retrieves list of assets matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="options">Optional limit and cursor</param>
    /// <param name="filters">Search filters</param>
    /// <param name="next">Async handler to use</param>
    /// <returns>List of assets matching given filters and optional cursor</returns>
    let filter (options: AssetFilterQuery seq) (filters: AssetFilter seq) (next: NextHandler<Assets.AssetListResponse,'a>)
        : HttpContext -> Async<Context<'a>> =
            filterCore options filters fetch next

    /// <summary>
    /// Retrieves list of assets matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="options">Optional limit and cursor</param>
    /// <param name="filters">Search filters</param>
    /// <returns>List of assets matching given filters and optional cursor</returns>
    let filterAsync (options: AssetFilterQuery seq) (filters: AssetFilter seq)
        : HttpContext -> Async<Context<Assets.AssetListResponse>> =
            filterCore options filters fetch Async.single


namespace CogniteSdk

open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading


open Oryx
open CogniteSdk.Assets

[<Extension>]
type FilterAssetsExtensions =
    /// <summary>
    /// Retrieves list of assets matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="options">Optional limit and cursor</param>
    /// <param name="filters">Search filters</param>
    /// <returns>List of assets matching given filters and optional cursor</returns>
    [<Extension>]
    static member FilterAsync (this: ClientExtensions.Assets, options: AssetFilterQuery seq, filters: AssetFilter seq, [<Optional>] token: CancellationToken) =
        async {
            let! ctx = Filter.filterAsync options filters this.Ctx
            match ctx.Result with
            | Ok assets ->
                return {|
                        NextCursor = assets.NextCursor
                        Items = assets.Items |> Seq.map (fun asset -> asset.ToEntity ())
                    |}
            | Error error ->
                let err = error2Exception error
                return raise err
        } |> fun op -> Async.StartAsTask(op, cancellationToken = token)
