// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite.Alpha

open System.Collections.Generic

open Oryx
open Oryx.Cognite
open Oryx.Cognite.Alpha

open CogniteSdk.Alpha

[<RequireQualifiedAccess>]
module LogAnalytics =
    [<Literal>]
    let Url = "/logs"

    let ingest (items: LogIngest) (source: HttpHandler<unit>) : HttpHandler<CogniteSdk.EmptyResponse> =
        source
        |> withLogMessage "loganalytics:ingest"
        |> withAlphaHeader
        |> postV10<LogIngest, CogniteSdk.EmptyResponse> items Url

    let retrieve<'T> (request: LogRetrieve) (source: HttpHandler<unit>) : HttpHandler<Log<'T> seq> =
        http {
            let! ret =
                source
                |> withLogMessage "loganalytics:retrieve"
                |> withCompletion System.Net.Http.HttpCompletionOption.ResponseHeadersRead
                |> postV10<_, CogniteSdk.ItemsWithoutCursor<_>> request (Url +/ "list")

            return ret.Items
        }

    let sync<'T> (request: LogSync) (source: HttpHandler<unit>) : HttpHandler<LogSyncResponse<'T>> =
        source
        |> withLogMessage "loganalytics:sync"
        |> withCompletion System.Net.Http.HttpCompletionOption.ResponseHeadersRead
        |> postV10 request (Url +/ "sync")
