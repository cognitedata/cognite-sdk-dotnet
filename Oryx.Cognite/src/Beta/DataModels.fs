// Copyright 2026 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite.Beta

open System.Net.Http

open Oryx
open Oryx.Cognite
open Oryx.Cognite.Beta

open CogniteSdk
open CogniteSdk.DataModels

/// Data modeling handlers that opt in to the beta API
[<RequireQualifiedAccess>]
module DataModels =
    let private instancesUrl = "/models/instances"

    /// Create or update a list of instances using the beta API
    let upsertInstances (request: InstanceWriteRequest) (source: HttpHandler<unit>) : HttpHandler<SlimInstance seq> =
        http {
            let! ret =
                source
                |> withLogMessage "Beta:models:instances:create"
                |> withBetaHeader
                |> withCompletion HttpCompletionOption.ResponseHeadersRead
                |> postV10<_, ItemsWithoutCursor<_>> request instancesUrl

            return ret.Items
        }
