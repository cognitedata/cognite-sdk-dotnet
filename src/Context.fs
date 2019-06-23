namespace Cognite.Sdk

open System
open System.Net
open System.Net.Http
open System.Reflection

open System.Collections.Specialized

//open FSharp.Data
open FSharp.Data.HttpRequestHeaders
open System.Net.Http.Headers

type RequestMethod =
    | POST
    | PUT
    | GET
    | DELETE

//type QueryStringParams = (string*string) list

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
    ClientFactory: IHttpClientFactory
    Method: HttpMethod
    Body: string option
    Resource: string
    Query:NameValueCollection
    Headers: (string*string) list
    Project: string
    Version: ApiVersion
}

type Context<'a> = {
    Request: HttpRequest
    Result: Result<'a, ResponseError>
}

type HttpContext = Context<HttpResponseMessage>

module Request =
    let version =
        let version = Assembly.GetExecutingAssembly().GetName().Version
        {| Major=version.Major; Minor=version.Minor; Revision=version.Revision |}

    /// Default context to use. Fetches from http://api.cognitedata.com.
    let defaultRequest =
        {
            ClientFactory = null
            Method = HttpMethod.Get
            Body = None
            Resource = String.Empty
            Query = NameValueCollection ()
            Headers = [
                Accept "application/json"
                ContentType "application/json"
                UserAgent (sprintf "Fusion.NET / v%d.%d.%d (Dag Brattli)" version.Major version.Minor version.Revision)
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

    let setClientFactory (factory: IHttpClientFactory) (context: HttpContext) =
        { context with Request = { context.Request with ClientFactory = factory } }