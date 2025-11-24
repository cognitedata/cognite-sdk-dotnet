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
        |> postV10 items (Url +/ stream +/ "records")

    let upsert
        (stream: string)
        (items: StreamRecordIngest)
        (source: HttpHandler<unit>)
        : HttpHandler<CogniteSdk.EmptyResponse> =
        source
        |> withLogMessage "streamrecords:upsert"
        |> postV10 items (Url +/ stream +/ "records/upsert")

    let delete (stream: string) (items: StreamRecordDelete) (source: HttpHandler<unit>) : HttpHandler<unit> =
        source
        |> withLogMessage "streamrecords:delete"
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
        |> withCompletion System.Net.Http.HttpCompletionOption.ResponseHeadersRead
        |> postV10 request (Url +/ stream +/ "records/sync")

    let aggregate
        (stream: string)
        (request: StreamRecordsAggregate)
        (source: HttpHandler<unit>)
        : HttpHandler<StreamRecordsAggregateResponse> =
        source
        |> withLogMessage "streamrecords:aggregate"
        |> withCompletion System.Net.Http.HttpCompletionOption.ResponseHeadersRead
        |> postV10 request (Url +/ stream +/ "records/aggregate")

    let createStream (stream: StreamWrite) (source: HttpHandler<unit>) : HttpHandler<Stream> =
        http {
            let request = CogniteSdk.ItemsWithoutCursor(Items = [ stream ])

            let! ret =
                source
                |> withLogMessage "streamrecords:createstream"
                |> postV10<_, CogniteSdk.ItemsWithoutCursor<_>> request Url

            return Seq.exactlyOne (ret.Items)
        }

    let deleteStream (stream: string) (source: HttpHandler<unit>) : HttpHandler<unit> =
        let request =
            CogniteSdk.ItemsWithoutCursor(Items = [ StreamDeleteItem(ExternalId = stream) ])

        source
        |> withLogMessage "streamrecords:deletestream"
        |> POST
        |> withVersion V10
        |> withResource (Url +/ "delete")
        |> withContent (fun () ->
            new JsonPushStreamContent<CogniteSdk.ItemsWithoutCursor<StreamDeleteItem>>(request, jsonOptions))
        |> fetch
        |> withError decodeError
        |> ignoreResponse
        |> log

    let listStreams (source: HttpHandler<unit>) : HttpHandler<Stream seq> =
        http {
            let! ret =
                source
                |> withLogMessage "streamrecords:liststreams"
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

        source |> withLogMessage "streamrecords:retrievestream" |> getV10 url
