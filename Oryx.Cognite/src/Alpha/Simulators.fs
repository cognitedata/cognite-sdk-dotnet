// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite.Alpha

open System.Collections.Generic
open Oryx
open Oryx.Cognite
open Oryx.Cognite.Alpha

open CogniteSdk.Alpha

open System
open Oryx.SystemTextJson
open Oryx.SystemTextJson.ResponseReader

[<RequireQualifiedAccess>]
module Simulators =
    open CogniteSdk

    [<Literal>]

    let Url = "/simulators"

    let runUrl = Url +/ "run"
    let runsUrl = runUrl + "s"
    let integrationsUrl = Url +/ "integrations"
    let modelsUrl = Url +/ "models"
    let modelRevisionsUrl = modelsUrl +/ "revisions"
    let routinesUrl = Url +/ "routines"

    let routineRevisionsUrl = routinesUrl +/ "revisions"

    let routineRevisionsListUrl = routineRevisionsUrl +/ "list"

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

    let createSimulatorModels
        (items: SimulatorModelCreate seq)
        (source: HttpHandler<unit>)
        : HttpHandler<SimulatorModel seq> =
        source
        |> withLogMessage "simulators:createSimulatorModel"
        |> withAlphaHeader
        |> HttpHandler.create items modelsUrl

    let listSimulatorModels
        (query: SimulatorModelQuery)
        (source: HttpHandler<unit>)
        : HttpHandler<ItemsWithoutCursor<SimulatorModel>> =
        source
        |> withLogMessage "simulators:listSimulatorModels"
        |> withAlphaHeader
        |> HttpHandler.list query modelsUrl

    let updateSimulatorModels
        (items: UpdateItem<SimulatorModelUpdate> seq)
        (source: HttpHandler<unit>)
        : HttpHandler<SimulatorModel seq> =
        source
        |> withLogMessage "simulators:updateSimulatorModels"
        |> withAlphaHeader
        |> HttpHandler.update items modelsUrl

    let deleteSimulatorModels (items: SimulatorModelDelete) (source: HttpHandler<unit>) : HttpHandler<EmptyResponse> =
        source
        |> withLogMessage "simulators:deleteSimulatorModels"
        |> withAlphaHeader
        |> HttpHandler.delete items modelsUrl

    let createSimulatorModelRevisions
        (items: SimulatorModelRevisionCreate seq)
        (source: HttpHandler<unit>)
        : HttpHandler<SimulatorModelRevision seq> =
        source
        |> withLogMessage "simulators:createSimulatorModelRevision"
        |> withAlphaHeader
        |> HttpHandler.create items modelRevisionsUrl

    let listSimulatorModelRevisions
        (query: SimulatorModelRevisionQuery)
        (source: HttpHandler<unit>)
        : HttpHandler<ItemsWithoutCursor<SimulatorModelRevision>> =
        source
        |> withLogMessage "simulators:listSimulatorModelRevisions"
        |> withAlphaHeader
        |> HttpHandler.list query modelRevisionsUrl

    let updateSimulatorModelRevisions
        (items: UpdateItem<SimulatorModelRevisionUpdate> seq)
        (source: HttpHandler<unit>)
        : HttpHandler<SimulatorModelRevision seq> =
        source
        |> withLogMessage "simulators:updateSimulatorModelRevisions"
        |> withAlphaHeader
        |> HttpHandler.update items modelRevisionsUrl


    let createSimulatorRoutines
        (items: SimulatorRoutineCreateCommandItem seq)
        (source: HttpHandler<unit>)
        : HttpHandler<SimulatorRoutine seq> =
        source
        |> withLogMessage "simulators:createSimulatorRoutines"
        |> withAlphaHeader
        |> HttpHandler.create items routinesUrl

    let createSimulatorRoutinesPredefined
        (items: SimulatorRoutineCreateCommandPredefined seq)
        (source: HttpHandler<unit>)
        : HttpHandler<SimulatorRoutine seq> =
        source
        |> withLogMessage "simulators:createSimulatorRoutinesPredefined"
        |> withAlphaHeader
        |> HttpHandler.create items routinesUrl

    let createSimulatorRoutineRevisions
        (items: SimulatorRoutineRevisionCreate seq)
        (source: HttpHandler<unit>)
        : HttpHandler<SimulatorRoutineRevision seq> =
        source
        |> withLogMessage "simulators:createSimulatorRoutineRevision"
        |> withAlphaHeader
        |> HttpHandler.create items routineRevisionsUrl

    let listSimulatorRoutineRevisions
        (query: SimulatorRoutineRevisionQuery)
        (source: HttpHandler<unit>)
        : HttpHandler<ItemsWithoutCursor<SimulatorRoutineRevision>> =
        source
        |> withLogMessage "simulators:listSimulatorRoutineRevisions"
        |> withAlphaHeader
        |> HttpHandler.list query routineRevisionsListUrl

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

    let update (items: UpdateItem<SimulatorUpdate> seq) (source: HttpHandler<unit>) : HttpHandler<Simulator seq> =
        source
        |> withLogMessage "simulators:update"
        |> withAlphaHeader
        |> HttpHandler.update items Url
