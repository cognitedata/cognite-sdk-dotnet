namespace Cognite.Sdk

open System
open System.Net

open FSharp.Data
open FSharp.Data.HttpRequestHeaders

/// Will be raised if decoding a response fails.
exception DecodeException of string

type HttpMethod =
    | POST
    | PUT
    | GET
    | DELETE

type QueryParams = (string*string) list

type Context = {
    Method: HttpMethod
    Body: string option
    Resource: string
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

module Request =
    /// A request function for fetching from the Cognite API.
    let fetch (ctx: Context) =
        async {
            let res = ctx.Resource
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

    /// Default context to use. Fetches from http://api.cognitedata.com.
    let defaultContext : Context = {
        Method = GET
        Body = None
        Resource = String.Empty
        Query = []
        Headers = [
            Accept HttpContentTypes.Json
            ContentType HttpContentTypes.Json
            UserAgent "CogniteSdk.NET; Dag Brattli"
        ]
        Fetch = fetch
        Project = ""
    }

    /// Add HTTP header to context.
    let addHeader (header: string*string) (context: Context) =
        { context with Headers = header :: context.Headers}

    /// Helper for setting Bearer token as Authorization header.
    let setToken (token: string) (context: Context) =
        let header = ("Authorization", sprintf "Bearer: %s" token)
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

    let setResource (resource: string) (context: Context) =
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
    let setMethod (method: HttpMethod) (context: Context) =
        { context with Method = method }


    /// **Description**
    ///
    /// Set the project to connect to.
    ///
    /// **Parameters**
    ///   * `project` - parameter of type `string`
    ///   * `context` - parameter of type `Context`
    ///
    /// **Output Type**
    ///   * `Context`
    ///
    let setProject (project: string) (context: Context) =
        { context with Project = project }


    /// **Description**
    ///
    /// Set the fetch method to be used for making requests. Should not
    /// be needed for most scenarios, but nice for unit-testing.
    ///
    /// **Parameters**
    ///   * `fetch` - parameter of type `Fetch`
    ///   * `context` - parameter of type `Context`
    ///
    /// **Output Type**
    ///   * `Context`
    ///
    let setFetch (fetch: Fetch) (context: Context) =
        { context with Fetch = fetch }
