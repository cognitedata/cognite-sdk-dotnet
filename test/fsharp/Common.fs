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
    let fromJson (json: string) (next: NextHandler<HttpResponse,_>) (ctx: HttpContext) =
        let response = {
            StatusCode = 200
            Body = Text json
            ResponseUrl = String.Empty
            Headers = Map.empty
            Cookies = Map.empty
        }

        next { Request = ctx.Request; Result = Ok response }


