// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

/// handler functions for the Beta SDK.
namespace Oryx.Cognite.Beta

open Oryx.Cognite

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
    let withBeta (next: NextFunc<_,_>) (context: HttpContext) =
        withVersion
            V10
            next
            { context with Request = { context.Request with Headers = context.Request.Headers.Add("version", "beta") } }

    let getBeta<'a, 'b> (url: string)  : HttpHandler<HttpResponseMessage, 'a, 'b> =
        withBeta  >=> get url

    let inline getById (id: int64) (url: string) : HttpHandler<HttpResponseMessage, 'a, 'b> =
        url +/ sprintf "%d" id |> getBeta

    let getWithQuery<'a, 'b> (query: IQueryParams) (url: string) : HttpHandler<HttpResponseMessage, ItemsWithCursor<'a>, 'b> =
        let parms = query.ToQueryParams ()
        GET
        >=> withBeta
        >=> withResource url
        >=> withQuery parms
        >=> fetch
        >=> log
        >=> withError decodeError
        >=> json jsonOptions

    let post<'a, 'b, 'c> (content: 'a) (url: string) : HttpHandler<HttpResponseMessage, 'b, 'c> =
        POST
        >=> withResource url
        >=> withContent (fun () -> new JsonPushStreamContent<'a>(content, jsonOptions) :> _)
        >=> fetch
        >=> log
        >=> withError decodeError
        >=> json jsonOptions

    let postBeta<'a, 'b, 'c> (content: 'a) (url: string) : HttpHandler<HttpResponseMessage, 'b, 'c> =
        withBeta  >=> post content url

    let postWithQuery<'a, 'b, 'c> (content: 'a) (query: IQueryParams) (url: string) : HttpHandler<HttpResponseMessage, 'b, 'c> =
        let parms = query.ToQueryParams ()

        POST
        >=> withBeta
        >=> withResource url
        >=> withQuery parms
        >=> withContent (fun () -> new JsonPushStreamContent<'a>(content, jsonOptions) :> _)
        >=> fetch
        >=> log
        >=> withError decodeError
        >=> json jsonOptions

    let inline list (content: 'a) (url: string) : HttpHandler<HttpResponseMessage, 'b, 'c> =
        withCompletion HttpCompletionOption.ResponseHeadersRead
        >=> postBeta  content (url +/ "list")

    let inline count (content: 'a) (url: string) : HttpHandler<HttpResponseMessage, int, 'c> =
        req {
            let url =  url +/ "count"
            let! item =
                withCompletion HttpCompletionOption.ResponseHeadersRead
                >=> postBeta<'a, AggregateCount, 'c> content url
            return item.Count
        }

    let search<'a, 'b, 'c> (content: 'a) (url: string) : HttpHandler<HttpResponseMessage, IEnumerable<'b>, 'c> =
        req {
            let url = url +/ "search"
            let! ret =
                withCompletion HttpCompletionOption.ResponseHeadersRead
                >=> postBeta<'a, ItemsWithoutCursor<'b>, 'c> content url
            return ret.Items
        }

    let searchPlayground<'a, 'b, 'c> (content: 'a) (url: string) : HttpHandler<HttpResponseMessage, 'b, 'c> =
        req {
            let url = url +/ "search"
            let! ret =
                withCompletion HttpCompletionOption.ResponseHeadersRead
                >=> postBeta<'a, 'b, 'c> content url
            return ret
        }

    let update<'a, 'b, 'c> (items: IEnumerable<UpdateItem<'a>>) (url: string) : HttpHandler<HttpResponseMessage, IEnumerable<'b>, 'c> =
        req {
            let url = url +/ "update"
            let request = ItemsWithoutCursor<UpdateItem<'a>>(Items = items)
            let! ret = postBeta<ItemsWithoutCursor<UpdateItem<'a>>, ItemsWithoutCursor<'b>, 'c> request url
            return ret.Items
        }

    let retrieve<'a, 'b> (ids: Identity seq) (url: string) : HttpHandler<HttpResponseMessage, IEnumerable<'a>, 'b> =
        req {
            let url = url +/ "byids"
            let request = ItemsWithoutCursor<Identity>(Items = ids)
            let! ret =
                withCompletion HttpCompletionOption.ResponseHeadersRead
                >=> postBeta<ItemsWithoutCursor<Identity>, ItemsWithoutCursor<'a>, 'b> request url
            return ret.Items
        }

    let create<'a, 'b, 'c> (content: IEnumerable<'a>) (url: string) : HttpHandler<HttpResponseMessage, IEnumerable<'b>, 'c> =
        req {
            let content' = ItemsWithoutCursor(Items=content)
            let! ret = postBeta<ItemsWithoutCursor<'a>, ItemsWithoutCursor<'b>, 'c> content' url
            return ret.Items
        }

    let createWithQuery<'a, 'b, 'c> (content: IEnumerable<'a>) (query: IQueryParams) (url: string) : HttpHandler<HttpResponseMessage, IEnumerable<'b>, 'c> =
        req {
            let content' = ItemsWithoutCursor(Items=content)
            let! ret = postWithQuery<ItemsWithoutCursor<'a>, ItemsWithoutCursor<'b>, 'c> content' query url
            return ret.Items
        }

    let inline delete<'a, 'b, 'c> (content: 'a) (url: string) : HttpHandler<HttpResponseMessage, 'b, 'c> =
        url +/ "delete" |> postBeta content
