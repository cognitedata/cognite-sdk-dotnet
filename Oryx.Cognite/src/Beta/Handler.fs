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
    let withBetaHeader<'T> (source: HttpHandler<'T>) : HttpHandler<'T> =
        source |> withHeader ("cdf-version", "beta")

    let withBetaVersion<'TSource> (source: HttpHandler<'TSource>) : HttpHandler<'TSource> =
        source |> withBetaHeader<'TSource> |> withVersion V10

    let withAlphaHeader<'T> (source: HttpHandler<'T>) : HttpHandler<'T> =
        source |> withHeader ("cdf-version", "alpha")

    let withAlphaVersion<'TSource> (source: HttpHandler<'TSource>) : HttpHandler<'TSource> =
        source |> withAlphaHeader<'TSource> |> withVersion V10

    let get (url: string) (source: HttpHandler<unit>) : HttpHandler<unit> = source |> withBetaVersion |> get url

    let inline getById (id: int64) (url: string) (source: HttpHandler<unit>) : HttpHandler<unit> =
        let url = url +/ sprintf "%d" id
        source |> get url

    let getWithQuery<'TResult>
        (query: IQueryParams)
        (url: string)
        (source: HttpHandler<unit>)
        : HttpHandler<ItemsWithCursor<'TResult>> =
        source |> withBetaVersion |> getWithQuery query url

    let post<'TContent, 'TResult>
        (content: 'TContent)
        (url: string)
        (source: HttpHandler<unit>)
        : HttpHandler<'TResult> =
        source |> withBetaVersion |> post<'TContent, 'TResult> content url

    let postWithQuery<'TContent, 'TResult>
        (content: 'TContent)
        (query: IQueryParams)
        (url: string)
        (source: HttpHandler<unit>)
        : HttpHandler<'TResult> =
        source
        |> withBetaVersion
        |> postWithQuery<'TContent, 'TResult> content query url jsonOptions

    let inline list (content: 'TSource) (url: string) (source: HttpHandler<unit>) : HttpHandler<'TResult> =
        source
        |> withCompletion HttpCompletionOption.ResponseHeadersRead
        |> post content (url +/ "list")

    let inline count (content: 'TSource) (url: string) (source: HttpHandler<unit>) : HttpHandler<int> =
        let url = url +/ "count"

        source
        |> withCompletion HttpCompletionOption.ResponseHeadersRead
        |> post<'TSource, AggregateCount> content url
        |> HttpHandler.map (fun item -> item.Count)

    let search<'TSource, 'TResult>
        (content: 'TSource)
        (url: string)
        (source: HttpHandler<unit>)
        : HttpHandler<IEnumerable<'TResult>> =
        let url = url +/ "search"

        source
        |> withCompletion HttpCompletionOption.ResponseHeadersRead
        |> post<'TSource, ItemsWithoutCursor<'TResult>> content url
        |> HttpHandler.map (fun ret -> ret.Items)

    let update<'TContent, 'TResult>
        (items: IEnumerable<UpdateItem<'TContent>>)
        (url: string)
        (source: HttpHandler<unit>)
        : HttpHandler<IEnumerable<'TResult>> =
        http {
            let url = url +/ "update"
            let request = ItemsWithoutCursor<UpdateItem<'TContent>>(Items = items)

            let! ret =
                source
                |> post<ItemsWithoutCursor<UpdateItem<'TContent>>, ItemsWithoutCursor<'TResult>> request url

            return ret.Items
        }

    let retrieve<'TResult>
        (ids: Identity seq)
        (url: string)
        (source: HttpHandler<unit>)
        : HttpHandler<IEnumerable<'TResult>> =
        let url = url +/ "byids"
        let request = ItemsWithoutCursor<Identity>(Items = ids)

        source
        |> withCompletion HttpCompletionOption.ResponseHeadersRead
        |> post<ItemsWithoutCursor<Identity>, ItemsWithoutCursor<'TResult>> request url
        |> HttpHandler.map (fun ret -> ret.Items)

    let create<'TSource, 'TResult>
        (content: IEnumerable<'TSource>)
        (url: string)
        (source: HttpHandler<unit>)
        : HttpHandler<IEnumerable<'TResult>> =
        http {
            let content' = ItemsWithoutCursor(Items = content)

            let! ret =
                source
                |> post<ItemsWithoutCursor<'TSource>, ItemsWithoutCursor<'TResult>> content' url

            return ret.Items
        }

    let createWithQuery<'TContent, 'TResult>
        (content: IEnumerable<'TContent>)
        (query: IQueryParams)
        (url: string)
        (source: HttpHandler<unit>)
        : HttpHandler<IEnumerable<'TResult>> =
        http {
            let content' = ItemsWithoutCursor(Items = content)

            let! ret =
                source
                |> postWithQuery<ItemsWithoutCursor<'TContent>, ItemsWithoutCursor<'TResult>> content' query url

            return ret.Items
        }

    let inline delete<'T> (content: 'T) (url: string) (source: HttpHandler<unit>) : HttpHandler<unit> =
        let url = url +/ "delete"
        source |> post content url
