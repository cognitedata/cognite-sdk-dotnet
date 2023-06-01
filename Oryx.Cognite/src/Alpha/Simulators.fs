// Copyright 2023 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite.Alpha

open Oryx
open Oryx.Cognite
open Oryx.Cognite.Alpha

open CogniteSdk.Alpha

[<RequireQualifiedAccess>]
module Simulators =
    open CogniteSdk

    [<Literal>]
    let Url = "/simulators"

    let runsUrl = Url +/ "runs"
    let runCallbackUrl = Url +/ "run/callback"
    let createRunsUrl = Url +/ "run"

    let createSimulationRuns
        (items: SimulationRunCreate seq)
        (source: HttpHandler<unit>)
        : HttpHandler<SimulationRun seq> =
        source
        |> withLogMessage "simulators:createSimulationRuns"
        |> withAlphaHeader
        |> HttpHandler.create items createRunsUrl


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

        let request = ItemsWithoutCursor<SimulationRunCallbackItem>(Items = [ query ])

        source
        |> withLogMessage "simulators:simulationRunCallback"
        |> withAlphaHeader
        |> postV10<ItemsWithoutCursor<SimulationRunCallbackItem>, ItemsWithoutCursor<SimulationRun>>
            request
            runCallbackUrl
