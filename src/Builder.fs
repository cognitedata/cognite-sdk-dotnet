namespace Cognite.Sdk

open FSharp.Data


type RequestBuilder () =
    member this.Zero () : HttpHandler = fun _ -> Async.single Request.defaultContext
    member this.Return (res: 'a) : (_ -> Async<Context<'a>>) = fun _ -> Async.single { Request = Request.defaultRequest; Result = Ok res }

    member this.Return (req: HttpRequest) : HttpHandler = fun ctx -> Async.single { Request = Request.defaultRequest; Result = Request.defaultResult }

    member this.Delay (fn) = fn ()

    member this.Bind(source: HttpHandler<'a, 'b>, fn: 'b -> HttpHandler<'a, 'c>) :  HttpHandler<'a, 'c> =
        let fn' (acb: Async<Context<'b>>) (ctx: Context<'a>) : Async<Context<'c>> = async {
            let! cb = acb
            match cb.Result with
            | Ok res ->
                return! (fn res) ctx
            | Error error ->
                return { Request = cb.Request; Result = Error error }
        }
        (fun ctx -> fn' (source ctx) ctx)

[<AutoOpen>]
module Builder =
    /// Query builder for an async reactive event source
    let req = RequestBuilder ()
