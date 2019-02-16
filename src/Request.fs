namespace Cognite.Sdk

open System
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
    Method: Method
    Body: string option
    Resource: Resource
    Query: Params
    Headers: (string*string) list
    Fetch: Context -> Async<string>

    Project: string
}

module Request =

    let fetch (ctx: Context) =
        async {
            let (Resource res) = ctx.Resource
            let url = sprintf "https://api.cognitedata.com/api/0.5/projects/%s%s" ctx.Project res
            let headers = ctx.Headers
            let body = ctx.Body |> Option.map HttpRequestBody.TextRequest
            let method = ctx.Method.ToString().ToUpper()
            return! Http.AsyncRequestString (url, ctx.Query, headers, method, ?body=body)
        }

module Context =
    let defaultContext = {
        Method = Get
        Body = None
        Resource = Resource String.Empty
        Query = []
        Headers = [
            Accept HttpContentTypes.Json
            ContentType HttpContentTypes.Json
        ]
        Fetch = Request.fetch
        Project = ""
    }

    let addHeader (header: string*string) (context: Context) =
        { context with Headers = header :: context.Headers}

    let addQueries (queries: Params) (context: Context) =
        { context with Query = context.Query @ queries}

    let addQuery (query: string*string) (context: Context) =
        { context with Query = query :: context.Query }

    let setResource (resource: Resource) (context: Context) =
        { context with Resource = resource }

    let setBody (body: string) (context: Context) =
        { context with Body = Some body }

    let setMethod (method: Method) (context: Context) =
        { context with Method = method }

    let setProject (project: string) (context: Context) =
        { context with Project = project }

    let setFetch (fetch: Context -> Async<string>) (context: Context) =
        { context with Fetch = fetch }
