// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

/// Common types for the SDK.
namespace Oryx.Cognite

open System
open System.Collections.Generic
open System.Net.Http
open System.Text.Json
open System.Threading
open System.Threading.Tasks
open System.IO.Compression

open FSharp.Control.Tasks
open Google.Protobuf

open Oryx
open Oryx.SystemTextJson
open Oryx.SystemTextJson.ResponseReader
open Oryx.Protobuf
open Oryx.Protobuf.ResponseReader

open Oryx.Cognite

open CogniteSdk

/// Oryx HTTP handlers for specific use within the Cognite SDK
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<AutoOpen>]
module HttpHandler =
    let withResource (resource: string): IHttpHandler<'TSource> =
        { new IHttpHandler<'TSource> with
            member _.Subscribe(next) =
                { new IHttpNext<'TSource> with
                    member _.OnNextAsync(ctx, content) =
                        next.OnNextAsync(
                            { ctx with
                                Request =
                                    { ctx.Request with
                                        Items = ctx.Request.Items.Add(PlaceHolder.Resource, String resource)
                                    }
                            },
                            content = content
                        )

                    member _.OnErrorAsync(ctx, exn) = next.OnErrorAsync(ctx, exn)
                    member _.OnCompletedAsync(ctx) = next.OnCompletedAsync(ctx)
        }}

    let withVersion (version: ApiVersion): IHttpHandler<'TSource> =
        { new IHttpHandler<'TSource> with
            member _.Subscribe(next) =
                { new IHttpNext<'TSource> with
                    member _.OnNextAsync(ctx, content) =
                        next.OnNextAsync(
                            { ctx with
                                Request =
                                    { ctx.Request with
                                        Items = ctx.Request.Items.Add(PlaceHolder.ApiVersion, String(version.ToString()))
                                    }
                            },
                            content = content
                        )

                    member _.OnErrorAsync(ctx, exn) = next.OnErrorAsync(ctx, exn)
                    member _.OnCompletedAsync(ctx) = next.OnCompletedAsync(ctx)
                }}


    let withUrl (url: string): IHttpHandler<'TSource> =
        let urlBuilder (request: HttpRequest) =
            let extra = request.Items
            let baseUrl =
                match Map.tryFind PlaceHolder.BaseUrl extra with
                | Some (Url url) -> url.ToString ()
                | _ -> "https://api.cognitedata.com"

            if not (Map.containsKey PlaceHolder.HasAppId extra)
            then failwith "Client must set the Application ID (appId)"
            baseUrl +/ url

        { new IHttpHandler<'TSource> with
            member _.Subscribe(next) =
                { new IHttpNext<'TSource> with
                    member _.OnNextAsync(ctx, content) =
                        next.OnNextAsync({ ctx with Request = { ctx.Request with UrlBuilder = urlBuilder } }, content=content)
                    member _.OnErrorAsync(ctx, exn) = next.OnErrorAsync(ctx, exn)
                    member _.OnCompletedAsync(ctx) = next.OnCompletedAsync(ctx)
                }}


    /// Raises error for C# extension methods. Translates Oryx errors into CogniteSdk equivalents so clients don't
    /// need to open the Oryx namespace.
    let raiseError (error: exn) =
        raise error

    /// Composes the given handler token provider with the request. Adapts a C# renewer function to F#.
    let withTokenRenewer<'TResult> (tokenRenewer: Func<CancellationToken, Task<string>>) (handler: IHttpHandler<unit, 'TResult>) =
        let renewer ct = task {
            try
                let! token = tokenRenewer.Invoke ct
                match Option.ofObj token with
                | Some token -> return Ok token
                | None -> return (ArgumentNullException "No token received." :> exn) |> Error
            with
            | ex -> return ex |> Error
        }

        withTokenRenewer renewer >=> handler

    /// Decode response message into a ResponseException.
    let decodeError (response: HttpResponse) (content: HttpContent) : Task<exn> = task {
        let mediaType =
           content.Headers |> Option.ofObj
           |> Option.bind (fun headers -> headers.ContentType |> Option.ofObj)
           |> Option.bind (fun contentType -> contentType.MediaType |> Option.ofObj)
           |> Option.defaultValue String.Empty

        if mediaType.Contains "application/json" then
            use! stream = content.ReadAsStreamAsync ()

            try
                let! error = JsonSerializer.DeserializeAsync<ApiResponseError>(stream, jsonOptions)
                let requestId = response.Headers |> Map.tryFind "X-Request-ID"

                match requestId with
                | Some requestIds ->
                    let requestId = Seq.tryExactlyOne requestIds |> Option.defaultValue String.Empty
                    error.RequestId <- requestId
                | None -> ()

                return error.ToException ()
            with
            | ex ->
                let exn = ResponseException (response.ReasonPhrase, ex)
                exn.Code <- int response.StatusCode
                return exn :> _
        else
            let exn = ResponseException response.ReasonPhrase
            exn.Code <- int response.StatusCode
            return exn :> _
    }

    let getOptions<'TNext, 'TResult> (url: string) (options: JsonSerializerOptions) : IHttpHandler<unit, 'TResult> =
        GET
        >=> withResource url
        >=> fetch
        >=> withError decodeError
        >=> json options
        >=> log

    let get<'TNext, 'TResult> (url: string) : IHttpHandler<unit, 'TResult> =
        getOptions url jsonOptions

    let getV10<'TNext, 'TResult> (url: string)  : IHttpHandler<unit, 'TResult> =
        withVersion V10 >=> get url

    let getV10Options<'TNext, 'TResult> (url: string) (options: JsonSerializerOptions) : IHttpHandler<unit, 'TResult> =
        withVersion V10 >=> getOptions url options

    let inline getById (id: int64) (url: string) : IHttpHandler<unit, 'TResult> =
        url +/ sprintf "%d" id |> getV10

    let getWithQueryOptions<'TResult> (query: IQueryParams) (url: string) (options: JsonSerializerOptions) : IHttpHandler<unit, 'TResult> =
        let parms = query.ToQueryParams ()
        GET
        >=> withVersion V10
        >=> withResource url
        >=> withQuery parms
        >=> fetch
        >=> withError decodeError
        >=> json options
        >=> log

    let getWithQuery<'TResult> (query: IQueryParams) (url: string) : IHttpHandler<unit, 'TResult> =
        getWithQueryOptions query url jsonOptions

    let post<'T, 'TResult> (content: 'T) (url: string) : IHttpHandler<unit, 'TResult> =
        POST
        >=> withResource url
        >=> withContent (fun () -> new JsonPushStreamContent<'T>(content, jsonOptions) :> _)
        >=> fetch
        >=> withError decodeError
        >=> json jsonOptions
        >=> log

    let postV10<'T, 'TResult> (content: 'T) (url: string) : IHttpHandler<unit, 'TResult> =
        withVersion V10 >=> post content url

    let postWithQuery<'T, 'TResult> (content: 'T) (query: IQueryParams) (url: string) (options: JsonSerializerOptions): IHttpHandler<unit, 'TResult> =
        let parms = query.ToQueryParams ()

        POST
        >=> withVersion V10
        >=> withResource url
        >=> withQuery parms
        >=> withContent (fun () -> new JsonPushStreamContent<'T>(content, options) :> _)
        >=> fetch
        >=> withError decodeError
        >=> json options
        >=> log

    let inline list (content: 'T) (url: string) : IHttpHandler<unit, 'TResult> =
        withCompletion HttpCompletionOption.ResponseHeadersRead
        >=> postV10 content (url +/ "list")

    let inline aggregate (content: 'TSource) (url: string) : IHttpHandler<unit, int> =
        req {
            let url =  url +/ "aggregate"
            let! ret =
                withCompletion HttpCompletionOption.ResponseHeadersRead
                >=> postV10<'TSource, ItemsWithoutCursor<AggregateCount>> content url
            let item = ret.Items |> Seq.head
            return item.Count
        }

    let search<'TSource, 'TResult> (content: 'TSource) (url: string) : IHttpHandler<unit, IEnumerable<'TResult>> =
        req {
            let url = url +/ "search"
            let! ret =
                withCompletion HttpCompletionOption.ResponseHeadersRead
                >=> postV10<'TSource, ItemsWithoutCursor<'TResult>> content url
            return ret.Items
        }

    let update<'TSource, 'TResult> (items: IEnumerable<UpdateItem<'TSource>>) (url: string) : IHttpHandler<unit, IEnumerable<'TResult>> =
        req {
            let url = url +/ "update"
            let request = ItemsWithoutCursor<UpdateItem<'TSource>>(Items = items)
            let! ret = postV10<ItemsWithoutCursor<UpdateItem<'TSource>>, ItemsWithoutCursor<'TResult>> request url
            return ret.Items
        }

    let retrieve<'TSource, 'TResult> (ids: Identity seq) (url: string) : IHttpHandler<unit, IEnumerable<'TResult>> =
        req {
            let url = url +/ "byids"
            let request = ItemsWithoutCursor<Identity>(Items = ids)
            let! ret =
                withCompletion HttpCompletionOption.ResponseHeadersRead
                >=> postV10<ItemsWithoutCursor<Identity>, ItemsWithoutCursor<'TResult>> request url
            return ret.Items
        }

    let retrieveIgnoreUnkownIds<'TSource, 'TResult> (ids: Identity seq) (ignoreUnknownIdsOpt: bool option) (url: string) : IHttpHandler<unit, IEnumerable<'TResult>> =
        match ignoreUnknownIdsOpt with
        | Some ignoreUnknownIds ->
            req {
                let url = url +/ "byids"
                let request = ItemsWithIgnoreUnknownIds<Identity>(Items = ids, IgnoreUnknownIds = ignoreUnknownIds)
                let! ret =
                    withCompletion HttpCompletionOption.ResponseHeadersRead
                    >=> postV10<ItemsWithIgnoreUnknownIds<Identity>, ItemsWithoutCursor<'TResult>> request url
                return ret.Items
            }
        | None -> retrieve ids url

    let create<'TSource, 'TResult> (content: IEnumerable<'TSource>) (url: string) : IHttpHandler<unit, IEnumerable<'TResult>> =
        req {
            let content' = ItemsWithoutCursor(Items=content)
            let! ret = postV10<ItemsWithoutCursor<'TSource>, ItemsWithoutCursor<'TResult>> content' url
            return ret.Items
        }

    let createWithQuery<'TSource, 'TResult> (content: IEnumerable<'TSource>) (query: IQueryParams) (url: string) : IHttpHandler<unit, IEnumerable<'TResult>> =
        req {
            let content' = ItemsWithoutCursor(Items=content)
            let! ret = postWithQuery<ItemsWithoutCursor<'TSource>, ItemsWithoutCursor<'TResult>> content' query url jsonOptions
            return ret.Items
        }

    let createWithQueryEmptyOptions<'TSource> (content: IEnumerable<'TSource>) (query: IQueryParams) (url: string) (options: JsonSerializerOptions) : IHttpHandler<unit, EmptyResponse> =
        let content' = ItemsWithoutCursor(Items=content)
        postWithQuery<ItemsWithoutCursor<'TSource>, EmptyResponse> content' query url options

    let createWithQueryEmpty<'TSource> (content: IEnumerable<'TSource>) (query: IQueryParams) (url: string) : IHttpHandler<unit, EmptyResponse> =
        createWithQueryEmptyOptions content query url jsonOptions

    let createEmpty<'TSource> (content: IEnumerable<'TSource>) (url: string) : IHttpHandler<unit, EmptyResponse> =
        let content' = ItemsWithoutCursor(Items=content)
        postV10<ItemsWithoutCursor<'TSource>, EmptyResponse> content' url

    let inline delete<'T, 'TNext, 'TResult> (content: 'T) (url: string) : IHttpHandler<unit, 'TNext> =
        url +/ "delete" |> postV10 content

    /// List content using protocol buffers
    let listProtobuf<'TSource, 'TResult> (content: 'TSource) (url: string) (parser: IO.Stream -> 'TResult): IHttpHandler<unit, 'TResult> =
        let url = url +/ "list"
        POST
        >=> withCompletion HttpCompletionOption.ResponseHeadersRead
        >=> withVersion V10
        >=> withResource url
        >=> withResponseType ResponseType.Protobuf
        >=> withContent (fun () -> new JsonPushStreamContent<'TSource>(content, jsonOptions) :> _)
        >=> fetch
        >=> withError decodeError
        >=> protobuf parser
        >=> log

    /// Create content using protocol buffers
    let createProtobuf<'TResult> (content: IMessage) (url: string) : IHttpHandler<unit, 'TResult> =
        POST
        >=> withVersion V10
        >=> withResource url
        >=> withContent (fun () -> new ProtobufPushStreamContent(content) :> _)
        >=> fetch
        >=> withError decodeError
        >=> json jsonOptions
        >=> log

    let createGzipProtobuf<'TResult> (content: IMessage) (compression: CompressionLevel) (url: string) : IHttpHandler<unit, 'TResult> =
        POST
        >=> withVersion V10
        >=> withResource url
        >=> withContent (fun () -> new GZipProtobufStreamContent(content, compression) :> _)
        >=> fetch
        >=> withError decodeError
        >=> json jsonOptions
        >=> log
