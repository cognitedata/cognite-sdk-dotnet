namespace Cognite.Sdk.Api

open System
open System.Threading.Tasks

open Cognite.Sdk
open Cognite.Sdk.Request

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

    member this.SetFetch(fetch: Func<Context, Task<string>>) =
        let fetch' context = async {
            let! response = async {
                try
                    let! result = fetch.Invoke(context) |> Async.AwaitTask
                    return Ok result
                with
                | ex ->
                    return RequestException ex |> Error
            }
            return response
        }

        context
        |> setFetch fetch'
        |> Client

    /// <summary>
    /// Creates a Client for accessing the API.
    /// </summary>
    static member Create () =
        Client defaultContext
