namespace CogniteSdk.Assets

open System.IO
open System.Net.Http


open Oryx
open Thoth.Json.Net

open CogniteSdk


[<RequireQualifiedAccess>]
module Filter =
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

    type Request = {
        Filters : FilterOption seq
        Options : Option seq
    } with
        member this.Encoder =
            Encode.object [
                yield "filter", Encode.object [
                    yield! this.Filters |> Seq.map FilterOption.Render
                ]
                yield! this.Options |> Seq.map Option.Render
            ]

    let filterCore (options: Option seq) (filters: FilterOption seq)(fetch: HttpHandler<HttpResponseMessage, Stream, 'a>) =
        let decoder = Encode.decodeResponse List.Assets.Decoder id
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
    let filter (options: Option seq) (filters: FilterOption seq) (next: NextHandler<List.Assets,'a>)
        : HttpContext -> Async<Context<'a>> =
            filterCore options filters fetch next

    /// <summary>
    /// Retrieves list of assets matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="options">Optional limit and cursor</param>
    /// <param name="filters">Search filters</param>
    /// <returns>List of assets matching given filters and optional cursor</returns>
    let filterAsync (options: Option seq) (filters: FilterOption seq)
        : HttpContext -> Async<Context<List.Assets>> =
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
    static member FilterAsync (this: ClientExtensions.Assets, options: Filter.Option seq, filters: FilterOption seq, [<Optional>] token: CancellationToken) =
        async {
            let! ctx = Filter.filterAsync options filters this.Ctx
            match ctx.Result with
            | Ok assets ->
                return {|
                        NextCursor = assets.NextCursor
                        Items = assets.Items |> Seq.map (fun asset -> asset.ToAsset ())
                    |}
            | Error error ->
                let err = error2Exception error
                return raise err
        } |> fun op -> Async.StartAsTask(op, cancellationToken = token)
