// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

/// Common types for the SDK.
namespace Oryx.Cognite

open System
open System.Collections.Generic
open System.Net.Http
open System.Reflection
open System.Text.Json
open System.Threading
open System.Threading.Tasks
open System.IO.Compression

open FSharp.Control.TaskBuilder
open Google.Protobuf
open Microsoft.Extensions.Logging

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
    let urlBuilder (request: HttpRequest) =

        let items = request.Items

        let version =
            match Map.tryFind PlaceHolder.ApiVersion items with
            | Some (Value.String version) -> version
            | _ -> failwith "API version not set."

        let project =
            match Map.tryFind PlaceHolder.Project items with
            | Some (Value.String project) -> project
            | _ -> failwith "Client must set project."

        let resource =
            match Map.tryFind PlaceHolder.Resource items with
            | Some (Value.String resource) -> resource
            | _ -> failwith "Resource not set."

        let baseUrl =
            match Map.tryFind PlaceHolder.BaseUrl items with
            | Some (Value.Url url) -> url.ToString()
            | _ -> "https://api.cognitedata.com"

        if not (Map.containsKey PlaceHolder.HasAppId items) then
            failwith "Client must set the Application ID (appId)."

        sprintf "api/%s/projects/%s%s" version project resource
        |> combine baseUrl

    let private fileVersion =
        Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            .InformationalVersion

    let withUrlBuilder urlBuilder (source: HttpHandler<unit>) : HttpHandler<unit> = source |> withUrlBuilder urlBuilder

    /// Set the project to connect to.
    let withProject (project: string) (source: HttpHandler<unit>) : HttpHandler<unit> =
        source
        |> update (fun ctx ->
            { ctx with
                Request = { ctx.Request with Items = ctx.Request.Items.Add(PlaceHolder.Project, Value.String project) } })

    let withAppId (appId: string) (source: HttpHandler<unit>) : HttpHandler<unit> =
        source
        |> update (fun ctx ->
            { ctx with
                Request =
                    { ctx.Request with
                        Headers = ctx.Request.Headers.Add("x-cdp-app", appId)
                        Items = ctx.Request.Items.Add(PlaceHolder.HasAppId, Value.String "true") } })

    let withBaseUrl (baseUrl: Uri) (source: HttpHandler<unit>) : HttpHandler<unit> =
        source
        |> update (fun ctx ->
            { ctx with
                Request = { ctx.Request with Items = ctx.Request.Items.Add(PlaceHolder.BaseUrl, Value.Url baseUrl) } })

    let withLogLevel (logLevel: LogLevel) (source: HttpHandler<'TSource>) : HttpHandler<'TSource> =
        withLogLevel logLevel source

    let empty: HttpHandler<unit> =
        httpRequest
        |> withUrlBuilder urlBuilder
        |> withHeader ("x-cdp-sdk", sprintf "CogniteNetSdk:%s" fileVersion)
        |> withLogFormat "CDF ({Message}): {Url}\n→ {RequestContent}\n← {ResponseContent}"

    let withResource (resource: string) (source: HttpHandler<'TSource>) : HttpHandler<'TSource> =
        source
        |> update (fun ctx ->
            { ctx with
                Request =
                    { ctx.Request with Items = ctx.Request.Items.Add(PlaceHolder.Resource, Value.String resource) } })

    let withVersion<'TSource> (version: ApiVersion) (source: HttpHandler<'TSource>) : HttpHandler<'TSource> =
        source
        |> update (fun ctx ->
            { ctx with
                Request =
                    { ctx.Request with
                        Items = ctx.Request.Items.Add(PlaceHolder.ApiVersion, Value.String(version.ToString())) } })

    let withUrl (url: string) (source: HttpHandler<'TSource>) : HttpHandler<'TSource> =
        let urlBuilder (request: HttpRequest) =
            let extra = request.Items

            let baseUrl =
                match Map.tryFind PlaceHolder.BaseUrl extra with
                | Some (Value.Url url) -> url.ToString()
                | _ -> "https://api.cognitedata.com"

            if not (Map.containsKey PlaceHolder.HasAppId extra) then
                failwith "Client must set the Application ID (appId)"

            baseUrl +/ url

        source
        |> update (fun ctx -> { ctx with Request = { ctx.Request with UrlBuilder = urlBuilder } })

    /// Raises error for C# extension methods. Translates Oryx errors into CogniteSdk equivalents so clients don't
    /// need to open the Oryx namespace.
    let raiseError (error: exn) = raise error

    /// Composes the given handler token provider with the request. Adapts a C# renewer function to F#.
    let withTokenRenewer<'TResult>
        (tokenRenewer: Func<CancellationToken, Task<string>>)
        (source: HttpHandler<'TResult>)
        =
        let renewer ct =
            task {
                try
                    let! token = tokenRenewer.Invoke ct

                    match Option.ofObj token with
                    | Some token -> return Ok token
                    | None ->
                        return
                            (ArgumentNullException "No token received." :> exn)
                            |> Error
                with
                | ex -> return ex |> Error
            }

        source |> withTokenRenewer renewer

    /// Decode response message into a ResponseException.
    let decodeError (response: HttpResponse) (content: HttpContent) : Task<exn> =
        task {
            let mediaType =
                content.Headers
                |> Option.ofObj
                |> Option.bind (fun headers -> headers.ContentType |> Option.ofObj)
                |> Option.bind (fun contentType -> contentType.MediaType |> Option.ofObj)
                |> Option.defaultValue String.Empty

            if mediaType.Contains "application/json" then
                use! stream = content.ReadAsStreamAsync()

                try
                    let! error = JsonSerializer.DeserializeAsync<ApiResponseError>(stream, jsonOptions)
                    let requestId = response.Headers |> Map.tryFind "X-Request-ID"

                    match requestId with
                    | Some requestIds ->
                        let requestId =
                            Seq.tryExactlyOne requestIds
                            |> Option.defaultValue String.Empty

                        error.RequestId <- requestId
                    | None -> ()

                    return error.ToException()
                with
                | ex ->
                    let exn = ResponseException(response.ReasonPhrase, ex)
                    exn.Code <- int response.StatusCode
                    return exn :> _
            else
                let exn = ResponseException response.ReasonPhrase
                exn.Code <- int response.StatusCode
                return exn :> _
        }

    let getOptions<'TResult> (url: string) (options: JsonSerializerOptions) (source: HttpHandler<unit>) =
        source
        |> GET
        |> withResource url
        |> fetch
        |> withError decodeError
        |> json<'TResult> options
        |> log

    let get<'TResult> (url: string) (source: HttpHandler<unit>) =
        source |> getOptions<'TResult> url jsonOptions

    let getV10<'TResult> (url: string) source =
        source |> withVersion V10 |> get<'TResult> url

    let getV10Options<'TResult>
        (url: string)
        (options: JsonSerializerOptions)
        (source: HttpHandler<unit>)
        : HttpHandler<'TResult> =
        source
        |> withVersion V10
        |> getOptions url options

    let inline getById<'TResult> (id: int64) (url: string) (source: HttpHandler<unit>) : HttpHandler<'TResult> =
        source
        |> getV10<'TResult> (url +/ sprintf "%d" id)

    let getWithQueryOptions<'TResult>
        (query: IQueryParams)
        (url: string)
        (options: JsonSerializerOptions)
        (source: HttpHandler<unit>)
        : HttpHandler<'TResult> =
        let parms = query.ToQueryParams()

        source
        |> GET
        |> withVersion V10
        |> withResource url
        |> withQuery parms
        |> fetch
        |> withError decodeError
        |> json options
        |> log

    let getWithQuery<'TResult> (query: IQueryParams) (url: string) (source: HttpHandler<unit>) : HttpHandler<'TResult> =
        getWithQueryOptions query url jsonOptions source

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
        |> json<'TResult> jsonOptions
        |> log

    let postV10<'TContent, 'TResult>
        (content: 'TContent)
        (url: string)
        (source: HttpHandler<unit>)
        : HttpHandler<'TResult> =
        source
        |> withVersion V10
        |> post<'TContent, 'TResult> content url

    let postWithQuery<'TContent, 'TResult>
        (content: 'TContent)
        (query: IQueryParams)
        (url: string)
        (options: JsonSerializerOptions)
        (source: HttpHandler<unit>)
        : HttpHandler<'TResult> =
        let parameters = query.ToQueryParams()

        source
        |> POST
        |> withVersion V10
        |> withResource url
        |> withQuery parameters
        |> withContent (fun () -> new JsonPushStreamContent<'TContent>(content, options) :> _)
        |> fetch
        |> withError decodeError
        |> json options
        |> log

    let inline list<'TContent, 'TResult> (content: 'TContent) (url: string) source =
        source
        |> withCompletion HttpCompletionOption.ResponseHeadersRead
        |> postV10<'TContent, 'TResult> content (url +/ "list")

    let inline aggregate (content: 'TSource) (url: string) (source: HttpHandler<unit>) : HttpHandler<int> =
        http {
            let url = url +/ "aggregate"

            let! ret =
                source
                |> withCompletion HttpCompletionOption.ResponseHeadersRead
                |> postV10<'TSource, ItemsWithoutCursor<AggregateCount>> content url

            let item = ret.Items |> Seq.head
            return item.Count
        }

    let search<'TSource, 'TResult>
        (content: 'TSource)
        (url: string)
        (source: HttpHandler<unit>)
        : HttpHandler<IEnumerable<'TResult>> =
        http {
            let url = url +/ "search"

            let! ret =
                source
                |> withCompletion HttpCompletionOption.ResponseHeadersRead
                |> postV10<'TSource, ItemsWithoutCursor<'TResult>> content url

            return ret.Items
        }

    let update<'TSource, 'TResult>
        (items: IEnumerable<UpdateItem<'TSource>>)
        (url: string)
        (source: HttpHandler<unit>)
        : HttpHandler<IEnumerable<'TResult>> =
        http {
            let url = url +/ "update"
            let request = ItemsWithoutCursor<UpdateItem<'TSource>>(Items = items)

            let! ret =
                source
                |> postV10<ItemsWithoutCursor<UpdateItem<'TSource>>, ItemsWithoutCursor<'TResult>> request url

            return ret.Items
        }

    let retrieve<'TResult>
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
                |> postV10<ItemsWithoutCursor<Identity>, ItemsWithoutCursor<'TResult>> request url

            return ret.Items
        }

    let retrieveIgnoreUnknownIds<'TSource, 'TResult>
        (ids: Identity seq)
        (ignoreUnknownIdsOpt: bool option)
        (url: string)
        (source: HttpHandler<unit>)
        : HttpHandler<IEnumerable<'TResult>> =
        match ignoreUnknownIdsOpt with
        | Some ignoreUnknownIds ->
            http {
                let url = url +/ "byids"

                let request =
                    ItemsWithIgnoreUnknownIds<Identity>(Items = ids, IgnoreUnknownIds = ignoreUnknownIds)

                let! ret =
                    source
                    |> withCompletion HttpCompletionOption.ResponseHeadersRead
                    |> postV10<ItemsWithIgnoreUnknownIds<Identity>, ItemsWithoutCursor<'TResult>> request url

                return ret.Items
            }
        | None -> source |> retrieve ids url

    let create<'TSource, 'TResult>
        (content: IEnumerable<'TSource>)
        (url: string)
        (source: HttpHandler<unit>)
        : HttpHandler<IEnumerable<'TResult>> =
        http {
            let content' = ItemsWithoutCursor(Items = content)

            let! ret =
                source
                |> postV10<ItemsWithoutCursor<'TSource>, ItemsWithoutCursor<'TResult>> content' url

            return ret.Items
        }

    let createWithQuery<'TSource, 'TResult>
        (content: IEnumerable<'TSource>)
        (query: IQueryParams)
        (url: string)
        (source: HttpHandler<unit>)
        : HttpHandler<IEnumerable<'TResult>> =
        http {
            let content' = ItemsWithoutCursor(Items = content)

            let! ret =
                source
                |> postWithQuery<ItemsWithoutCursor<'TSource>, ItemsWithoutCursor<'TResult>>
                    content'
                    query
                    url
                    jsonOptions

            return ret.Items
        }

    let createWithQueryEmptyOptions<'TSource>
        (content: IEnumerable<'TSource>)
        (query: IQueryParams)
        (url: string)
        (options: JsonSerializerOptions)
        (source: HttpHandler<unit>)
        : HttpHandler<EmptyResponse> =
        let content' = ItemsWithoutCursor(Items = content)

        source
        |> postWithQuery<ItemsWithoutCursor<'TSource>, EmptyResponse> content' query url options

    let createWithQueryEmpty<'TSource>
        (content: IEnumerable<'TSource>)
        (query: IQueryParams)
        (url: string)
        (source: HttpHandler<unit>)
        : HttpHandler<EmptyResponse> =
        source
        |> createWithQueryEmptyOptions content query url jsonOptions

    let createEmpty<'TSource>
        (content: IEnumerable<'TSource>)
        (url: string)
        (source: HttpHandler<unit>)
        : HttpHandler<EmptyResponse> =
        let content' = ItemsWithoutCursor(Items = content)

        source
        |> postV10<ItemsWithoutCursor<'TSource>, EmptyResponse> content' url

    let inline delete<'T, 'TNext, 'TResult> (content: 'T) (url: string) source : HttpHandler<'TNext> =
        source |> postV10 content (url +/ "delete")

    /// List content using protocol buffers
    let listProtobuf<'TSource, 'TResult>
        (content: 'TSource)
        (url: string)
        (parser: IO.Stream -> 'TResult)
        (source: HttpHandler<unit>)
        : HttpHandler<'TResult> =
        let url = url +/ "list"

        source
        |> POST
        |> withCompletion HttpCompletionOption.ResponseHeadersRead
        |> withVersion V10
        |> withResource url
        |> withResponseType ResponseType.Protobuf
        |> withContent (fun () -> new JsonPushStreamContent<'TSource>(content, jsonOptions) :> _)
        |> fetch
        |> withError decodeError
        |> protobuf parser
        |> log

    /// Create content using protocol buffers
    let createProtobuf<'TResult> (content: IMessage) (url: string) (source: HttpHandler<unit>) : HttpHandler<'TResult> =
        source
        |> POST
        |> withVersion V10
        |> withResource url
        |> withContent (fun () -> new ProtobufPushStreamContent(content) :> _)
        |> fetch
        |> withError decodeError
        |> json jsonOptions
        |> log

    let createGzipProtobuf<'TResult>
        (content: IMessage)
        (compression: CompressionLevel)
        (url: string)
        (source: HttpHandler<unit>)
        : HttpHandler<'TResult> =
        source
        |> POST
        |> withVersion V10
        |> withResource url
        |> withContent (fun () -> new GZipProtobufStreamContent(content, compression) :> _)
        |> fetch
        |> withError decodeError
        |> json jsonOptions
        |> log
