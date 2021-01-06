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

open FSharp.Control.Tasks.V2.ContextInsensitive

open Oryx
open Oryx.Retry
open Oryx.SystemTextJson
open Oryx.SystemTextJson.ResponseReader
open Oryx.Protobuf
open Oryx.Protobuf.ResponseReader

open Oryx.Cognite

open CogniteSdk

/// Oryx HTTP handlers for specific use within the Cognite SDK
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<AutoOpen>]
module Handler =
    // TODO: add to Oryx and remove here when available in Oryx
    let withHeader<'TResult> (name: string) (value: string) (next: NextFunc<unit, 'TResult>) (context: Context<unit>) =
        next { context with Request = { context.Request with Headers = context.Request.Headers.Add(name, value) } }

    let withResource (resource: string) (next: NextFunc<_,_>) (context: HttpContext) =
        next { context with Request = { context.Request with Items = context.Request.Items.Add(PlaceHolder.Resource, String resource) } }

    let withVersion (version: ApiVersion) (next: NextFunc<_,_>) (context: HttpContext) =
        next { context with Request = { context.Request with Items = context.Request.Items.Add(PlaceHolder.ApiVersion, String (version.ToString ())) } }

    let withUrl (url: string) (next: NextFunc<_,_>) (context: HttpContext) =
        let urlBuilder (request: HttpRequest) =
            let extra = request.Items
            let baseUrl =
                match Map.tryFind PlaceHolder.BaseUrl extra with
                | Some (Url url) -> url.ToString ()
                | _ -> "https://api.cognitedata.com"

            if not (Map.containsKey PlaceHolder.HasAppId extra)
            then failwith "Client must set the Application ID (appId)"
            baseUrl +/ url
        next { context with Request = { context.Request with UrlBuilder = urlBuilder } }

    /// Raises error for C# extension methods. Translates Oryx errors into CogniteSdk equivalents so clients don't
    /// need to open the Oryx namespace.
    let raiseError (error: HandlerError<ResponseException>) =
        match error with
        | ResponseError error -> raise error
        | Panic err -> raise err

    /// Composes the given handler token provider with the request. Adapts a C# renewer function to F#.
    let withTokenRenewer<'TResult, 'TNext, 'TError> (tokenRenewer: Func<CancellationToken, Task<string>>) (handler: HttpHandler<unit, 'TNext, 'TResult, 'TError>) =
        let renewer ct = task {
            try
                let! token = tokenRenewer.Invoke ct
                match Option.ofObj token with
                | Some token -> return Ok token
                | None -> return Panic (ArgumentNullException "No token received.") |> Error
            with
            | ex -> return Panic ex |> Error
        }

        withTokenRenewer renewer >=> handler

    /// Runs handler and returns the Ok result. Throws exception if any errors occured. Used by C# SDK.
    let runUnsafeAsync (ctx : HttpContext) (token: CancellationToken) (handler: HttpHandler<unit, 'r,'r>) : Task<'r> = task {
        let runUnsafe  =
            ctx
            |> Context.withCancellationToken token
            |> runAsync

        match! runUnsafe handler with
        | Ok value -> return value
        | Error error -> return raiseError error
    }

    /// Decode response message into a ResponseException.
    let decodeError (response: HttpResponse<HttpContent>) : Task<HandlerError<ResponseException>> = task {
        let mediaType =
           Option.ofObj response.Content
           |> Option.bind (fun content -> content.Headers |> Option.ofObj)
           |> Option.bind (fun headers -> headers.ContentType |> Option.ofObj)
           |> Option.bind (fun contentType -> contentType.MediaType |> Option.ofObj)
           |> Option.defaultValue String.Empty

        if mediaType.Contains "application/json" then
            use! stream = response.Content.ReadAsStreamAsync ()

            try
                let! error = JsonSerializer.DeserializeAsync<ApiResponseError>(stream, jsonOptions)
                let requestId = response.Headers |> Map.tryFind "X-Request-ID"

                match requestId with
                | Some requestIds ->
                    let requestId = Seq.tryExactlyOne requestIds |> Option.defaultValue String.Empty
                    error.RequestId <- requestId
                | None -> ()

                return error.ToException () |> Oryx.ResponseError
            with
            | ex ->
                let exn = ResponseException (response.ReasonPhrase, ex)
                exn.Code <- int response.StatusCode
                return Oryx.ResponseError exn
        else
            let exn = ResponseException response.ReasonPhrase
            exn.Code <- int response.StatusCode
            return Oryx.ResponseError exn
    }

    let get<'TNext, 'TResult> (url: string) : HttpHandler<unit, 'TNext, 'TResult> =
        GET
        >=> withResource url
        >=> fetch
        >=> log
        >=> withError decodeError
        >=> json jsonOptions

    let getV10<'TNext, 'TResult> (url: string)  : HttpHandler<unit, 'TNext, 'TResult> =
        withVersion V10 >=> get url

    let inline getById (id: int64) (url: string) : HttpHandler<unit, 'TNext, 'TResult> =
        url +/ sprintf "%d" id |> getV10

    let getWithQuery<'TNext, 'TResult> (query: IQueryParams) (url: string) : HttpHandler<unit, ItemsWithCursor<'TNext>, 'TResult> =
        let parms = query.ToQueryParams ()
        GET
        >=> withVersion V10
        >=> withResource url
        >=> withQuery parms
        >=> fetch
        >=> log
        >=> withError decodeError
        >=> json jsonOptions

    let post<'T, 'TNext, 'TResult> (content: 'T) (url: string) : HttpHandler<unit, 'TNext, 'TResult> =
        POST
        >=> withResource url
        >=> withContent (fun () -> new JsonPushStreamContent<'T>(content, jsonOptions) :> _)
        >=> fetch
        >=> log
        >=> withError decodeError
        >=> json jsonOptions

    let postV10<'T, 'TNext, 'TResult> (content: 'T) (url: string) : HttpHandler<unit, 'TNext, 'TResult> =
        withVersion V10 >=> post content url

    let postWithQuery<'T, 'TNext, 'TResult> (content: 'T) (query: IQueryParams) (url: string) : HttpHandler<unit, 'TNext, 'TResult> =
        let parms = query.ToQueryParams ()

        POST
        >=> withVersion V10
        >=> withResource url
        >=> withQuery parms
        >=> withContent (fun () -> new JsonPushStreamContent<'T>(content, jsonOptions) :> _)
        >=> fetch
        >=> log
        >=> withError decodeError
        >=> json jsonOptions

    let inline list (content: 'T) (url: string) : HttpHandler<unit, 'TNext, 'TResult> =
        withCompletion HttpCompletionOption.ResponseHeadersRead
        >=> postV10 content (url +/ "list")

    let inline aggregate (content: 'T) (url: string) : HttpHandler<unit, int, 'TResult> =
        req {
            let url =  url +/ "aggregate"
            let! ret =
                withCompletion HttpCompletionOption.ResponseHeadersRead
                >=> postV10<'T, ItemsWithoutCursor<AggregateCount>, 'TResult> content url
            let item = ret.Items |> Seq.head
            return item.Count
        }

    let search<'T, 'TNext, 'TResult> (content: 'T) (url: string) : HttpHandler<unit, IEnumerable<'TNext>, 'TResult> =
        req {
            let url = url +/ "search"
            let! ret =
                withCompletion HttpCompletionOption.ResponseHeadersRead
                >=> postV10<'T, ItemsWithoutCursor<'TNext>, 'TResult> content url
            return ret.Items
        }

    let update<'T, 'TNext, 'TResult> (items: IEnumerable<UpdateItem<'T>>) (url: string) : HttpHandler<unit, IEnumerable<'TNext>, 'TResult> =
        req {
            let url = url +/ "update"
            let request = ItemsWithoutCursor<UpdateItem<'T>>(Items = items)
            let! ret = postV10<ItemsWithoutCursor<UpdateItem<'T>>, ItemsWithoutCursor<'TNext>, 'TResult> request url
            return ret.Items
        }

    let retrieve<'TNext, 'TResult> (ids: Identity seq) (url: string) : HttpHandler<unit, IEnumerable<'TNext>, 'TResult> =
        req {
            let url = url +/ "byids"
            let request = ItemsWithoutCursor<Identity>(Items = ids)
            let! ret =
                withCompletion HttpCompletionOption.ResponseHeadersRead
                >=> postV10<ItemsWithoutCursor<Identity>, ItemsWithoutCursor<'TNext>, 'TResult> request url
            return ret.Items
        }

    let retrieveIgnoreUnkownIds<'TNext, 'TResult> (ids: Identity seq) (ignoreUnknownIdsOpt: bool option) (url: string) : HttpHandler<unit, IEnumerable<'TNext>, 'TResult> =
        match ignoreUnknownIdsOpt with
        | Some ignoreUnknownIds ->
            req {
                let url = url +/ "byids"
                let request = ItemsWithIgnoreUnknownIds<Identity>(Items = ids, IgnoreUnknownIds = ignoreUnknownIds)
                let! ret =
                    withCompletion HttpCompletionOption.ResponseHeadersRead
                    >=> postV10<ItemsWithIgnoreUnknownIds<Identity>, ItemsWithoutCursor<'TNext>, 'TResult> request url
                return ret.Items
            }
        | None -> retrieve ids url

    let create<'T, 'TNext, 'TResult> (content: IEnumerable<'T>) (url: string) : HttpHandler<unit, IEnumerable<'TNext>, 'TResult> =
        req {
            let content' = ItemsWithoutCursor(Items=content)
            let! ret = postV10<ItemsWithoutCursor<'T>, ItemsWithoutCursor<'TNext>, 'TResult> content' url
            return ret.Items
        }

    let createWithQuery<'T, 'TNext, 'TResult> (content: IEnumerable<'T>) (query: IQueryParams) (url: string) : HttpHandler<unit, IEnumerable<'TNext>, 'TResult> =
        req {
            let content' = ItemsWithoutCursor(Items=content)
            let! ret = postWithQuery<ItemsWithoutCursor<'T>, ItemsWithoutCursor<'TNext>, 'TResult> content' query url
            return ret.Items
        }

    let createWithQueryEmpty<'T, 'TResult> (content: IEnumerable<'T>) (query: IQueryParams) (url: string) : HttpHandler<unit, EmptyResponse, 'TResult> =
        let content' = ItemsWithoutCursor(Items=content)
        postWithQuery<ItemsWithoutCursor<'T>, EmptyResponse, 'TResult> content' query url

    let createEmpty<'T, 'TResult> (content: IEnumerable<'T>) (url: string) : HttpHandler<unit, EmptyResponse, 'TResult> =
        let content' = ItemsWithoutCursor(Items=content)
        postV10<ItemsWithoutCursor<'T>, EmptyResponse, 'TResult> content' url

    let inline delete<'T, 'TNext, 'TResult> (content: 'T) (url: string) : HttpHandler<unit, 'TNext, 'TResult> =
        url +/ "delete" |> postV10 content

    /// List content using protocol buffers
    let listProtobuf<'T, 'TNext, 'TResult> (content: 'T) (url: string) (parser: IO.Stream -> 'TNext): HttpHandler<unit, 'TNext, 'TResult> =
        let url = url +/ "list"
        POST
        >=> withCompletion HttpCompletionOption.ResponseHeadersRead
        >=> withVersion V10
        >=> withResource url
        >=> withResponseType ResponseType.Protobuf
        >=> withContent (fun () -> new JsonPushStreamContent<'T>(content, jsonOptions) :> _)
        >=> fetch
        >=> log
        >=> withError decodeError
        >=> protobuf parser

    /// Create content using protocol buffers
    let createProtobuf<'TNext, 'TResult> (content: Google.Protobuf.IMessage) (url: string) : HttpHandler<unit, 'TNext, 'TResult> =
        POST
        >=> withVersion V10
        >=> withResource url
        >=> withContent (fun () -> new ProtobufPushStreamContent(content) :> _)
        >=> fetch
        >=> log
        >=> withError decodeError
        >=> json jsonOptions

    /// Retry handler. May be used by F# clients. C# clients should use Polly instead.
    let retry (initialDelay: int<ms>) (maxRetries : int) (next: NextFunc<'a,'r>) (ctx: Context<'a>) : HttpFuncResult<'r> =
        let shouldRetry (error: HandlerError<ResponseException>) : bool =
            match error with
            | ResponseError err ->
                match err.Code with
                // Rate limiting
                | 429 -> true
                // 500 is hard to say, but we should avoid having those in the api. We get random and transient 500
                // responses often enough that it's worth retrying them.
                | 500 -> true
                // 502 and 503 are usually transient.
                | 502 -> true
                | 503 -> true
                // Do not retry other responses.
                | _ -> false
            | Panic err ->
                match err with
                | :? Net.Http.HttpRequestException
                | :? System.Net.WebException -> true
                // do not retry other exceptions.
                | _ -> false

        retry shouldRetry initialDelay maxRetries next ctx
