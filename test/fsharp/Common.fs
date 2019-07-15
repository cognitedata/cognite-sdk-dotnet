namespace Tests

open System
open System.IO
open Cognite.Sdk
open System.Text


[<RequireQualifiedAccess>]
module Result =
    let isOk = function
        | Ok _ -> true
        | Error _ -> false

    let isError res = not (isOk res)

module Fetch =

    let fromJson (json: string) (next: NextHandler<Stream,_>) (ctx: HttpContext) =
        let byteArray = Encoding.ASCII.GetBytes( json )
        let stream = new MemoryStream( byteArray ) :> Stream
        next { Request = ctx.Request; Result = Ok stream }


