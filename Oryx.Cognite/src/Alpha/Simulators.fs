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
        (query: SimulationFilterIntegrationsQuery) // TODO: check if in namespace
        (source: HttpHandler<unit>)
        : HttpHandler<ItemsWithoutCursor<SimulatorIntegrations>> =
        let listUrl = integrationsUrl +/ "list"
        source
        |> withLogMessage "simulators:listSimulatorIntegrations"
        |> withAlphaHeader
        |> HttpHandler.list query listUrl

    let simulatorIntegrationUpdate
        (items: UpdateItem<SimulatorIntegrationUpdate> seq)
        (source: HttpHandler<unit>)
        : HttpHandler<SimulatorIntegrations seq> =
        source
        |> withLogMessage "simulators:simulatorIntegrationUpdate"
        |> withAlphaHeader
        |> HttpHandler.update items integrationsUrl

    let createSimulatorIntegrations
        (items: CreateSimulatorIntegrations seq)
        (source: HttpHandler<unit>)
        : HttpHandler<SimulatorIntegrations seq> =
        source
        |> withLogMessage "simulators:createSimulationRuns"
        |> withAlphaHeader
        |> HttpHandler.create items integrationsUrl

    let listSimulators
        (query: FilterSimulatorsQuery)
        (source: HttpHandler<unit>)
        : HttpHandler<ItemsWithoutCursor<SimulatorsRun>> =
        source
        |> withLogMessage "simulators:list"
        |> withAlphaHeader
        |> HttpHandler.list query Url

    let createSimulators (items: CreateSimulators seq) (source: HttpHandler<unit>) : HttpHandler<Simulator seq> =
        source
        |> withLogMessage "simulators:create"
        |> withAlphaHeader
        |> HttpHandler.create items Url

    let deleteSimulators (items: DeleteSimulators) (source: HttpHandler<unit>) : HttpHandler<EmptyResponse> = // TODO:Update this to return unit
        source
        |> withLogMessage "simulators:deleteSimulators"
        |> withAlphaHeader
        |> HttpHandler.delete items simulatorsDeleteUrl

    let updateSimulators (items: UpdateSimulators seq) (source: HttpHandler<unit>) : HttpHandler<Simulator seq> =
        source
        |> withLogMessage "simulators:updateSimulators"
        |> withAlphaHeader
        |> HttpHandler.create items simulatorsUpdateUrl
