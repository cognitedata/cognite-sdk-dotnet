namespace Cognite.Sdk

open System
open System.Net

open FSharp.Data
open FSharp.Data.HttpRequestHeaders

type Method =
    | Post
    | Put
    | Get
    | Delete

type QueryParams = (string*string) list
type Resource = Resource of string

type Context = {
    Method: Method
    Body: string option
    Resource: Resource
    Query: QueryParams
    Headers: (string*string) list
    Fetch: Fetch

    Project: string
}

and Fetch = Context -> Async<Result<string, exn>>

type RequestError = {
    Code: int
    Message: string
    Extra: Map<string, string>
}

exception DecodeException of string

module Request =

    /// A request function for fetching from the Cognite API.
    let fetch (ctx: Context) =
        async {
            let (Resource res) = ctx.Resource
            let url = sprintf "https://api.cognitedata.com/api/0.5/projects/%s%s" ctx.Project res
            let headers = ctx.Headers
            let body = ctx.Body |> Option.map HttpRequestBody.TextRequest
            let method = ctx.Method.ToString().ToUpper()
            try
                let! response = Http.AsyncRequestString (url, ctx.Query, headers, method, ?body=body)
                return Ok response
            with
            | ex -> return Error ex
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
            UserAgent "CogniteNetSdk; Dag Brattli"
        ]
        Fetch = Request.fetch
        Project = ""
    }

    /// Add HTTP header to context.
    let addHeader (header: string*string) (context: Context) =
        { context with Headers = header :: context.Headers}

    /// **Description**
    ///
    /// Add query parameters to context. These parameters will be added
    /// to the query string of requests that uses this context.
    /// 
    /// **Parameters**
    ///   * `query` - List of tuples (name, value)
    ///   * `context` - The context to add the query to.
    ///
    let addQuery (query: QueryParams) (context: Context) =
        { context with Query = context.Query @ query}

    let addQueryItem (query: string*string) (context: Context) =
        { context with Query = query :: context.Query }

    let setResource (resource: Resource) (context: Context) =
        { context with Resource = resource }

    let setBody (body: string) (context: Context) =
        { context with Body = Some body }


    /// **Description**
    ///
    /// Set the method to be used for requests using this context. 
    /// 
    /// **Parameters**
    ///   * `method` - Method is a parameter of type `Method` and can be 
    ///     `Put`, `Get`, `Post` or `Delete`. 
    ///   * `context` - parameter of type `Context`
    ///
    /// **Output Type**
    ///   * `Context`
    ///
    let setMethod (method: Method) (context: Context) =
        { context with Method = method }

    let setProject (project: string) (context: Context) =
        { context with Project = project }

    let setFetch (fetch: Fetch) (context: Context) =
        { context with Fetch = fetch }
