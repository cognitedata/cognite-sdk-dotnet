// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace CogniteSdk

open System.Net.Http
open Oryx

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
        Context.create ()
        |> Client

    /// Client Assets extension methods
    member val Assets = Assets.ClientExtension context with get
    /// Client TimeSeries extension methods
    member val TimeSeries = TimeSeries.ClientExtension context with get
    /// Client DataPoints extension methods
    member val DataPoints = DataPoints.ClientExtension context with get
    /// Client Events extension methods
    member val Events = Events.ClientExtension context with get
