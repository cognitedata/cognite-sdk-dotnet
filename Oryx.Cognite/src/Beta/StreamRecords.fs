// Copyright 2024 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite.Beta

open Oryx
open Oryx.Cognite
open Oryx.Cognite.Beta

open CogniteSdk.Beta

[<RequireQualifiedAccess>]
module StreamRecords =
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

    let retrieveStream (stream: string) (source: HttpHandler<unit>) : HttpHandler<Stream> =
        source
        |> withLogMessage "streamrecords:retrievestream"
        |> withAlphaHeader
        |> getV10 (Url +/ stream)
