namespace Cognite.Sdk

open System
open System.Net

open FSharp.Data
open FSharp.Data.HttpRequestHeaders

type HttpMethod =
    | POST
    | PUT
    | GET
    | DELETE

type QueryStringParams = (string*string) list

// type ResponseData =
//     | Text of string
//     | Binary of byte array

// type Response = {
//     HttpStatus: int
//     Data: ResponseData
// }

type HttpRequest = {
    Method: HttpMethod
    Body: string option
    Resource: string
    Query: QueryStringParams
    Headers: (string*string) list
    Project: string
}

and Context = {
    Request: HttpRequest
    Result: Result<HttpResponse, ResponseError>
    Fetch: Fetch
}

and Fetch = Context -> Async<Context>

module Request =
    let (|Informal|Success|Redirection|ClientError|ServerError|) x =
        if x < 200 then
            Informal
        else if x < 300 then
            Success
        else if x < 400 then
            Redirection
        else if x < 500 then
            ClientError
        else
            ServerError

    /// A request function for fetching from the Cognite API.
    let fetch (ctx: Context) =
        async {
            let res = ctx.Request.Resource
            let url = sprintf "https://api.cognitedata.com/api/0.5/projects/%s%s" ctx.Request.Project res
            let headers = ctx.Request.Headers
            let body = ctx.Request.Body |> Option.map HttpRequestBody.TextRequest
            let method = ctx.Request.Method.ToString().ToUpper()
            try
                let! response = Http.AsyncRequest (url, ctx.Request.Query, headers, method, ?body=body, silentHttpErrors=true)
                match response.StatusCode with
                | Success -> return { ctx with Result = Ok response }
                | _ -> return { ctx with Result = ErrorResponse response |> Error }
            with
            | ex -> return { ctx with Result = RequestException ex |> Error }
        }

    /// Default context to use. Fetches from http://api.cognitedata.com.
    let defaultRequest =
        {
            Method = GET
            Body = None
            Resource = String.Empty
            Query = []
            Headers = [
                Accept HttpContentTypes.Json
                ContentType HttpContentTypes.Json
                UserAgent "CogniteSdk.NET; Dag Brattli"
            ]
            Project = String.Empty
        }
    let defaultResult =
        Ok {
            StatusCode = 200
            Body = Text String.Empty
            ResponseUrl = String.Empty
            Headers = Map.empty
            Cookies = Map.empty
        }
    let defaultContext : Context = {
        Request = defaultRequest
        Result = defaultResult
        Fetch = fetch
    }

    /// Add HTTP header to context.
    let addHeader (header: string*string) (context: Context) =
        { context with Request = { context.Request with Headers = header :: context.Request.Headers  } }

    /// Helper for setting Bearer token as Authorization header.
    let setToken (token: string) (context: Context) =
        let header = ("Authorization", sprintf "Bearer: %s" token)
        { context with Request = { context.Request with Headers = header :: context.Request.Headers  } }

    /// **Description**
    ///
    /// Add query parameters to context. These parameters will be added
    /// to the query string of requests that uses this context.
    ///
    /// **Parameters**
    ///   * `query` - List of tuples (name, value)
    ///   * `context` - The context to add the query to.
    ///
    let addQuery (query: QueryStringParams) (context: Context) =
        { context with Request = { context.Request with Query = context.Request.Query @ query  } }

    let addQueryItem (query: string*string) (context: Context) =
        { context with Request = { context.Request with Query = query :: context.Request.Query  } }

    let setResource (resource: string) (context: Context) =
        { context with Request = { context.Request with Resource = resource  } }

    let setBody (body: string) (context: Context) =
        { context with Request = { context.Request with Body = Some body } }

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
        { context with Request = { context.Request with Method = method } }

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
        { context with Request = { context.Request with Project = project } }

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

    //let compose ()

    //[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    //module HttpResult =