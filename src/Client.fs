namespace CogniteSdk

open System.Net.Http
open Oryx

module ClientExtensions =
    type Assets internal (context: HttpContext) =
        member internal __.Ctx =
            context

    type TimeSeries internal (context: HttpContext) =
        member internal __.Ctx =
            context

    type DataPoints internal (context: HttpContext) =
        member internal __.Ctx =
            context

    type Events internal (context: HttpContext) =
        member internal __.Ctx =
            context

/// <summary>
/// Client for making requests to the API.
/// </summary>
/// <param name="context">Context to use for this session.</param>
type Client private (context: HttpContext) =
    let context = context

    /// For use with AddHttpClient dependency injection
    new (httpClient: HttpClient) =
        let context =
            Context.create ()
            |> Context.setUrlBuilder Context.urlBuilder
            |> Context.setHttpClient httpClient
        Client context

    member internal __.Ctx =
        context

    /// <summary>
    /// Add header for accessing the API.
    /// </summary>
    /// <param name="project">Name of project.</param>
    member this.AddHeader (name: string, value: string) =
        context
        |> Context.addHeader (name, value)
        |> Client

    /// <summary>
    /// Set project for accessing the API.
    /// </summary>
    /// <param name="project">Name of project.</param>
    member this.SetProject (project: string) =
        context
        |> Context.setProject project
        |> Client

    member this.SetAppId (appId: string) =
        context
        |> Context.setAppId appId
        |> Client

    member this.SetHttpClient (client: HttpClient) =
        context
        |> Context.setHttpClient client
        |> Client

    member this.SetServiceUrl (serviceUrl: string) =
        context
        |> Context.setServiceUrl serviceUrl
        |> Client

    /// <summary>
    /// Creates a Client for accessing the API.
    /// </summary>
    static member Create () =
        let context =
            Context.create ()
            |> Context.setUrlBuilder Context.urlBuilder

        Client context

    /// Client Assets extension methods
    member val Assets = Assets.ClientExtension context with get
    /// Client TimeSeries extension methods
    member val TimeSeries = TimeSeries.TimeSeriesClientExtension context with get
    /// Client DataPoints extension methods
    member val DataPoints = TimeSeries.DataPointsClientExtension context with get