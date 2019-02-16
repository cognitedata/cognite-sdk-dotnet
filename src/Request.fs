namespace Cognite.Sdk

open FSharp.Data
open FSharp.Data.HttpRequestHeaders

type Method =
    | Post
    | Put
    | Get
    | Delete

type Params = (string*string) list
type Resource = Resource of string

type Context = {
    ApiKey: string
    Project: string
    Fetch: Method -> Context -> Resource -> Params -> Async<string>
}

module Request =

    let fetch (method: Method) (ctx: Context) (resource: Resource) (query: Params) =
        async {
            let (Resource res) = resource
            let url = sprintf "https://api.cognitedata.com/api/0.5/projects/%s%s" ctx.Project res
            let headers = [
                Accept HttpContentTypes.Json
                ContentType HttpContentTypes.Json
                ("api-key", ctx.ApiKey)
            ]

            return! Http.AsyncRequestString (url, headers=headers, httpMethod=method.ToString().ToUpper(), query=query)
        }