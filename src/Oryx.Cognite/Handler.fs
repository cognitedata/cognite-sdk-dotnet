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

open CogniteSdk


// Shadow types for Oryx that pins the error type to ResponseError
type HttpFuncResult<'r> = Task<Result<Context<'r>, HandlerError<ResponseException>>>
type HttpFunc<'a, 'r> = Context<'a> -> HttpFuncResult<'r, ResponseException>
type NextFunc<'a, 'r> = HttpFunc<'a, 'r, ResponseException>
type HttpHandler<'a, 'b, 'r> = NextFunc<'b, 'r, ResponseException> -> Context<'a> -> HttpFuncResult<'r, ResponseException>
type HttpHandler<'a, 'r> = HttpHandler<'a, 'a, 'r, ResponseException>
type HttpHandler<'r> = HttpHandler<HttpResponseMessage, 'r, ResponseException>
type HttpHandler = HttpHandler<HttpResponseMessage, ResponseException>

/// Oryx HTTP handlers for specific use within the Cognite SDK
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<AutoOpen>]
module Handler =
    let setResource (resource: string) (next: NextFunc<_,_>) (context: HttpContext) =
        next { context with Request = { context.Request with Extra = context.Request.Extra.Add("resource", resource) } }

    let setVersion (version: ApiVersion) (next: NextFunc<_,_>) (context: HttpContext) =
        next { context with Request = { context.Request with Extra = context.Request.Extra.Add("apiVersion", version.ToString ()) } }

    let setUrl (url: string) (next: NextFunc<_,_>) (context: HttpContext) =
        let urlBuilder (request: HttpRequest) =
            let extra = request.Extra
            let serviceUrl =
                if Map.containsKey "serviceUrl" extra
                then extra.["serviceUrl"]
                else "https://api.cognitedata.com"

            if not (Map.containsKey "hasAppId" extra)
            then failwith "Client must set the Application ID (appId)"

            sprintf "%s%s" serviceUrl url
        next { context with Request = { context.Request with UrlBuilder = urlBuilder } }

    /// Raises error for C# extension methods. Translates Oryx errors into CogniteSdk equivalents so clients don't
    /// need to open the Oryx namespace.
    let raiseError (error: HandlerError<ResponseException>) =
        match error with
        | ResponseError error -> raise error
        | Panic (Oryx.JsonDecodeException err) -> raise <| Oryx.Cognite.JsonDecodeException err
        | Panic (err) -> raise err

    /// Runs handler and returns the Ok result. Throws exception if any errors occured. Used by C# SDK.
    let runUnsafeAsync (handler: HttpHandler<HttpResponseMessage, 'r,'r>) (ctx : HttpContext) (token: CancellationToken) : Task<'r> = task {
        let! result =
            ctx
            |> Context.setCancellationToken token
            |> runAsync handler
        match result with
        | Ok value -> return value
        | Error error -> return raiseError error
    }

    /// Decode response message into a ResponseException.
    let decodeError (response: HttpResponseMessage) : Task<HandlerError<ResponseException>> = task {
        if response.Content.Headers.ContentType.MediaType.Contains "application/json" then
            use! stream = response.Content.ReadAsStreamAsync ()
            try
                let! error = JsonSerializer.DeserializeAsync<ApiResponseErrorDto>(stream, jsonOptions)
                let _, requestId = response.Headers.TryGetValues "x-request-id"
                match Seq.tryExactlyOne requestId with
                | Some requestId -> error.RequestId <- requestId
                | None -> ()

                return error.ToException () |> Oryx.ResponseError
            with
            | ex ->
                let exn = ResponseException(response.ReasonPhrase, ex)
                exn.Code <- int response.StatusCode
                return Oryx.ResponseError exn
        else
            let exn = ResponseException(response.ReasonPhrase)
            exn.Code <- int response.StatusCode
            return Oryx.ResponseError exn
    }

    let get<'a, 'b> (url: string) : HttpHandler<HttpResponseMessage, 'a, 'b> =      
        GET
        >=> setVersion V10
        >=> setResource url
        >=> fetch
        >=> withError decodeError
        >=> json jsonOptions
        
    let inline getById<'a, 'b> (id: int64) (url: string) : HttpHandler<HttpResponseMessage, 'a, 'b> =
        url +/ sprintf "%d" id |> get

    let getWithQuery<'a, 'b> (query: IQueryParams) (url: string) : HttpHandler<HttpResponseMessage, ItemsWithCursor<'a>, 'b> =
        let parms = 
            query.ToQueryParams ()
            |> Seq.map (fun x -> x.ToTuple())
            |> List.ofSeq
            
        GET
        >=> setVersion V10
        >=> setResource url
        >=> addQuery parms 
        >=> fetch
        >=> withError decodeError
        >=> json jsonOptions

    let post<'a, 'b, 'c> (content: 'a) (url: string) : HttpHandler<HttpResponseMessage, 'b, 'c> =
        POST
        >=> setVersion V10
        >=> setResource url
        >=> setContent (new JsonPushStreamContent<'a>(content, jsonOptions))
        >=> fetch
        >=> withError decodeError
        >=> json jsonOptions

    let inline list<'a, 'b, 'c> (content: 'a) (url: string) : HttpHandler<HttpResponseMessage, 'b, 'c> =
        url +/ "list" |> post content
        
    let inline search<'a, 'b, 'c> (content: 'a) (url: string) : HttpHandler<HttpResponseMessage, 'b, 'c> =
        url +/ "search" |> post content
        
    let inline update<'a, 'b, 'c> (content: 'a) (url: string) : HttpHandler<HttpResponseMessage, 'b, 'c> =
        url +/ "update" |> post content
    
    let inline retrieve<'a, 'b, 'c> (ids: Identity seq) (url: string) : HttpHandler<HttpResponseMessage, ItemsWithoutCursor<'a>, 'b> =
        let request = ItemsWithoutCursor<Identity>(Items = ids)
        url +/ "byids" |> post request

    let inline create<'a, 'b, 'c> (content: 'a) (url: string) : HttpHandler<HttpResponseMessage, 'b, 'c> =
        post content url
        
    let inline delete<'a, 'b, 'c> (content: 'a) (url: string) : HttpHandler<HttpResponseMessage, 'b, 'c> =
        url +/ "delete" |> post content
    
    let retry (initialDelay: int<ms>) (maxRetries : int) (next: NextFunc<'a,'r>) (ctx: Context<'a>) : HttpFuncResult<'r> =
        let shouldRetry (error: HandlerError<ResponseException>) : bool =
            match error with
            | ResponseError err ->
                match err.Code with
                // Rate limiting
                | 429 -> true
                // and I would like to say never on other 4xx, but we give 401 when we can't authenticate because
                // we lose connection to db, so 401 can be transient
                | 401 -> true
                // 500 is hard to say, but we should avoid having those in the api
                | 500 ->
                    true // we get random and transient 500 responses often enough that it's worth retrying them.
                // 502 and 503 are usually transient.
                | 502 -> true
                | 503 -> true
                // do not retry other responses.
                | _ -> false
            | Panic (Oryx.JsonDecodeException _) -> false
            | Panic err ->
                match err with
                | :? Net.Http.HttpRequestException
                | :? System.Net.WebException -> true
                // do not retry other exceptions.
                | _ -> true

        retry shouldRetry initialDelay maxRetries next ctx
