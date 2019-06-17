namespace Cognite.Sdk

open System
open System.Net
open System.Reflection

open FSharp.Data
open FSharp.Data.HttpRequestHeaders

type HttpMethod =
    | POST
    | PUT
    | GET
    | DELETE

type QueryStringParams = (string*string) list

type ApiVersion =
    | V05
    | V06
    | V10

    override this.ToString () =
        match this with
        | V05 -> "0.5"
        | V06 -> "0.6"
        | V10 -> "v1"

type HttpRequest = {
    Method: HttpMethod
    Body: string option
    Resource: string
    Query: QueryStringParams
    Headers: (string*string) list
    Project: string
    Version: ApiVersion
}

type Context<'a> = {
    Request: HttpRequest
    Result: Result<'a, ResponseError>
}

type HttpContext = Context<HttpResponse>

module Request =
    let version =
        let version = Assembly.GetExecutingAssembly().GetName().Version
        {| Major=version.Major; Minor=version.Minor; Revision=version.Revision|}

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
                UserAgent (sprintf "Fusion.NET / v%d.%d.%d (Dag Brattli)" version.Major version.Minor version.Revision)
            ]
            Project = String.Empty
            Version = V05
        }
    let defaultResult =
        Ok {
            StatusCode = 404
            Body = Text String.Empty
            ResponseUrl = String.Empty
            Headers = Map.empty
            Cookies = Map.empty
        }
    let defaultContext : Context<HttpResponse> = {
        Request = defaultRequest
        Result = defaultResult
    }

    /// Add HTTP header to context.
    let addHeader (header: string*string) (context: HttpContext) =
        { context with Request = { context.Request with Headers = header :: context.Request.Headers  } }

    /// Helper for setting Bearer token as Authorization header.
    let setToken (token: string) (context: HttpContext) =
        let header = ("Authorization", sprintf "Bearer: %s" token)
        { context with Request = { context.Request with Headers = header :: context.Request.Headers  } }

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
    let setProject (project: string) (context: HttpContext) =
        { context with Request = { context.Request with Project = project } }
