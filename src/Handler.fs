namespace Fusion

open System
open System.Net.Http
open System.Text
open System.Web

open Microsoft.FSharp.Data.UnitSystems.SI

open FSharp.Control.Tasks.V2.ContextInsensitive
open System.IO
open Newtonsoft.Json
open Thoth.Json.Net
open System.Net.Http.Headers
open System.Net
open System.Threading.Tasks

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
            let next'  = second next
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
            { Request = Request.defaultRequest; Result = Ok a }

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
    let shouldRetry = function
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
            match err with
            | HttpResponse (httpResponse, _) ->
                if shouldRetry (int httpResponse.StatusCode) && maxRetries > 0 then
                    do! int initialDelay |> Async.Sleep
                    return! retry nextDelay (maxRetries - 1) handler next ctx
                else
                    return ctx'
            | Exception error ->
                match error with
                | :? Net.Http.HttpRequestException
                | :? System.Net.WebException as ex ->
                    if maxRetries > 0 then
                        do! int initialDelay |> Async.Sleep

                    return! retry nextDelay (maxRetries - 1) handler next ctx
                | _ ->
                    return ctx'
            | _ ->
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

    let setBody (body: JsonValue) (next: NextHandler<_,_>) (context: HttpContext) =
        next { context with Request = { context.Request with Body = Some body } }

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
    let setMethod<'a> (method: RequestMethod) (next: NextHandler<HttpResponseMessage,'a>) (context: HttpContext) =
        next { context with Request = { context.Request with Method = method; Body = None } }

    let setVersion (version: ApiVersion) (next: NextHandler<_,_>) (context: HttpContext) =
        next { context with Request = { context.Request with Version = version } }

    let GET<'a> = setMethod<'a> RequestMethod.GET
    let POST<'a> = setMethod<'a> RequestMethod.POST
    let DELETE<'a> = setMethod<'a> RequestMethod.DELETE

    let (|Informal|Success|Redirection|ClientError|ServerError|) x =
        if x < 200 then
            Informal
        else if x < 300 then
            Success
        else if x < 400 then
            Redirection
        else if x < 500 then
            ClientError
        else
            ServerError

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
            } :> Task
        override this.TryComputeLength(length: byref<int64>) : bool =
            length <- -1L
            false

    /// A request function for fetching from the Cognite API.
    let fetch<'a> (next: NextHandler<IO.Stream,'a>) (ctx: HttpContext) : Async<Context<'a>> =
        async {
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
            let client = ctx.Request.HttpClient

            for header, value in ctx.Request.Headers do
                if not (client.DefaultRequestHeaders.Contains header) then
                    client.DefaultRequestHeaders.Add (header, value)

            let resultTask = task {
                try
                    let! response = task {
                        match ctx.Request.Method with
                        | GET ->
                            return! client.GetAsync(url)
                        | POST ->
                            use content = new JsonPushStreamContent(ctx.Request.Body.Value)
                            return! client.PostAsync(url, content)
                        | PUT ->
                            use content = new JsonPushStreamContent(ctx.Request.Body.Value)
                            return! client.PutAsync(url, content)
                        | DELETE ->
                            return! client.DeleteAsync(url)
                    }

                    let! stream = response.Content.ReadAsStreamAsync ()
                    if response.IsSuccessStatusCode then
                        return Ok stream
                    else
                        return (response, stream) |> HttpResponse |> Error
                with
                | ex ->
                    return ResponseError.Exception ex |> Error
            }

            let! result = resultTask |> Async.AwaitTask
            return! next { Request = ctx.Request; Result = result }
        }
