namespace Fusion.Api

open Fusion
open Fusion.Request
open Microsoft.Extensions.DependencyInjection
open System.Net.Http



/// <summary>
/// Client for making requests to the API.
/// </summary>
/// <param name="context">Context to use for this session.</param>
type Client private (context: HttpContext) =
    let context = context
    let fetch  = fetch

    /// Create new client with a default context (e.g will connect to CDF when used.)
    new () = Client (defaultContext)

    member internal __.Ctx =
        context

    /// <summary>
    /// Add header for accessing the API.
    /// </summary>
    /// <param name="project">Name of project.</param>
    member this.AddHeader (name: string, value: string)  =
        context
        |> addHeader (name, value)
        |> Client.New

    /// <summary>
    /// Set project for accessing the API.
    /// </summary>
    /// <param name="project">Name of project.</param>
    member this.SetProject (project: string) =
        context
        |> setProject project
        |> Client.New

    member this.SetHttpClient (client: HttpClient) =
        context
        |> setHttpClient client
        |> Client.New

    /// <summary>
    /// Creates a Client for accessing the API.
    /// </summary>
    static member Create (client: HttpClient) =
        Client().SetHttpClient(client)

    static member Create () =
        Client ()

    static member private New (context: HttpContext)  =
        Client (context)
