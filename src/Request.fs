namespace Cognite.Sdk

open FSharp.Data
open FSharp.Data.HttpRequestHeaders

type Context = {
    ApiKey: string
    Project: string
}

type Methods =
    | Post
    | Put
    | Get
    | Delete

module Request =

    let fetch (method: Methods) (ctx: Context) (resource: string) (query: (string*string) list) =
        async {
            let url = sprintf "https://api.cognitedata.com/api/0.5/projects/%s%s" ctx.Project resource
            let headers = [
                Accept HttpContentTypes.Json
                ContentType HttpContentTypes.Json
                ("api-key", ctx.ApiKey)
            ]
            return! Http.AsyncRequestString (url, headers=headers, httpMethod=method.ToString().ToUpper(), query=query)
        }