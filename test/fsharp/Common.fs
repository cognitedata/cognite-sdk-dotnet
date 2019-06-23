namespace Tests

open System
open FSharp.Data
open Cognite.Sdk


[<RequireQualifiedAccess>]
module Result =
    let isOk = function
        | Ok _ -> true
        | Error _ -> false

    let isError res = not (isOk res)

module Fetch =
    let fromJson (json: string) (next: NextHandler<string,_>) (ctx: HttpContext) =
        next { Request = ctx.Request; Result = Ok json }


