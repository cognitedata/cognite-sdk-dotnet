namespace Tests

open FSharp.Data
open Cognite.Sdk


type Fetcher (response: Result<HttpResponse, ResponseError>) =
    let mutable _ctx: Context option = None

    member this.Ctx =
        _ctx

    member this.Fetch (ctx: Context) = async {
        _ctx <- Some ctx
        return response
    }

    static member FromJson (json: string) =
        let response = {
            StatusCode = 200
            Body = Text json
            ResponseUrl = ""
            Headers = Map.empty
            Cookies = Map.empty
        }

        Ok response |> Fetcher


