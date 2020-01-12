// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

/// Common types for the SDK.
namespace Oryx.Cognite

open System
open System.Net.Http
open System.IO
open System.Reflection
open System.Text.Json
open System.Text.RegularExpressions
open System.Threading.Tasks

open CogniteSdk.Types.Common
open FSharp.Control.Tasks.V2.ContextInsensitive
open Thoth.Json.Net

open Oryx
open Oryx.Retry


// Shadow types that pins the error type for Oryx to ResponeError
type HttpFuncResult<'r> =  Task<Result<Context<'r>, HandlerError<ResponseException>>>
type HttpFunc<'a, 'r> = Context<'a> -> HttpFuncResult<'r, ResponseException>
type NextFunc<'a, 'r> = HttpFunc<'a, 'r, ResponseException>
type HttpHandler<'a, 'b, 'r> = NextFunc<'b, 'r, ResponseException> -> Context<'a> -> HttpFuncResult<'r, ResponseException>
type HttpHandler<'a, 'r> = HttpHandler<'a, 'a, 'r, ResponseException>
type HttpHandler<'r> = HttpHandler<HttpResponseMessage, 'r, ResponseException>
type HttpHandler = HttpHandler<HttpResponseMessage, ResponseException>

type ApiVersion =
    | V05
    | V06
    | V10

    override this.ToString () =
        match this with
        | V05 -> "0.5"
        | V06 -> "0.6"
        | V10 -> "v1"

/// Id or ExternalId
type Identity =
    internal
    | CaseId of int64
    | CaseExternalId of string

    static member Id id =
        CaseId id
    static member ExternalId id =
        CaseExternalId id

type Numeric =
    internal
    | CaseString of string
    | CaseInteger of int64
    | CaseFloat of double

    static member String value =
        CaseString value

    static member Integer value =
        CaseInteger value

    static member Float value =
        CaseFloat value

/// Used to describe a time range.
[<CLIMutable>]
type TimeRange = {
    Max: DateTimeOffset
    Min: DateTimeOffset
} with
    member this.Encoder =
        Encode.object [
            yield "max", this.Max.ToUnixTimeMilliseconds() |> Encode.int64
            yield "min", this.Min.ToUnixTimeMilliseconds() |> Encode.int64
        ]

[<AutoOpen>]
module Patterns =
    /// Active pattern to permit pattern matching over numeric values.
    let (|Integer|Float|String|) (value : Numeric) : Choice<int64, float, string>  =
        match value with
        | CaseInteger value -> Integer value
        | CaseFloat value -> Float value
        | CaseString value -> String value

    /// Active pattern to permit pattern matching over identity values.
    let (|Id|ExternalId|) (value : Identity) : Choice<int64, string>  =
        match value with
        | CaseId value -> Id value
        | CaseExternalId value -> ExternalId value

    let (|ParseInteger|_|) (str: string) =
       let mutable intvalue = 0
       if System.Int32.TryParse(str, &intvalue) then Some(intvalue)
       else None

    let (|ParseRegex|_|) regex str =
       let m = Regex(regex).Match(str)
       if m.Success
       then Some (List.tail [ for x in m.Groups -> x.Value ])
       else None

[<AutoOpen>]
module Common =
    [<Literal>]
    let MaxLimitSize = 1000

    let (+/) path1 path2 = Path.Combine(path1, path2)

    let jsonOptions = JsonSerializerOptions(AllowTrailingCommas=true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase)

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Context =
    let urlBuilder (request: HttpRequest) =
        let extra = request.Extra
        let version = extra.["apiVersion"]
        let project =
            if Map.containsKey "project" extra
            then extra.["project"]
            else failwith "Client must set project."

        let resource = extra.["resource"]
        let serviceUrl =
            if Map.containsKey "serviceUrl" extra
            then extra.["serviceUrl"]
            else "https://api.cognitedata.com"

        if not (Map.containsKey "hasAppId" extra)
        then failwith "Client must set the Application ID (appId)."

        sprintf "%s/api/%s/projects/%s%s" serviceUrl version project resource

    let private version =
        let version = Assembly.GetExecutingAssembly().GetName().Version
        (version.Major, version.Minor, version.Build)

    let setUrlBuilder ctx =
        Context.setUrlBuilder urlBuilder ctx

    /// Set the project to connect to.
    let setProject (project: string) (context: HttpContext) =
        { context with Request = { context.Request with Extra = context.Request.Extra.Add("project", project) } }

    let setAppId (appId: string) (context: HttpContext) =
        { context with Request = { context.Request with Headers =  ("x-cdp-app", appId) :: context.Request.Headers; Extra = context.Request.Extra.Add("hasAppId", "true") } }

    let setServiceUrl (serviceUrl: string) (context: HttpContext) =
        { context with Request = { context.Request with Extra = context.Request.Extra.Add("serviceUrl", serviceUrl) } }

    let create () =
        let major, minor, build = version
        Context.defaultContext
        |> Context.setUrlBuilder urlBuilder
        |> Context.addHeader ("x-cdp-sdk", sprintf "CogniteNetSdk:%d.%d.%d" major minor build)

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<AutoOpen>]
module Handlers =
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

    let decodeError (response: HttpResponseMessage) : Task<HandlerError<ResponseException>> = task {
        if response.Content.Headers.ContentType.MediaType.Contains "application/json" then
            use! stream = response.Content.ReadAsStreamAsync ()
            try
                let! error = JsonSerializer.DeserializeAsync<ApiResponseError>(stream, jsonOptions)
                let found, requestId = response.Headers.TryGetValues "x-request-id"
                match Seq.tryExactlyOne requestId with
                | Some requestId -> error.RequestId <- requestId
                | None -> ()

                return error.ToException () |> ResponseError
            with
            | ex ->
                let exn = ResponseException(response.ReasonPhrase, ex)
                exn.Code <- int response.StatusCode
                return ResponseError exn
        else
            let exn = ResponseException(response.ReasonPhrase)
            exn.Code <- int response.StatusCode
            return ResponseError exn
    }

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
