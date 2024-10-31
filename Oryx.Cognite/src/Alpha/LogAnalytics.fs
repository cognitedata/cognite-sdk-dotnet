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
                |> withAlphaHeader
                |> withCompletion System.Net.Http.HttpCompletionOption.ResponseHeadersRead
                |> postV10<_, CogniteSdk.ItemsWithoutCursor<_>> request (Url +/ stream +/ "records/filter")

            return ret.Items
        }

    let sync<'T> (stream: string) (request: LogSync) (source: HttpHandler<unit>) : HttpHandler<LogSyncResponse<'T>> =
        source
        |> withLogMessage "loganalytics:sync"
        |> withAlphaHeader
        |> withCompletion System.Net.Http.HttpCompletionOption.ResponseHeadersRead
        |> postV10 request (Url +/ stream +/ "records/sync")

    let createStream (stream: StreamWrite) (source: HttpHandler<unit>) : HttpHandler<Stream> =
        http {
            let request = CogniteSdk.ItemsWithoutCursor(Items = [ stream ])
            let! ret =
                source
                |> withLogMessage "loganalytics:createstream"
                |> withAlphaHeader
                |> postV10<_, CogniteSdk.ItemsWithoutCursor<_>> request Url
            
            return Seq.exactlyOne(ret.Items)
        }
    
    let deleteStream (stream: string) (source: HttpHandler<unit>) : HttpHandler<CogniteSdk.EmptyResponse> =
        source
        |> withLogMessage "loganalytics:deletestream"
        |> withAlphaHeader
        |> deleteV10 (Url +/ stream)
    
    let listStreams (source: HttpHandler<unit>) : HttpHandler<Stream seq> =
        http {
            let! ret =
                source
                |> withLogMessage "loganalytics:liststreams"
                |> withAlphaHeader
                |> withCompletion System.Net.Http.HttpCompletionOption.ResponseHeadersRead
                |> getV10<CogniteSdk.ItemsWithoutCursor<_>> Url
            
            return ret.Items
        }
    
    let retrieveStream (stream: string) (source: HttpHandler<unit>) : HttpHandler<Stream> =
        source
        |> withLogMessage "loganalyitics:retrievestream"
        |> withAlphaHeader
        |> getV10 (Url +/ stream)