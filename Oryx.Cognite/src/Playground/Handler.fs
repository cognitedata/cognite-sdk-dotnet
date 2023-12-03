// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

/// Common types for the SDK.
namespace Oryx.Cognite.Playground

open System.Collections.Generic
open System.Net.Http

open Oryx
open Oryx.Cognite
open Oryx.SystemTextJson
open Oryx.SystemTextJson.ResponseReader

open CogniteSdk

/// Oryx HTTP handlers for specific use within the Cognite SDK
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<AutoOpen>]
module Handler =
    let getPlayground<'TResult> (url: string) (source: HttpHandler<unit>) : HttpHandler<'TResult> =
        source |> withVersion Playground |> get url

    let inline getByIdz<'TResult> (id: int64) (url: string) (source: HttpHandler<unit>) : HttpHandler<'TResult> =
        let url = url +/ sprintf "%d" id
        source |> getPlayground url

    let getWithQuery<'a, 'b>
        (query: IQueryParams)
        (url: string)
        (source: HttpHandler<unit>)
        : HttpHandler<ItemsWithCursor<'a>> =
        let parms = query.ToQueryParams()

        source
        |> GET
        |> withVersion Playground
        |> withResource url
        |> withQuery parms
        |> fetch
        |> withError decodeError
        |> json jsonOptions
        |> log

    let post<'TContent, 'TResult>
        (content: 'TContent)
        (url: string)
        (source: HttpHandler<unit>)
        : HttpHandler<'TResult> =
        source
        |> POST
        |> withResource url
        |> withContent (fun () -> new JsonPushStreamContent<'TContent>(content, jsonOptions) :> _)
        |> fetch
        |> withError decodeError
        |> json jsonOptions
        |> log

    let postPlayground<'TContent, 'TResult>
        (content: 'TContent)
        (url: string)
        (source: HttpHandler<unit>)
        : HttpHandler<'TResult> =
        source |> withVersion Playground |> post content url

    let postWithQuery<'TContent, 'TResult>
        (content: 'TContent)
        (query: IQueryParams)
        (url: string)
        (source: HttpHandler<unit>)
        : HttpHandler<'TResult> =
        let parms = query.ToQueryParams()

        source
        |> POST
        |> withVersion Playground
        |> withResource url
        |> withQuery parms
        |> withContent (fun () -> new JsonPushStreamContent<'TContent>(content, jsonOptions) :> _)
        |> fetch
        |> withError decodeError
        |> json jsonOptions
        |> log

    let inline list (content: 'TContent) (url: string) (source: HttpHandler<unit>) : HttpHandler<'TResult> =
        source
        |> withCompletion HttpCompletionOption.ResponseHeadersRead
        |> postPlayground content (url +/ "list")

    let inline count (content: 'a) (url: string) (source: HttpHandler<unit>) : HttpHandler<int> =
        http {
            let url = url +/ "count"

            let! item =
                source
                |> withCompletion HttpCompletionOption.ResponseHeadersRead
                |> postPlayground<'a, AggregateCount> content url

            return item.Count
        }

    let search<'a, 'b> (content: 'a) (url: string) (source: HttpHandler<unit>) : HttpHandler<IEnumerable<'b>> =
        http {
            let url = url +/ "search"

            let! ret =
                source
                |> withCompletion HttpCompletionOption.ResponseHeadersRead
                |> postPlayground<'a, ItemsWithoutCursor<'b>> content url

            return ret.Items
        }

    let searchPlayground<'TContent, 'TResult>
        (content: 'TContent)
        (url: string)
        (source: HttpHandler<unit>)
        : HttpHandler<'TResult> =
        http {
            let url = url +/ "search"

            let! ret =
                source
                |> withCompletion HttpCompletionOption.ResponseHeadersRead
                |> postPlayground<'TContent, 'TResult> content url

            return ret
        }

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
                |> postPlayground<ItemsWithoutCursor<UpdateItem<'TContent>>, ItemsWithoutCursor<'TResult>> request url

            return ret.Items
        }

    let retrieve<'TContent, 'TResult>
        (ids: Identity seq)
        (url: string)
        (source: HttpHandler<unit>)
        : HttpHandler<IEnumerable<'TResult>> =
        http {
            let url = url +/ "byids"
            let request = ItemsWithoutCursor<Identity>(Items = ids)

            let! ret =
                source
                |> withCompletion HttpCompletionOption.ResponseHeadersRead
                |> postPlayground<ItemsWithoutCursor<Identity>, ItemsWithoutCursor<'TResult>> request url

            return ret.Items
        }

    let create<'TContent, 'TResult>
        (content: IEnumerable<'TContent>)
        (url: string)
        (source: HttpHandler<unit>)
        : HttpHandler<IEnumerable<'TResult>> =
        http {
            let content' = ItemsWithoutCursor(Items = content)

            let! ret =
                source
                |> postPlayground<ItemsWithoutCursor<'TContent>, ItemsWithoutCursor<'TResult>> content' url

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

    let inline delete<'TContent, 'TResult>
        (content: 'TContent)
        (url: string)
        (source: HttpHandler<unit>)
        : HttpHandler<'TResult> =
        let url = url +/ "delete"
        source |> postPlayground content url
