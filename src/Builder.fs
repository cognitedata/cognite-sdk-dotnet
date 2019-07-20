namespace Fusion

//open FSharp.Data
open System.Net
open System.Net.Http

type RequestBuilder () =
    member this.Zero () : HttpHandler<HttpResponseMessage, HttpResponseMessage, _> = fun next _ -> next defaultContext
    member this.Return (res: 'a) : HttpHandler<HttpResponseMessage, 'a, _> = fun next _ ->  next { Request = defaultRequest; Result = Ok res }

    member this.Return (req: HttpRequest) : HttpHandler<HttpResponseMessage, HttpResponseMessage, _> = fun next ctx -> next { Request = req; Result = defaultResult }

    member this.Delay (fn) = fn ()

    member this.Bind(source: HttpHandler<'a, 'b, 'd>, fn: 'b -> HttpHandler<'a, 'c, 'd>) :  HttpHandler<'a, 'c, 'd> =
        fun (next : NextHandler<'c, 'd>) (ctx : Context<'a>) ->
            let next' (cb : Context<'b>)  = async {
                match cb.Result with
                | Ok b ->
                    let inner = fn b
                    return! inner next ctx
                | Error error ->
                    return! next { Request = cb.Request; Result = Error error }
            }
            // Run source
            source next' ctx

[<AutoOpen>]
module Builder =
    /// Request builder for an async context of request/result
    let fusion = RequestBuilder ()
