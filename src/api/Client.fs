namespace Cognite.Sdk.Api

open Cognite.Sdk
open Cognite.Sdk.Context


/// <summary>
/// Client for accessing the API.
/// </summary>
/// <param name="context">Context to use for this session.</param>
type Client private (context: Context) =
    let context = context

    new() = Client(defaultContext)

    member internal __.Ctx =
        context

    /// <summary>
    /// Add header for accessing the API.
    /// </summary>
    /// <param name="project">Name of project.</param>
    member this.AddHeader(name: string, value: string)  =
        context
        |> addHeader (name, value)
        |> Client

    /// <summary>
    /// Set project for accessing the API.
    /// </summary>
    /// <param name="project">Name of project.</param>
    member this.SetProject(project: string) =
        context
        |> setProject project
        |> Client

    /// <summary>
    /// Creates a Client for accessing the API.
    /// </summary>
    static member Create () =
        Client defaultContext
