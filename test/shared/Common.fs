namespace Tests

open System.IO
open System.Text

open Oryx
open CogniteSdk
open System.Net.Http


[<RequireQualifiedAccess>]
module Result =
    let isOk = function
        | Ok _ -> true
        | Error _ -> false

    let isError res = not (isOk res)

module Fetch =

    let fromJson (json: string) (next: NextFunc<HttpResponseMessage,_>) (ctx: HttpContext) =
        let byteArray = Encoding.ASCII.GetBytes( json )
        let stream = new MemoryStream( byteArray ) :> Stream
        let streamContent = new StreamContent(stream)
        let response = new HttpResponseMessage(Content=streamContent)
        next { Request = ctx.Request; Result = Ok response }


