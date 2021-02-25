// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

/// handler functions for the Beta SDK.
namespace Oryx.Cognite.Beta

open System.Collections.Generic
open System.Net.Http

open Oryx
open Oryx.Cognite

open CogniteSdk

/// Oryx HTTP handlers for specific use within the Cognite SDK
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<AutoOpen>]
module Handler =
    let withBetaHeader<'TResult> : HttpHandler<unit> =
        withHeader "version" "beta"

    let withBetaVersion<'TResult> : HttpHandler<unit> =
        withBetaHeader<'TResult>
        >=> withVersion V10

    let get (url: string) : HttpHandler<unit> =
        withBetaVersion
        >=> get url

    let inline getById (id: int64) (url: string) : HttpHandler<unit> =
        url +/ sprintf "%d" id |> get

    let getWithQuery<'TNext> (query: IQueryParams) (url: string) : HttpHandler<unit, ItemsWithCursor<'TNext>> =
        withBetaVersion
        >=> getWithQuery query url

    let post<'TSource, 'TResult> (content: 'TSource) (url: string) : HttpHandler<unit, 'TResult> =
        withBetaVersion
        >=> post content url

    let postWithQuery<'TSource, 'TResult> (content: 'TSource) (query: IQueryParams) (url: string) : HttpHandler<unit, 'TResult> =
        withBetaVersion
        >=> postWithQuery content query url

    let inline list (content: 'TSource) (url: string) : HttpHandler<unit, 'TResult> =
        withCompletion HttpCompletionOption.ResponseHeadersRead
        >=> post content (url +/ "list")

    let inline count (content: 'TSource) (url: string) : HttpHandler<unit, int> =
        let url =  url +/ "count"

        withCompletion HttpCompletionOption.ResponseHeadersRead
        >=> post<'TSource, AggregateCount> content url
        >=> Handler.map (fun item -> item.Count)

    let search<'TSource, 'TResult> (content: 'TSource) (url: string) : HttpHandler<unit, IEnumerable<'TResult>> =
        let url = url +/ "search"

        withCompletion HttpCompletionOption.ResponseHeadersRead
        >=> post<'TSource, ItemsWithoutCursor<'TResult>> content url
        >=> Handler.map (fun ret -> ret.Items)

    let update<'TSource, 'TResult> (items: IEnumerable<UpdateItem<'TSource>>) (url: string) : HttpHandler<unit, IEnumerable<'TResult>> =
        req {
            let url = url +/ "update"
            let request = ItemsWithoutCursor<UpdateItem<'TSource>>(Items = items)
            let! ret = post<ItemsWithoutCursor<UpdateItem<'TSource>>, ItemsWithoutCursor<'TResult>> request url
            return ret.Items
        }

    let retrieve<'T> (ids: Identity seq) (url: string) : HttpHandler<unit, IEnumerable<'T>> =
        let url = url +/ "byids"
        let request = ItemsWithoutCursor<Identity>(Items = ids)

        withCompletion HttpCompletionOption.ResponseHeadersRead
        >=> post<ItemsWithoutCursor<Identity>, ItemsWithoutCursor<'T>> request url
        >=> Handler.map (fun ret -> ret.Items)

    let create<'TSource, 'TResult> (content: IEnumerable<'TSource>) (url: string) : HttpHandler<unit, IEnumerable<'TResult>> =
        req {
            let content' = ItemsWithoutCursor(Items=content)
            let! ret = post<ItemsWithoutCursor<'TSource>, ItemsWithoutCursor<'TResult>> content' url
            return ret.Items
        }

    let createWithQuery<'TSource, 'TResult> (content: IEnumerable<'TSource>) (query: IQueryParams) (url: string) : HttpHandler<unit, IEnumerable<'TResult>> =
        req {
            let content' = ItemsWithoutCursor(Items=content)
            let! ret = postWithQuery<ItemsWithoutCursor<'TSource>, ItemsWithoutCursor<'TResult>> content' query url
            return ret.Items
        }

    let inline delete<'T> (content: 'T) (url: string) : HttpHandler<unit> =
        url +/ "delete" |> post content
