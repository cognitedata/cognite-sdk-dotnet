namespace Cognite.Sdk

open FSharp.Data

type RequestBuilder () =
    member this.Zero () : HttpHandler<HttpResponse, HttpResponse, _> = fun next _ -> next Request.defaultContext
    member this.Return (res: 'a) : HttpHandler<HttpResponse, 'a, _> = fun next _ ->  next { Request = Request.defaultRequest; Result = Ok res }

    member this.Return (req: HttpRequest) : HttpHandler<HttpResponse, HttpResponse, _> = fun next ctx -> next { Request = req; Result = Request.defaultResult }

    member this.Delay (fn) = fn ()

    member this.Bind(source: HttpHandler<'a, 'b, 'd>, fn: 'b -> HttpHandler<'a, 'c, 'd>) :  HttpHandler<'a, 'c, 'd> =
        let handler1 (next1 : NextHandler<'c, 'd>) (ctx : Context<'a>)   =
            let handler2 (next2 : NextHandler<'b, 'd>) (ctx : Context<'b>)   =
                //failwith "error"
                match ctx.Result with
                | Ok res ->
                    let a = fn res
                    let b = a next2
                    //let c = b ctx
                    c
                | Error error ->
                    next2 { Request = ctx.Request; Result = Error error }

            source
        handler1


[<AutoOpen>]
module Builder =
    /// Request builder for an async context of request/result
    let fusion = RequestBuilder ()
