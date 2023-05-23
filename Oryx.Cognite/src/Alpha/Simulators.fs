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
