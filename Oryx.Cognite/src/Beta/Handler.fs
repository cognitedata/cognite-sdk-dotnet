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
    let withBetaHeader<'TResult> : HttpHandler<HttpResponseMessage, 'TResult> =
        withHeader<'TResult> "version" "beta"

    let withBetaVersion<'TResult> : HttpHandler<HttpResponseMessage, 'TResult> =
        withBetaHeader<'TResult>
        >=> withVersion V10

    let get (url: string) : HttpHandler<HttpResponseMessage, 'TResult> =
        withBetaVersion
        >=> get url

    let inline getById (id: int64) (url: string) : HttpHandler<HttpResponseMessage, 'TNext> =
        url +/ sprintf "%d" id |> get

    let getWithQuery<'TNext, 'TResult> (query: IQueryParams) (url: string) : HttpHandler<HttpResponseMessage, ItemsWithCursor<'TNext>, 'TResult> =
        withBetaVersion
        >=> getWithQuery query url

    let post<'T, 'TNext, 'TResult> (content: 'T) (url: string) : HttpHandler<HttpResponseMessage, 'TNext, 'TResult> =
        withBetaVersion
        >=> post content url

    let postWithQuery<'T, 'TNext, 'TResult> (content: 'T) (query: IQueryParams) (url: string) : HttpHandler<HttpResponseMessage, 'TNext, 'TResult> =
        withBetaVersion
        >=> postWithQuery content query url

    let inline list (content: 'T) (url: string) : HttpHandler<HttpResponseMessage, 'TNext, 'TResult> =
        withCompletion HttpCompletionOption.ResponseHeadersRead
        >=> post content (url +/ "list")

    let inline count (content: 'T) (url: string) : HttpHandler<HttpResponseMessage, int, 'TResult> =
        req {
            let url =  url +/ "count"
            let! item =
                withCompletion HttpCompletionOption.ResponseHeadersRead
                >=> post<'T, AggregateCount, 'TResult> content url
            return item.Count
        }

    let search<'T, 'TNext, 'TResult> (content: 'T) (url: string) : HttpHandler<HttpResponseMessage, IEnumerable<'TNext>, 'TResult> =
        req {
            let url = url +/ "search"
            let! ret =
                withCompletion HttpCompletionOption.ResponseHeadersRead
                >=> post<'T, ItemsWithoutCursor<'TNext>, 'TResult> content url
            return ret.Items
        }

    let searchPlayground<'T, 'TNext, 'TResult> (content: 'T) (url: string) : HttpHandler<HttpResponseMessage, 'TNext, 'TResult> =
        req {
            let url = url +/ "search"
            let! ret =
                withCompletion HttpCompletionOption.ResponseHeadersRead
                >=> post<'T, 'TNext, 'TResult> content url
            return ret
        }

    let update<'T, 'TNext, 'TResult> (items: IEnumerable<UpdateItem<'T>>) (url: string) : HttpHandler<HttpResponseMessage, IEnumerable<'TNext>, 'TResult> =
        req {
            let url = url +/ "update"
            let request = ItemsWithoutCursor<UpdateItem<'T>>(Items = items)
            let! ret = post<ItemsWithoutCursor<UpdateItem<'T>>, ItemsWithoutCursor<'TNext>, 'TResult> request url
            return ret.Items
        }

    let retrieve<'T, 'TNext> (ids: Identity seq) (url: string) : HttpHandler<HttpResponseMessage, IEnumerable<'T>, 'TNext> =
        let url = url +/ "byids"
        let request = ItemsWithoutCursor<Identity>(Items = ids)

        withCompletion HttpCompletionOption.ResponseHeadersRead
        >=> post<ItemsWithoutCursor<Identity>, ItemsWithoutCursor<'T>, 'TNext> request url
        >=> Handler.map (fun ret -> ret.Items)

    let create<'T, 'TNext, 'TResult> (content: IEnumerable<'T>) (url: string) : HttpHandler<HttpResponseMessage, IEnumerable<'TNext>, 'TResult> =
        req {
            let content' = ItemsWithoutCursor(Items=content)
            let! ret = post<ItemsWithoutCursor<'T>, ItemsWithoutCursor<'TNext>, 'TResult> content' url
            return ret.Items
        }

    let createWithQuery<'T, 'TNext, 'TResult> (content: IEnumerable<'T>) (query: IQueryParams) (url: string) : HttpHandler<HttpResponseMessage, IEnumerable<'TNext>, 'TResult> =
        req {
            let content' = ItemsWithoutCursor(Items=content)
            let! ret = postWithQuery<ItemsWithoutCursor<'T>, ItemsWithoutCursor<'TNext>, 'TResult> content' query url
            return ret.Items
        }

    let inline delete<'T, 'TNext, 'TResult> (content: 'T) (url: string) : HttpHandler<HttpResponseMessage, 'TNext, 'TResult> =
        url +/ "delete" |> post content
