namespace Fusion

open System
open System.Net
open System.Net.Http
open System.Net.Http.Headers
open System.IO
open System.Text
open System.Threading.Tasks
open System.Web

open Microsoft.FSharp.Data.UnitSystems.SI

open FSharp.Control.Tasks.V2.ContextInsensitive
open Google.Protobuf
open Newtonsoft.Json
open Thoth.Json.Net

[<Measure>] type ms

type NextHandler<'a, 'b> = Context<'a> -> Async<Context<'b>>

type HttpHandler<'a, 'b, 'c> = NextHandler<'b, 'c> -> Context<'a> -> Async<Context<'c>>

type HttpHandler<'a, 'b> = HttpHandler<'a, 'a, 'b>

type HttpHandler<'a> = HttpHandler<HttpResponseMessage, 'a>

type HttpHandler = HttpHandler<HttpResponseMessage, HttpResponseMessage>

[<AutoOpen>]
module Handler =

    /// **Description**
    ///
    /// Run the handler in the given context.
    ///
    /// **Parameters**
    ///   * `handler` - parameter of type `HttpHandler<'a,'b,'b>`
    ///   * `ctx` - parameter of type `Context<'a>`
    ///
    /// **Output Type**
    ///   * `Async<Result<'b,ResponseError>>`
    ///
    let runHandler (handler: HttpHandler<'a,'b,'b>) (ctx : Context<'a>) : Async<Result<'b,ResponseError>> =
        async {
            let! a = handler Async.single ctx
            return a.Result
        }

    let private secondsInMilliseconds = 1000<ms/UnitSymbols.s>  // relation between seconds and millisecond

    [<Literal>]
    let DefaultInitialRetryDelay = 150<ms>
    [<Literal>]
    let DefaultMaxBackoffDelay = 120<UnitSymbols.s>

    let rand = System.Random ()

    let bind fn ctx =
        match ctx.Result with
        | Ok res ->
            fn res
        | Error err ->
            { Request = ctx.Request; Result = Error err }

    let bindAsync (fn: Context<'a> -> Async<Context<'b>>) (a: Async<Context<'a>>) : Async<Context<'b>> =
        async {
            let! p = a
            match p.Result with
            | Ok _ ->
                return! fn p
            | Error err ->
                return { Request = p.Request; Result = Error err }
        }

    let compose (first : HttpHandler<'a, 'b, 'd>) (second : HttpHandler<'b, 'c, 'd>) : HttpHandler<'a,'c,'d> =
        fun (next: NextHandler<_, _>) (ctx : Context<'a>) ->
            let next' = second next
            let next'' = first next'

            next'' ctx

    let (>>=) a b =
        bindAsync b a

    let (>=>) a b =
        compose a b

    // https://fsharpforfunandprofit.com/posts/elevated-world-4/
    let traverseContext fn (list : Context<'a> list) =
        // define the monadic functions
        let (>>=) ctx fn = bind fn ctx

        let retn a =
            { Request = Context.defaultRequest; Result = Ok a }

        // define a "cons" function
        let cons head tail = head :: tail

        // right fold over the list
        let initState = retn []
        let folder head tail =
            fn head >>= (fun h ->
                tail >>= (fun t ->
                    retn (cons h t)
                )
            )

        List.foldBack folder list initState

    let sequenceContext (ctx : Context<'a> list) : Context<'a list> = traverseContext id ctx

    // https://github.com/cognitedata/cdp-spark-datasource/blob/master/src/main/scala/com/cognite/spark/datasource/CdpConnector.scala#L198
    let shouldRetry (err: ResponseError) =
        let retryCode =
            match err.Code with
            // @larscognite: Retry on 429,
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

        let retryEx =
            if err.InnerException.IsSome then
                match err.InnerException.Value with
                | :? Net.Http.HttpRequestException
                | :? System.Net.WebException -> true
                // do not retry other exceptions.
                | _ -> false
            else
                false

        // Retry if retriable code or retryable exception
        retryCode || retryEx

    /// **Description**
    ///
    /// Retries the given HTTP handler up to `maxRetries` retries with
    /// exponential backoff and up to 2 minute with randomness.
    ///
    /// **Parameters**
    ///   * `maxRetries` - max number of retries.
    ///   * `initialDelay` -
    ///   * `ctx` - parameter of type `Context<'a>`
    ///
    /// **Output Type**
    ///   * `Async<Context<'a>>`
    ///
    let rec retry (initialDelay: int<ms>) (maxRetries : int) (handler: HttpHandler<'a,'b,'c>) (next: NextHandler<'b,'c>) (ctx: Context<'a>) : Async<Context<'c>> = async {
        // https://github.com/cognitedata/cdp-spark-datasource/blob/master/src/main/scala/com/cognite/spark/datasource/CdpConnector.scala#L170

        let exponentialDelay = min (secondsInMilliseconds * DefaultMaxBackoffDelay / 2) (initialDelay * 2)
        let randomDelayScale = min (secondsInMilliseconds * DefaultMaxBackoffDelay / 2) (initialDelay * 2)
        let nextDelay = rand.Next(int randomDelayScale) * 1<ms> + exponentialDelay

        let! ctx' = handler next ctx

        match ctx'.Result with
        | Ok _ -> return ctx'
        | Error err ->
            if shouldRetry err && maxRetries > 0 then
                do! int initialDelay |> Async.Sleep
                return! retry nextDelay (maxRetries - 1) handler next ctx
            else
                return ctx'
    }

    /// **Description**
    ///
    /// 10000 concurrently. 500-100 in each.
    ///
    /// **Parameters**
    ///   * `handlers` - parameter of type `HttpHandler list`
    ///   * `ctx` - parameter of type `Context<'a>`
    ///
    /// **Output Type**
    ///   * `Async<Context<'a>>`
    ///
    /// **Exceptions**
    ///
    let concurrent (handlers : HttpHandler<'a, 'b, 'b> seq) (next: NextHandler<'b list, 'c>) (ctx: Context<'a>) : Async<Context<'c>> = async {
        let! res =
            Seq.map (fun handler -> handler Async.single ctx) handlers
            |> Async.Parallel
            |> Async.map List.ofArray

        return! next (res |> sequenceContext)
    }

    /// **Description**
    ///
    /// Add query parameters to context. These parameters will be added
    /// to the query string of requests that uses this context.
    ///
    /// **Parameters**
    ///   * `query` - List of tuples (name, value)
    ///   * `context` - The context to add the query to.
    ///
    let addQuery (query: (string * string) list) (next: NextHandler<_,_>) (context: HttpContext) =
        next { context with Request = { context.Request with Query = query } }

    let setResource (resource: string) (next: NextHandler<_,_>) (context: HttpContext) =
        next { context with Request = { context.Request with Resource = resource  } }

    let setContent (content: Content) (next: NextHandler<_,_>) (context: HttpContext) =
        next { context with Request = { context.Request with Content = Some content } }

    let setResponseType (respType: Response) (next: NextHandler<_,_>) (context: HttpContext) =
        next { context with Request = { context.Request with ResponseType = respType }}

    /// **Description**
    ///
    /// Set the method to be used for requests using this context.
    ///
    /// **Parameters**
    ///   * `method` - Method is a parameter of type `Method` and can be
    ///     `Put`, `Get`, `Post` or `Delete`.
    ///   * `context` - parameter of type `Context`
    ///
    /// **Output Type**
    ///   * `Context`
    ///
    let setMethod<'a> (method: HttpMethod) (next: NextHandler<HttpResponseMessage,'a>) (context: HttpContext) =
        next { context with Request = { context.Request with Method = method; Content = None } }

    let setVersion (version: ApiVersion) (next: NextHandler<_,_>) (context: HttpContext) =
        next { context with Request = { context.Request with Version = version } }

    let GET<'a> = setMethod<'a> HttpMethod.Get
    let POST<'a> = setMethod<'a> HttpMethod.Post
    let DELETE<'a> = setMethod<'a> HttpMethod.Delete

    let decodeStreamAsync (decoder : Decoder<'a>) (stream : IO.Stream) =
        task {
            use tr = new StreamReader(stream) // StreamReader will dispose the stream
            use jtr = new JsonTextReader(tr)
            let! json = Newtonsoft.Json.Linq.JValue.ReadFromAsync jtr

            return Decode.fromValue "$" decoder json
        }

    /// HttpContent implementation to push a JsonValue directly to the output stream.
    type JsonPushStreamContent (content : JsonValue) =
        inherit HttpContent ()
        let _content = content
        do
            base.Headers.ContentType <- MediaTypeHeaderValue("application/json")
        override this.SerializeToStreamAsync(stream: Stream, context: TransportContext) : Task =
            task {
                use sw = new StreamWriter(stream, UTF8Encoding(false), 1024, true)
                use jtw = new JsonTextWriter(sw, Formatting = Formatting.None)
                jtw.Formatting <- Formatting.None
                do! content.WriteToAsync(jtw)
                do! jtw.FlushAsync()
                return ()
            } :> _
        override this.TryComputeLength(length: byref<int64>) : bool =
            length <- -1L
            false

    type ProtobufPushStreamContent (content : IMessage) =
        inherit HttpContent ()
        let _content = content
        do
            base.Headers.ContentType <- MediaTypeHeaderValue("application/protobuf")
        override this.SerializeToStreamAsync(stream: Stream, context: TransportContext) : Task =
            task {
                content.WriteTo(stream)
            } :> _
        override this.TryComputeLength(length: byref<int64>) : bool =
            length <- -1L
            false

    let buildRequest (client: HttpClient) (ctx: HttpContext) : HttpRequestMessage =
        let res = ctx.Request.Resource
        let query = ctx.Request.Query
        let url =
            let result = sprintf "%s/api/%s/projects/%s%s" ctx.Request.ServiceUrl (ctx.Request.Version.ToString ()) ctx.Request.Project res
            if Seq.isEmpty query then
                result
            else
                let queryString = HttpUtility.ParseQueryString(String.Empty)
                for (key, value) in query do
                    queryString.Add (key, value)
                sprintf "%s?%s" result (queryString.ToString ())


        let request = new HttpRequestMessage (ctx.Request.Method, url)

        let appIdHeader =
            match ctx.Request.AppId with
            | Some appId -> ("x-cdp-app", appId)
            | None -> failwith "Client must set the application ID (appId)"

        let contentHeader =
            match ctx.Request.ResponseType with
            | JsonValue -> ("Accept", "application/json")
            | Protobuf -> ("Accept", "application/protobuf")

        for header, value in contentHeader :: appIdHeader :: ctx.Request.Headers do
            if not (client.DefaultRequestHeaders.Contains header) then
                request.Headers.Add (header, value)

        if ctx.Request.Content.IsSome then
            let content =
                match ctx.Request.Content.Value with
                    | CaseJsonValue value -> new JsonPushStreamContent (value) :> HttpContent
                    | CaseProtobuf value -> new ProtobufPushStreamContent (value) :> HttpContent
            request.Content <- content
        request

    let sendRequest (request: HttpRequestMessage) (client: HttpClient) : Task<Result<Stream, ResponseError>> =
        task {
            try
                let! response = client.SendAsync request
                let! stream = response.Content.ReadAsStreamAsync ()
                if response.IsSuccessStatusCode then
                    return Ok stream
                else
                    let! result = decodeStreamAsync ApiResponseError.Decoder stream
                    match result with
                    | Ok apiResponseError ->
                        return apiResponseError.Error |> Error
                    | Error message ->
                        return {
                            ResponseError.empty with
                                Code = int response.StatusCode
                                Message = message
                        } |> Error
            with
            | ex ->
                return {
                    ResponseError.empty with
                        Code = 400
                        Message = ex.Message
                        InnerException = Some ex
                } |> Error
        }

    let fetch<'a> (next: NextHandler<IO.Stream, 'a>) (ctx: HttpContext) : Async<Context<'a>> =
        async {
            let client =
                match ctx.Request.HttpClient with
                | Some client -> client
                | None -> failwith "Must set httpClient"
            use message = buildRequest client ctx

            let! result = sendRequest message client |> Async.AwaitTask
            if ctx.Request.Content.IsSome then
                message.Content.Dispose ()
            return! next { Request = ctx.Request; Result = result }
        }
