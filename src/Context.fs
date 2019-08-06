namespace Fusion


open System
open System.Net
open System.Net.Http
open System.Reflection
open Thoth.Json.Net

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

type Content =
    internal
    | CaseJsonValue of JsonValue
    | CaseProtobuf of Google.Protobuf.IMessage

    static member JsonValue jsonValue = CaseJsonValue jsonValue
    static member Protobuf protobuf = CaseProtobuf protobuf

type Response = JsonValue | Protobuf

type HttpRequest = {
    HttpClient: HttpClient option
    Method: HttpMethod
    Content: Content option
    Resource: string
    Query: (string * string) list
    ResponseType: Response
    Headers: (string * string) list
    Project: string
    Version: ApiVersion
    ServiceUrl: string
    AppId: string option
}

type Context<'a> = {
    Request: HttpRequest
    Result: Result<'a, ResponseError>
}

type HttpContext = Context<HttpResponseMessage>

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Context =
    let private version =
        let version = Assembly.GetExecutingAssembly().GetName().Version
        {| Major=version.Major; Minor=version.Minor; Build=version.Build |}

    /// Default context to use. Fetches from http://api.cognitedata.com.
    let internal defaultRequest =
        let ua = sprintf "Fusion.NET / v%d.%d.%d (Cognite)" version.Major version.Minor version.Build
        {
            HttpClient = None
            Method = HttpMethod.Get
            Content = None
            Resource = String.Empty
            Query = List.empty
            ResponseType = JsonValue
            Headers = [
                ("User-Agent", ua)
                ("x-cdp-sdk", ua)
            ]
            Project = String.Empty
            Version = V05
            ServiceUrl = "https://api.cognitedata.com"
            AppId = None
        }
    let internal defaultResult =
        Ok (new HttpResponseMessage (HttpStatusCode.NotFound))

    let internal defaultContext : Context<HttpResponseMessage> = {
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
    let setProject (project: string) (context: HttpContext) =
        { context with Request = { context.Request with Project = project } }

    let setHttpClient (client: HttpClient) (context: HttpContext) =
        { context with Request = { context.Request with HttpClient = Some client } }

    let setAppId (appId: string) (context: HttpContext) =
        { context with Request = { context.Request with AppId = Some appId } }

    let setServiceUrl (serviceUrl: string) (context: HttpContext) =
        { context with Request = { context.Request with ServiceUrl = serviceUrl } }

    /// Create new context with default values.
    let create () =
        defaultContext
