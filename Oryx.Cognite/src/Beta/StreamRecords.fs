// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite.Beta

open Oryx
open Oryx.Cognite
open Oryx.Cognite.Beta

open CogniteSdk.Beta

[<RequireQualifiedAccess>]
module StreamRecords =
    open Oryx.SystemTextJson

    [<Literal>]
    let Url = "/streams"

    let ingest
        (stream: string)
        (items: StreamRecordIngest)
        (source: HttpHandler<unit>)
        : HttpHandler<CogniteSdk.EmptyResponse> =
        source
        |> withLogMessage "streamrecords:ingest"
        |> withAlphaHeader
        |> postV10 items (Url +/ stream +/ "records")

    let upsert
        (stream: string)
        (items: StreamRecordIngest)
        (source: HttpHandler<unit>)
        : HttpHandler<CogniteSdk.EmptyResponse> =
        source
        |> withLogMessage "streamrecords:upsert"
        |> withAlphaHeader
        |> postV10 items (Url +/ stream +/ "records/upsert")

    let delete (stream: string) (items: StreamRecordDelete) (source: HttpHandler<unit>) : HttpHandler<unit> =
        source
        |> withLogMessage "streamrecords:delete"
        |> withAlphaHeader
        |> POST
        |> withVersion V10
        |> withResource (Url +/ stream +/ "records/delete")
        |> withContent (fun () -> new JsonPushStreamContent<StreamRecordDelete>(items, jsonOptions))
        |> fetch
        |> withError decodeError
        |> ignoreResponse
        |> log

    let retrieve<'T>
        (stream: string)
        (request: StreamRecordsRetrieve)
        (source: HttpHandler<unit>)
        : HttpHandler<StreamRecord<'T> seq> =
        http {
            let! ret =
                source
                |> withLogMessage "streamrecords:retrieve"
                |> withAlphaHeader
                |> withCompletion System.Net.Http.HttpCompletionOption.ResponseHeadersRead
                |> postV10<_, CogniteSdk.ItemsWithoutCursor<_>> request (Url +/ stream +/ "records/filter")

            return ret.Items
        }

    let sync<'T>
        (stream: string)
        (request: StreamRecordsSync)
        (source: HttpHandler<unit>)
        : HttpHandler<StreamRecordsSyncResponse<'T>> =
        source
        |> withLogMessage "streamrecords:sync"
        |> withAlphaHeader
        |> withCompletion System.Net.Http.HttpCompletionOption.ResponseHeadersRead
        |> postV10 request (Url +/ stream +/ "records/sync")

    let createStream (stream: StreamWrite) (source: HttpHandler<unit>) : HttpHandler<Stream> =
        http {
            let request = CogniteSdk.ItemsWithoutCursor(Items = [ stream ])

            let! ret =
                source
                |> withLogMessage "streamrecords:createstream"
                |> withAlphaHeader
                |> postV10<_, CogniteSdk.ItemsWithoutCursor<_>> request Url

            return Seq.exactlyOne (ret.Items)
        }

    let deleteStream (stream: string) (source: HttpHandler<unit>) : HttpHandler<CogniteSdk.EmptyResponse> =
        source
        |> withLogMessage "streamrecords:deletestream"
        |> withAlphaHeader
        |> deleteV10 (Url +/ stream)

    let listStreams (source: HttpHandler<unit>) : HttpHandler<Stream seq> =
        http {
            let! ret =
                source
                |> withLogMessage "streamrecords:liststreams"
                |> withAlphaHeader
                |> withCompletion System.Net.Http.HttpCompletionOption.ResponseHeadersRead
                |> getV10<CogniteSdk.ItemsWithoutCursor<_>> Url

            return ret.Items
        }

    let retrieveStream
        (stream: string)
        (includeStatistics: bool option)
        (source: HttpHandler<unit>)
        : HttpHandler<Stream> =
        let url =
            match includeStatistics with
            | Some true -> (Url +/ stream) + "?includeStatistics=true"
            | Some false -> (Url +/ stream) + "?includeStatistics=false"
            | None -> (Url +/ stream)

        source
        |> withLogMessage "streamrecords:retrievestream"
        |> withAlphaHeader
        |> getV10 url
