// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite.Alpha

open Oryx
open Oryx.Cognite
open Oryx.Cognite.Alpha

open CogniteSdk.Alpha

[<RequireQualifiedAccess>]
module LogAnalytics =
    [<Literal>]
    let Url = "/streams"

    let ingest (stream: string) (items: LogIngest) (source: HttpHandler<unit>) : HttpHandler<CogniteSdk.EmptyResponse> =
        source
        |> withLogMessage "loganalytics:ingest"
        |> withAlphaHeader
        |> postV10 items (Url +/ stream +/ "records")

    let retrieve<'T> (stream: string) (request: LogRetrieve) (source: HttpHandler<unit>) : HttpHandler<Log<'T> seq> =
        http {
            let! ret =
                source
                |> withLogMessage "loganalytics:retrieve"
                |> withCompletion System.Net.Http.HttpCompletionOption.ResponseHeadersRead
                |> postV10<_, CogniteSdk.ItemsWithoutCursor<_>> request (Url +/ stream +/ "records/filter")

            return ret.Items
        }

    let sync<'T> (stream: string) (request: LogSync) (source: HttpHandler<unit>) : HttpHandler<LogSyncResponse<'T>> =
        source
        |> withLogMessage "loganalytics:sync"
        |> withCompletion System.Net.Http.HttpCompletionOption.ResponseHeadersRead
        |> postV10 request (Url +/ stream +/ "records/sync")
