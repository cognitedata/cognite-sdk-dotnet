// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite.Alpha

open System.Collections.Generic
open Oryx
open Oryx.Cognite
open Oryx.Cognite.Alpha

open CogniteSdk.Alpha

[<RequireQualifiedAccess>]
module Simulators =
    open CogniteSdk

    [<Literal>]

    let Url = "/simulators"

    let runUrl = Url +/ "run"
    let runsUrl = runUrl + "s"
    let integrationsUrl = Url +/ "integrations"

    let createSimulationRuns
        (items: SimulationRunCreate seq)
        (source: HttpHandler<unit>)
        : HttpHandler<SimulationRun seq> =
        source
        |> withLogMessage "simulators:createSimulationRuns"
        |> withAlphaHeader
        |> HttpHandler.create items runUrl


    /// List all runs
    let listSimulationRuns
        (query: SimulationRunQuery)
        (source: HttpHandler<unit>)
        : HttpHandler<ItemsWithoutCursor<SimulationRun>> =
        source
        |> withLogMessage "simulators:listSimulationRuns"
        |> withAlphaHeader
        |> HttpHandler.list query runsUrl

    let simulationRunCallback
        (query: SimulationRunCallbackItem)
        (source: HttpHandler<unit>)
        : HttpHandler<ItemsWithoutCursor<SimulationRun>> =

        let runCallbackUrl = runUrl +/ "callback"
        let request = ItemsWithoutCursor<SimulationRunCallbackItem>(Items = [ query ])

        source
        |> withLogMessage "simulators:simulationRunCallback"
        |> withAlphaHeader
        |> postV10<ItemsWithoutCursor<SimulationRunCallbackItem>, ItemsWithoutCursor<SimulationRun>>
            request
            runCallbackUrl

    let retrieveSimulationRuns (ids: Identity seq) (source: HttpHandler<unit>) : HttpHandler<#SimulationRun seq> =
        source
        |> withLogMessage "simulators:retrieveSimulationRuns"
        |> withAlphaHeader
        |> retrieve ids runsUrl

    let listSimulatorIntegrations
        (query: SimulatorIntegrationQuery)
        (source: HttpHandler<unit>)
        : HttpHandler<ItemsWithoutCursor<SimulatorIntegration>> =
        source
        |> withLogMessage "simulators:listSimulatorIntegrations"
        |> withAlphaHeader
        |> HttpHandler.list query integrationsUrl

    let updateSimulatorIntegrations
        (items: UpdateItem<SimulatorIntegrationUpdate> seq)
        (source: HttpHandler<unit>)
        : HttpHandler<SimulatorIntegration seq> =
        source
        |> withLogMessage "simulators:updateSimulatorIntegrations"
        |> withAlphaHeader
        |> HttpHandler.update items integrationsUrl

    let createSimulatorIntegrations
        (items: SimulatorIntegrationCreate seq)
        (source: HttpHandler<unit>)
        : HttpHandler<SimulatorIntegration seq> =
        source
        |> withLogMessage "simulators:createSimulatorIntegrations"
        |> withAlphaHeader
        |> HttpHandler.create items integrationsUrl

    let list (query: SimulatorQuery) (source: HttpHandler<unit>) : HttpHandler<ItemsWithoutCursor<Simulator>> =
        source
        |> withLogMessage "simulators:list"
        |> withAlphaHeader
        |> HttpHandler.list query Url

    let create (items: SimulatorCreate seq) (source: HttpHandler<unit>) : HttpHandler<Simulator seq> =
        source
        |> withLogMessage "simulators:create"
        |> withAlphaHeader
        |> HttpHandler.create items Url

    let delete (items: SimulatorDelete) (source: HttpHandler<unit>) : HttpHandler<EmptyResponse> =
        source
        |> withLogMessage "simulators:delete"
        |> withAlphaHeader
        |> HttpHandler.delete items Url

    let update (items: SimulatorUpdateItem seq) (source: HttpHandler<unit>) : HttpHandler<Simulator seq> =
        let updateUrl = Url +/ "update"

        source
        |> withLogMessage "simulators:update"
        |> withAlphaHeader
        |> HttpHandler.create items updateUrl
