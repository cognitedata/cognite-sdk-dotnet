namespace Cognite.Sdk

module Functional =
    let bind (f: 'a -> Context<'b>) (a: Context<'a>) : Context<'b> =
        match a.Result with
        | Ok res ->
            f res
            //let ctx = f res
            //{ Request = a.Request; Result = ctx.Result }
        | Error err ->
            { Request = a.Request; Result = Error err }

type ContextBuilder() =
    member this.Zero () : HttpContext = Request.defaultContext
    member this.Return (res: 'a) : Context<'a> = { Request = Request.defaultRequest; Result = Ok res }
    //    Combine.concatSeq [xs; ys]
    member this.Delay (fn) = fn ()
    member this.Bind(source: Context<'a>, fn: 'a -> Context<'b>) : Context<'b> =
        Functional.bind fn source

[<AutoOpen>]
module Builder =
    /// Query builder for an async reactive event source
    let builder = ContextBuilder ()
