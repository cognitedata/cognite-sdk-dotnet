namespace Cognite.Sdk.Api

open System
open System.Threading.Tasks

open Cognite.Sdk
open Cognite.Sdk.Request

type HttpResponse (code: int, text: string) =
    member this.Code = code
    member this.Text = text

/// <summary>
/// Client for making requests to the API.
/// </summary>
/// <param name="context">Context to use for this session.</param>
type Client private (fetch: HttpHandler, context: HttpContext) =
    let context = context
    let fetch : HttpHandler = fetch

    /// Create new client with a default context (e.g will connect to CDF when used.)
    new () = Client (Request.fetch, defaultContext)

    member internal __.Ctx =
        context

    member internal __.Fetch =
        fetch

    /// <summary>
    /// Add header for accessing the API.
    /// </summary>
    /// <param name="project">Name of project.</param>
    member this.AddHeader (name: string, value: string)  =
        context
        |> addHeader (name, value)
        |> Client.New fetch

    /// <summary>
    /// Set project for accessing the API.
    /// </summary>
    /// <param name="project">Name of project.</param>
    member this.SetProject (project: string) =
        context
        |> setProject project
        |> Client.New fetch

    /// **Description**
    ///
    /// Set the fetch method to be used for making requests. Should not
    /// be needed for most scenarios, but nice for unit-testing.
    ///
    /// **Parameters**
    ///   * `handler` - Fetch handler to use for making requests.
    ///
    /// **Output Type**
    ///   * `Client`
    ///
    member this.SetFetch (handler: Func<HttpContext, Task<HttpResponse>>) =
        let fetch' context = async {
            let! response = async {
                try
                    let! result = handler.Invoke(context) |> Async.AwaitTask
                    let httpResponse : FSharp.Data.HttpResponse = {
                        StatusCode = result.Code
                        Body = FSharp.Data.Text result.Text
                        ResponseUrl = String.Empty
                        Headers = Map.empty
                        Cookies = Map.empty
                    }
                    match result.Code with
                    | Success ->
                        return { context with Result = Ok httpResponse }
                    | _ ->
                        return { context with Result = Error (ErrorResponse httpResponse) }
                with
                | ex ->
                    return { context with Result = RequestException ex |> Error }
            }
            return response
        }

        Client.New fetch' context

    /// <summary>
    /// Creates a Client for accessing the API.
    /// </summary>
    static member Create () =
        Client ()

    static member private New (fetch: HttpHandler) (context: HttpContext)  =
        Client (fetch, context)
