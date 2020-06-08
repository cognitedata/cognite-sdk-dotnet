// Copyright 2019 Cognite AS
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
    let withTokenRenewer<'TResult, 'TNext, 'TError> (tokenRenewer: Func<CancellationToken, Task<string>>) (handler: HttpHandler<HttpResponseMessage, 'TNext, 'TResult, 'TError>) =
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
    let runUnsafeAsync (ctx : HttpContext) (token: CancellationToken) (handler: HttpHandler<HttpResponseMessage, 'r,'r>) : Task<'r> = task {
        let runUnsafe  =
            ctx
            |> Context.withCancellationToken token
            |> runAsync

        match! runUnsafe handler with
        | Ok value -> return value
        | Error error -> return raiseError error
    }

    /// Decode response message into a ResponseException.
    let decodeError (response: HttpResponseMessage) : Task<HandlerError<ResponseException>> = task {
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
                let _, requestId = response.Headers.TryGetValues "x-request-id"
                match Seq.tryExactlyOne requestId with
                | Some requestId -> error.RequestId <- requestId
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


    let get<'a, 'b> (url: string) : HttpHandler<HttpResponseMessage, 'a, 'b> =
        GET
        >=> withResource url
        >=> fetch
        >=> log
        >=> withError decodeError
        >=> json jsonOptions

    let getV10<'a, 'b> (url: string)  : HttpHandler<HttpResponseMessage, 'a, 'b> =
        withVersion V10 >=> get url

    let inline getById (id: int64) (url: string) : HttpHandler<HttpResponseMessage, 'a, 'b> =
        url +/ sprintf "%d" id |> getV10

    let getWithQuery<'a, 'b> (query: IQueryParams) (url: string) : HttpHandler<HttpResponseMessage, ItemsWithCursor<'a>, 'b> =
        let parms = query.ToQueryParams ()
        GET
        >=> withVersion V10
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

    let postV10<'a, 'b, 'c> (content: 'a) (url: string) : HttpHandler<HttpResponseMessage, 'b, 'c> =
        withVersion V10 >=> post content url

    let postWithQuery<'a, 'b, 'c> (content: 'a) (query: IQueryParams) (url: string) : HttpHandler<HttpResponseMessage, 'b, 'c> =
        let parms = query.ToQueryParams ()

        POST
        >=> withVersion V10
        >=> withResource url
        >=> withQuery parms
        >=> withContent (fun () -> new JsonPushStreamContent<'a>(content, jsonOptions) :> _)
        >=> fetch
        >=> log
        >=> withError decodeError
        >=> json jsonOptions

    let inline list (content: 'a) (url: string) : HttpHandler<HttpResponseMessage, 'b, 'c> =
        withCompletion HttpCompletionOption.ResponseHeadersRead
        >=> postV10 content (url +/ "list")

    let inline aggregate (content: 'a) (url: string) : HttpHandler<HttpResponseMessage, int, 'c> =
        req {
            let url =  url +/ "aggregate"
            let! itemsobject =
                withCompletion HttpCompletionOption.ResponseHeadersRead
                >=> postV10<'a, ItemsWithoutCursor<AggregateCount>, 'c> content url
            let item = itemsobject.Items |> Seq.head
            return item.Count
        }

    let search<'a, 'b, 'c> (content: 'a) (url: string) : HttpHandler<HttpResponseMessage, IEnumerable<'b>, 'c> =
        req {
            let url = url +/ "search"
            let! ret =
                withCompletion HttpCompletionOption.ResponseHeadersRead
                >=> postV10<'a, ItemsWithoutCursor<'b>, 'c> content url
            return ret.Items
        }

    let update<'a, 'b, 'c> (items: IEnumerable<UpdateItem<'a>>) (url: string) : HttpHandler<HttpResponseMessage, IEnumerable<'b>, 'c> =
        req {
            let url = url +/ "update"
            let request = ItemsWithoutCursor<UpdateItem<'a>>(Items = items)
            let! ret = postV10<ItemsWithoutCursor<UpdateItem<'a>>, ItemsWithoutCursor<'b>, 'c> request url
            return ret.Items
        }

    let retrieve<'a, 'b> (ids: Identity seq) (url: string) : HttpHandler<HttpResponseMessage, IEnumerable<'a>, 'b> =
        req {
            let url = url +/ "byids"
            let request = ItemsWithoutCursor<Identity>(Items = ids)
            let! ret =
                withCompletion HttpCompletionOption.ResponseHeadersRead
                >=> postV10<ItemsWithoutCursor<Identity>, ItemsWithoutCursor<'a>, 'b> request url
            return ret.Items
        }

    let create<'a, 'b, 'c> (content: IEnumerable<'a>) (url: string) : HttpHandler<HttpResponseMessage, IEnumerable<'b>, 'c> =
        req {
            let content' = ItemsWithoutCursor(Items=content)
            let! ret = postV10<ItemsWithoutCursor<'a>, ItemsWithoutCursor<'b>, 'c> content' url
            return ret.Items
        }

    let createWithQuery<'a, 'b, 'c> (content: IEnumerable<'a>) (query: IQueryParams) (url: string) : HttpHandler<HttpResponseMessage, IEnumerable<'b>, 'c> =
        req {
            let content' = ItemsWithoutCursor(Items=content)
            let! ret = postWithQuery<ItemsWithoutCursor<'a>, ItemsWithoutCursor<'b>, 'c> content' query url
            return ret.Items
        }

    let createWithQueryEmpty<'a, 'b, 'c> (content: IEnumerable<'a>) (query: IQueryParams) (url: string) : HttpHandler<HttpResponseMessage, EmptyResponse, 'c> =
        let content' = ItemsWithoutCursor(Items=content)
        postWithQuery<ItemsWithoutCursor<'a>, EmptyResponse, 'c> content' query url

    let createEmpty<'a, 'b, 'c> (content: IEnumerable<'a>) (url: string) : HttpHandler<HttpResponseMessage, EmptyResponse, 'c> =
        let content' = ItemsWithoutCursor(Items=content)
        postV10<ItemsWithoutCursor<'a>, EmptyResponse, 'c> content' url

    let inline delete<'a, 'b, 'c> (content: 'a) (url: string) : HttpHandler<HttpResponseMessage, 'b, 'c> =
        url +/ "delete" |> postV10 content

    /// List content using protocol buffers
    let listProtobuf<'a, 'b, 'c> (content: 'a) (url: string) (parser: IO.Stream -> 'b): HttpHandler<HttpResponseMessage, 'b, 'c> =
        let url = url +/ "list"
        POST
        >=> withCompletion HttpCompletionOption.ResponseHeadersRead
        >=> withVersion V10
        >=> withResource url
        >=> withResponseType ResponseType.Protobuf
        >=> withContent (fun () -> new JsonPushStreamContent<'a>(content, jsonOptions) :> _)
        >=> fetch
        >=> log
        >=> withError decodeError
        >=> protobuf parser

    /// Create content using protocol buffers
    let createProtobuf<'a, 'b> (content: Google.Protobuf.IMessage) (url: string) : HttpHandler<HttpResponseMessage, 'a, 'b> =
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
