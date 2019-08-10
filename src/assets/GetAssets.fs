namespace Fusion.Assets

open System.IO
open System.Net.Http

open Thoth.Json.Net

open Fusion
open Fusion.Common

module List =
    [<Literal>]
    let Url = "/assets"

    type Assets = {
        Items: ReadDto seq
        NextCursor : string option } with

        static member Decoder : Decoder<Assets> =
            Decode.object (fun get -> {
                Items = get.Required.Field "items" (Decode.list ReadDto.Decoder |> Decode.map seq)
                NextCursor = get.Optional.Field "nextCursor" Decode.string
            })

    // Get parameters
    [<RequireQualifiedAccess>]
    type Option =
        private // Expose members instead for C# interoperability
        | CaseLimit of int
        | CaseCursor of string
        | CaseName of string
        | CaseParentIds of int64 seq
        | CaseRootIds of int64 seq
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

        /// The IDs of the root assets that the assets should be children of.
        static member RootIds ids =
            CaseRootIds ids

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

        /// Number of seconds that have elapsed since 00:00:00
        /// Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus
        /// leap seconds.
        static member MaxCreatedTime time =
            CaseMaxCreatedTime time

        /// Number of seconds that have elapsed since 00:00:00
        /// Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus
        /// leap seconds.
        static member MinLastUpdatedTime time =
            CaseMinLastUpdatedTime time

        /// Number of seconds that have elapsed since 00:00:00
        /// Thursday, 1 January 1970, Coordinated Universal Time (UTC), minus
        /// leap seconds.
        static member MaxLastUpdatedTime time =
            CaseMaxLastUpdatedTime time

        /// External Id provided by client. Unique within the project.
        static member ExternalIdPrefix prefix =
            CaseExternalIdPrefix prefix

        static member Render (this: Option) =
            match this with
            | CaseLimit limit -> "limit", limit.ToString ()
            | CaseCursor cursor -> "cursor", cursor
            | CaseName name -> "name", name
            | CaseParentIds ids -> "parentIds", Encode.int53seq ids |> Encode.stringify
            | CaseRootIds ids -> "rootIds", Encode.int53seq ids |> Encode.stringify
            | CaseSource source -> "source", source
            | CaseRoot root -> "root", root.ToString().ToLower()
            | CaseMinCreatedTime value -> "minCreatedTime", value.ToString ()
            | CaseMaxCreatedTime value -> "maxCreatedTime", value.ToString ()
            | CaseMinLastUpdatedTime value -> "minLastUpdatedTime", value.ToString ()
            | CaseMaxLastUpdatedTime value -> "maxLastUpdatedTime", value.ToString ()
            | CaseExternalIdPrefix externalId -> "externalIdPrefix", externalId

    let listCore (options: Option seq) (fetch: HttpHandler<HttpResponseMessage,Stream, 'a>) =
        let decoder = decodeResponse Assets.Decoder id
        let query = options |> Seq.map Option.Render |> List.ofSeq

        GET
        >=> setVersion V10
        >=> addQuery query
        >=> setResource Url
        >=> fetch
        >=> decoder

    /// <summary>
    /// List all assets in the given project. If given limit or 1000 results are exceeded, return a cursor to paginate through results.
    ///
    /// You can retrieve a subset of assets by supplying additional fields; Only assets satisfying all criteria will be
    /// returned.
    /// </summary>
    /// <param name="args">The asset argument object containing parameters to get used for the asset query.</param>
    /// <param name="next">Async handler to use.</param>
    /// <returns>List of assets and optional cursor.</returns>
    let list (options: Option seq) (next: NextHandler<Assets,'a>) : HttpContext -> Async<Context<'a>> =
        listCore options fetch next

    /// <summary>
    /// List all assets in the given project. If given limit or 1000 results are exceeded, return a cursor to paginate through results.
    ///
    /// You can retrieve a subset of assets by supplying additional fields; Only assets satisfying all criteria will be
    /// returned.
    /// </summary>
    /// <param name="args">The asset argument object containing parameters to get used for the asset query.</param>
    /// <returns>List of assets and optional cursor.</returns>
    let listAsync (options: Option seq) : HttpContext -> Async<Context<Assets>> =
        listCore options fetch Async.single

namespace Fusion

open System
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading

open Fusion.Assets
open Fusion.Common

[<Extension>]
type ListAssetsClientExtension =
    /// <summary>
    /// List all assets in the given project. If given limit or 1000 results are exceeded, return a cursor to paginate through results.
    ///
    /// You can retrieve a subset of assets by supplying additional fields; Only assets satisfying all criteria will be
    /// returned.
    /// </summary>
    /// <param name="args">The asset argument object containing parameters to get used for the asset query.</param>
    /// <returns>List of assets and optional cursor.</returns>
    [<Extension>]
    static member ListAsync (this: ClientExtensions.Assets, args: List.Option seq, [<Optional>] token: CancellationToken) =
        async {
            let! ctx = List.listAsync args this.Ctx
            match ctx.Result with
            | Ok assets ->
                return {|
                        NextCursor = if assets.NextCursor.IsSome then assets.NextCursor.Value else String.Empty
                        Items = assets.Items |> Seq.map (fun asset -> asset.ToPoco ())
                    |}
            | Error error ->
                let err = error2Exception error
                return raise err
        } |> fun op -> Async.StartAsTask(op, cancellationToken = token)
