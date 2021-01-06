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
    let withBetaHeader<'TResult> : HttpHandler<unit, 'TResult> =
        withHeader<'TResult> "version" "beta"

    let withBetaVersion<'TResult> : HttpHandler<unit, 'TResult> =
        withBetaHeader<'TResult>
        >=> withVersion V10

    let get (url: string) : HttpHandler<unit, 'TResult> =
        withBetaVersion
        >=> get url

    let inline getById (id: int64) (url: string) : HttpHandler<unit, 'TNext> =
        url +/ sprintf "%d" id |> get

    let getWithQuery<'TNext, 'TResult> (query: IQueryParams) (url: string) : HttpHandler<unit, ItemsWithCursor<'TNext>, 'TResult> =
        withBetaVersion
        >=> getWithQuery query url

    let post<'T, 'TNext, 'TResult> (content: 'T) (url: string) : HttpHandler<unit, 'TNext, 'TResult> =
        withBetaVersion
        >=> post content url

    let postWithQuery<'T, 'TNext, 'TResult> (content: 'T) (query: IQueryParams) (url: string) : HttpHandler<unit, 'TNext, 'TResult> =
        withBetaVersion
        >=> postWithQuery content query url

    let inline list (content: 'T) (url: string) : HttpHandler<unit, 'TNext, 'TResult> =
        withCompletion HttpCompletionOption.ResponseHeadersRead
        >=> post content (url +/ "list")

    let inline count (content: 'T) (url: string) : HttpHandler<unit, int, 'TResult> =
        let url =  url +/ "count"

        withCompletion HttpCompletionOption.ResponseHeadersRead
        >=> post<'T, AggregateCount, 'TResult> content url
        >=> Handler.map (fun item -> item.Count)

    let search<'T, 'TNext, 'TResult> (content: 'T) (url: string) : HttpHandler<unit, IEnumerable<'TNext>, 'TResult> =
        let url = url +/ "search"

        withCompletion HttpCompletionOption.ResponseHeadersRead
        >=> post<'T, ItemsWithoutCursor<'TNext>, 'TResult> content url
        >=> Handler.map (fun ret -> ret.Items)

    let update<'T, 'TNext, 'TResult> (items: IEnumerable<UpdateItem<'T>>) (url: string) : HttpHandler<unit, IEnumerable<'TNext>, 'TResult> =
        req {
            let url = url +/ "update"
            let request = ItemsWithoutCursor<UpdateItem<'T>>(Items = items)
            let! ret = post<ItemsWithoutCursor<UpdateItem<'T>>, ItemsWithoutCursor<'TNext>, 'TResult> request url
            return ret.Items
        }

    let retrieve<'T, 'TNext> (ids: Identity seq) (url: string) : HttpHandler<unit, IEnumerable<'T>, 'TNext> =
        let url = url +/ "byids"
        let request = ItemsWithoutCursor<Identity>(Items = ids)

        withCompletion HttpCompletionOption.ResponseHeadersRead
        >=> post<ItemsWithoutCursor<Identity>, ItemsWithoutCursor<'T>, 'TNext> request url
        >=> Handler.map (fun ret -> ret.Items)

    let create<'T, 'TNext, 'TResult> (content: IEnumerable<'T>) (url: string) : HttpHandler<unit, IEnumerable<'TNext>, 'TResult> =
        req {
            let content' = ItemsWithoutCursor(Items=content)
            let! ret = post<ItemsWithoutCursor<'T>, ItemsWithoutCursor<'TNext>, 'TResult> content' url
            return ret.Items
        }

    let createWithQuery<'T, 'TNext, 'TResult> (content: IEnumerable<'T>) (query: IQueryParams) (url: string) : HttpHandler<unit, IEnumerable<'TNext>, 'TResult> =
        req {
            let content' = ItemsWithoutCursor(Items=content)
            let! ret = postWithQuery<ItemsWithoutCursor<'T>, ItemsWithoutCursor<'TNext>, 'TResult> content' query url
            return ret.Items
        }

    let inline delete<'T, 'TNext, 'TResult> (content: 'T) (url: string) : HttpHandler<unit, 'TNext, 'TResult> =
        url +/ "delete" |> post content
