namespace Cognite.Sdk

open FSharp.Data
open Microsoft.FSharp.Data.UnitSystems.SI

[<Measure>] type ms

type HttpHandler<'a, 'b> = Context<'a> -> Async<Context<'b>>

type HttpHandler<'a> = HttpHandler<'a, 'a>

type HttpHandler = HttpHandler<HttpResponse>


[<AutoOpen>]
module Handler =
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

    let bindAsync (fn: Context<'a> -> Async<Context<'b>>) (a: Async<Context<'a>>) : Async<Context<'b>> = async {
        let! p = a
        match p.Result with
        | Ok _ ->
            return! fn p
        | Error err ->
            return { Request = p.Request; Result = Error err }
    }

    let compose (first : HttpHandler<'a, 'b>) (second : HttpHandler<'b, 'c>) : HttpHandler<'a,'c> =
        fun ctx -> bindAsync second (first ctx)

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
    let rec retry (initialDelay: int<ms>) (maxRetries : int) (handler: Context<'a> -> Async<Context<'b>>) (ctx: Context<'a>) : Async<Context<'b>> = async {
        // https://github.com/cognitedata/cdp-spark-datasource/blob/master/src/main/scala/com/cognite/spark/datasource/CdpConnector.scala#L170

        let exponentialDelay = min (secondsInMilliseconds * DefaultMaxBackoffDelay / 2) (initialDelay * 2)
        let randomDelayScale = min (secondsInMilliseconds * DefaultMaxBackoffDelay / 2) (initialDelay * 2)
        let nextDelay = rand.Next(int randomDelayScale) * 1<ms> + exponentialDelay

        let! ctx' = handler ctx

        match ctx'.Result with
        | Ok _ -> return ctx'
        | Error err ->
            match err with
            | ErrorResponse httpResponse ->
                if shouldRetry httpResponse.StatusCode && maxRetries > 0 then
                    do! int initialDelay |> Async.Sleep
                    return! retry nextDelay (maxRetries - 1) handler ctx
                else
                    return ctx'
            | RequestException error ->
                match error with
                | :? System.Net.WebException as ex ->
                    if maxRetries > 0 then
                        do! int initialDelay |> Async.Sleep

                    return! retry nextDelay (maxRetries - 1) handler ctx
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
    let concurrent (handlers : (Context<'a> -> Async<Context<'b>>) seq) (ctx: Context<'a>) : Async<Context<'b list>> = async {
        let! res =
            Seq.map (fun handler -> handler ctx) handlers
            |> Async.Parallel
            |> Async.map List.ofArray

        return res |> sequenceContext
    }