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

    type Context with
            //{ Request = Request.defaultRequest; Result = response; Fetch = this.Fetch}

        member this.Fetch (ctx: Context) = async {
            _request <- ctx.Request
            return ctx
        }

        static member FromJson (json: string) =
            let response = {
                StatusCode = 200
                Body = Text json
                ResponseUrl = String.Empty
                Headers = Map.empty
                Cookies = Map.empty
            }

            Ok response |> Fetcher


