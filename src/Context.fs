namespace Fusion

open System
open System.Net
open System.Net.Http
open System.Reflection

type RequestMethod =
    | POST
    | PUT
    | GET
    | DELETE

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
    HttpClient: HttpClient
    Method: RequestMethod
    Body: string option
    Resource: string
    Query: (string * string) list
    Headers: (string * string) list
    Project: string
    Version: ApiVersion
}

type Context<'a> = {
    Request: HttpRequest
    Result: Result<'a, ResponseError>
}

type HttpContext = Context<HttpResponseMessage>

[<AutoOpen>]
module Request =
    let version =
        let version = Assembly.GetExecutingAssembly().GetName().Version
        {| Major=version.Major; Minor=version.Minor; Build=version.Build |}

    /// Default context to use. Fetches from http://api.cognitedata.com.
    let defaultRequest =
        let ua = sprintf "Fusion.Net / v%d.%d.%d (Cognite)" version.Major version.Minor version.Build
        {
            HttpClient = null
            Method = RequestMethod.GET
            Body = None
            Resource = String.Empty
            Query = List.empty
            Headers = [
                ("Accept", "application/json")
                ("User-Agent", ua)
                ("x-cdp-sdk", ua)
            ]
            Project = String.Empty
            Version = V05
        }
    let defaultResult =
        Ok (new HttpResponseMessage (HttpStatusCode.NotFound))

    let defaultContext : Context<HttpResponseMessage> = {
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

    let setHttpClient (client: HttpClient) (context: HttpContext) =
        { context with Request = { context.Request with HttpClient = client } }
