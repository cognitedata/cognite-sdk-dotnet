// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite.Alpha

open Oryx
open Oryx.Cognite
open Oryx.Cognite.Alpha

open CogniteSdk.Alpha
open System.IO.Compression

[<RequireQualifiedAccess>]
module Simulators =
    open CogniteSdk

    [<Literal>]

    let Url = "/simulators"

    let runUrl = Url +/ "run"
    let runsUrl = runUrl + "s"
    let logsUrl = Url +/ "logs"
    let integrationsUrl = Url +/ "integrations"
    let modelsUrl = Url +/ "models"
    let modelRevisionsUrl = modelsUrl +/ "revisions"
    let routinesUrl = Url +/ "routines"
    let routineRevisionsUrl = routinesUrl +/ "revisions"

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
        : HttpHandler<ItemsWithCursor<SimulationRun>> =
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

    let listSimulationRunsData
        (ids: SimulationRunId seq)
        (source: HttpHandler<unit>)
        : HttpHandler<ItemsWithoutCursor<SimulationRunData>> =
        let dataUrl = runsUrl +/ "data/list"
        let request = ItemsWithoutCursor<SimulationRunId>(Items = ids)

        source
        |> withLogMessage "simulators:listSimulationRunsData"
        |> withAlphaHeader
        |> postV10<ItemsWithoutCursor<SimulationRunId>, ItemsWithoutCursor<SimulationRunData>> request dataUrl

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
        : HttpHandler<ItemsWithCursor<SimulatorModel>> =
        source
        |> withLogMessage "simulators:listSimulatorModels"
        |> withAlphaHeader
        |> HttpHandler.list query modelsUrl

    let retrieveSimulatorModels (ids: Identity seq) (source: HttpHandler<unit>) : HttpHandler<#SimulatorModel seq> =
        source
        |> withLogMessage "simulators:retrieveSimulatorModels"
        |> withAlphaHeader
        |> retrieve ids modelsUrl

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
        : HttpHandler<ItemsWithCursor<SimulatorModelRevision>> =
        source
        |> withLogMessage "simulators:listSimulatorModelRevisions"
        |> withAlphaHeader
        |> HttpHandler.list query modelRevisionsUrl

    let retrieveSimulatorModelRevisions
        (ids: Identity seq)
        (source: HttpHandler<unit>)
        : HttpHandler<#SimulatorModelRevision seq> =
        source
        |> withLogMessage "simulators:retrieveSimulatorModelRevisions"
        |> withAlphaHeader
        |> retrieve ids modelRevisionsUrl

    let updateSimulatorModelRevisions
        (items: UpdateItem<SimulatorModelRevisionUpdate> seq)
        (source: HttpHandler<unit>)
        : HttpHandler<SimulatorModelRevision seq> =
        source
        |> withLogMessage "simulators:updateSimulatorModelRevisions"
        |> withAlphaHeader
        |> HttpHandler.update items modelRevisionsUrl

    let updateSimulatorModelRevisionData
        (items: SimulatorModelRevisionDataUpdateItem seq)
        (source: HttpHandler<unit>)
        : HttpHandler<ItemsWithoutCursor<SimulatorModelRevisionData>> =

        let updateUrl = modelRevisionsUrl +/ "data/update"

        let request =
            ItemsWithoutCursor<SimulatorModelRevisionDataUpdateItem>(Items = items)

        source
        |> withLogMessage "simulators:updateSimulatorModelRevisionData"
        |> withAlphaHeader
        |> postV10<
            ItemsWithoutCursor<SimulatorModelRevisionDataUpdateItem>,
            ItemsWithoutCursor<SimulatorModelRevisionData>
            >
            request
            updateUrl



    let retrieveSimulatorModelRevisionData
        (request: ItemsWithoutCursor<SimulatorModelRevisionDataRetrieve>)
        (source: HttpHandler<unit>)
        : HttpHandler<ItemsWithoutCursor<SimulatorModelRevisionData>> =
        source
        |> withLogMessage "simulators:retrieveSimulatorModelRevisionData"
        |> withAlphaHeader
        |> HttpHandler.list request (modelRevisionsUrl +/ "data")

    let listSimulatorRoutines
        (query: SimulatorRoutineQuery)
        (source: HttpHandler<unit>)
        : HttpHandler<ItemsWithCursor<SimulatorRoutine>> =
        source
        |> withLogMessage "simulators:listSimulatorRoutines"
        |> withAlphaHeader
        |> HttpHandler.list query routinesUrl

    let deleteSimulatorRoutines
        (items: SimulatorRoutineDelete)
        (source: HttpHandler<unit>)
        : HttpHandler<EmptyResponse> =
        source
        |> withLogMessage "simulators:deleteSimulatorRoutines"
        |> withAlphaHeader
        |> HttpHandler.delete items routinesUrl

    let createSimulatorRoutines
        (items: SimulatorRoutineCreateCommandItem seq)
        (source: HttpHandler<unit>)
        : HttpHandler<SimulatorRoutine seq> =
        source
        |> withLogMessage "simulators:createSimulatorRoutines"
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
        : HttpHandler<ItemsWithCursor<SimulatorRoutineRevision>> =
        source
        |> withLogMessage "simulators:listSimulatorRoutineRevisions"
        |> withAlphaHeader
        |> HttpHandler.list query routineRevisionsUrl

    let retrieveSimulatorRoutineRevisions
        (ids: Identity seq)
        (source: HttpHandler<unit>)
        : HttpHandler<#SimulatorRoutineRevision seq> =
        source
        |> withLogMessage "simulators:retrieveSimulatorRoutineRevisions"
        |> withAlphaHeader
        |> retrieve ids routineRevisionsUrl

    let updateSimulatorLogs
        (items: UpdateItem<SimulatorLogUpdate> seq)
        (source: HttpHandler<unit>)
        : HttpHandler<EmptyResponse> =
        let content = ItemsWithoutCursor<_>(Items = items)

        source
        |> withLogMessage "simulators:updateLogs"
        |> withAlphaHeader
        |> HttpHandler.postV10<_, EmptyResponse> content (logsUrl +/ "/update")

    let updateSimulatorLogsWithGzip
        (items: UpdateItem<SimulatorLogUpdate> seq)
        (compression: CompressionLevel)
        (source: HttpHandler<unit>)
        : HttpHandler<EmptyResponse> =
        let content = ItemsWithoutCursor<_>(Items = items)

        createGzipJson<ItemsWithoutCursor<UpdateItem<SimulatorLogUpdate>>, EmptyResponse> 
            content 
            compression 
            (logsUrl +/ "/update")
            (source
             |> withLogMessage "simulators:updateLogsGzip" 
             |> withAlphaHeader)

    let retrieveSimulatorLogs (ids: Identity seq) (source: HttpHandler<unit>) : HttpHandler<#SimulatorLog seq> =

        source
        |> withLogMessage "simulators:retrieveLogs"
        |> withAlphaHeader
        |> retrieve ids logsUrl

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
