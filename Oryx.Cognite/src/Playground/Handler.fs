// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

/// Common types for the SDK.
namespace Oryx.Cognite.Playground

open System.Collections.Generic
open System.Net.Http

open Oryx
open Oryx.SystemTextJson
open Oryx.SystemTextJson.ResponseReader

open Oryx.Cognite

open CogniteSdk

/// Oryx HTTP handlers for specific use within the Cognite SDK
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<AutoOpen>]
module Handler =
    let getPlayground<'a, 'b> (url: string)  : HttpHandler<unit, 'a> =
        withVersion Playground >=> get url

    let inline getById (id: int64) (url: string) : HttpHandler<unit, 'a> =
        url +/ sprintf "%d" id |> getPlayground

    let getWithQuery<'a, 'b> (query: IQueryParams) (url: string) : HttpHandler<unit, ItemsWithCursor<'a>> =
        let parms = query.ToQueryParams ()
        GET
        >=> withVersion Playground
        >=> withResource url
        >=> withQuery parms
        >=> fetch
        >=> log
        >=> withError decodeError
        >=> json jsonOptions

    let post<'a, 'b> (content: 'a) (url: string) : HttpHandler<unit, 'b> =
        POST
        >=> withResource url
        >=> withContent (fun () -> new JsonPushStreamContent<'a>(content, jsonOptions) :> _)
        >=> fetch
        >=> log
        >=> withError decodeError
        >=> json jsonOptions

    let postPlayground<'a, 'b> (content: 'a) (url: string) : HttpHandler<unit, 'b> =
        withVersion Playground >=> post content url

    let postWithQuery<'a, 'b> (content: 'a) (query: IQueryParams) (url: string) : HttpHandler<unit, 'b> =
        let parms = query.ToQueryParams ()

        POST
        >=> withVersion Playground
        >=> withResource url
        >=> withQuery parms
        >=> withContent (fun () -> new JsonPushStreamContent<'a>(content, jsonOptions) :> _)
        >=> fetch
        >=> log
        >=> withError decodeError
        >=> json jsonOptions

    let inline list (content: 'a) (url: string) : HttpHandler<unit, 'b> =
        withCompletion HttpCompletionOption.ResponseHeadersRead
        >=> postPlayground  content (url +/ "list")

    let inline count (content: 'a) (url: string) : HttpHandler<unit, int> =
        req {
            let url =  url +/ "count"
            let! item =
                withCompletion HttpCompletionOption.ResponseHeadersRead
                >=> postPlayground<'a, AggregateCount> content url
            return item.Count
        }

    let search<'a, 'b> (content: 'a) (url: string) : HttpHandler<unit, IEnumerable<'b>> =
        req {
            let url = url +/ "search"
            let! ret =
                withCompletion HttpCompletionOption.ResponseHeadersRead
                >=> postPlayground<'a, ItemsWithoutCursor<'b>> content url
            return ret.Items
        }

    let searchPlayground<'a, 'b> (content: 'a) (url: string) : HttpHandler<unit, 'b> =
        req {
            let url = url +/ "search"
            let! ret =
                withCompletion HttpCompletionOption.ResponseHeadersRead
                >=> postPlayground<'a, 'b> content url
            return ret
        }

    let update<'a, 'b> (items: IEnumerable<UpdateItem<'a>>) (url: string) : HttpHandler<unit, IEnumerable<'b>> =
        req {
            let url = url +/ "update"
            let request = ItemsWithoutCursor<UpdateItem<'a>>(Items = items)
            let! ret = postPlayground<ItemsWithoutCursor<UpdateItem<'a>>, ItemsWithoutCursor<'b>> request url
            return ret.Items
        }

    let retrieve<'a, 'b> (ids: Identity seq) (url: string) : HttpHandler<unit, IEnumerable<'a>> =
        req {
            let url = url +/ "byids"
            let request = ItemsWithoutCursor<Identity>(Items = ids)
            let! ret =
                withCompletion HttpCompletionOption.ResponseHeadersRead
                >=> postPlayground<ItemsWithoutCursor<Identity>, ItemsWithoutCursor<'a>> request url
            return ret.Items
        }

    let create<'a, 'b> (content: IEnumerable<'a>) (url: string) : HttpHandler<unit, IEnumerable<'b>> =
        req {
            let content' = ItemsWithoutCursor(Items=content)
            let! ret = postPlayground<ItemsWithoutCursor<'a>, ItemsWithoutCursor<'b>> content' url
            return ret.Items
        }

    let createWithQuery<'a, 'b> (content: IEnumerable<'a>) (query: IQueryParams) (url: string) : HttpHandler<unit, IEnumerable<'b>> =
        req {
            let content' = ItemsWithoutCursor(Items=content)
            let! ret = postWithQuery<ItemsWithoutCursor<'a>, ItemsWithoutCursor<'b>> content' query url
            return ret.Items
        }

    let inline delete<'a, 'b> (content: 'a) (url: string) : HttpHandler<unit, 'b> =
        url +/ "delete" |> postPlayground content
