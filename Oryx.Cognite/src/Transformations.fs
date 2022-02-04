// Copyright 2022 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System
open System.Collections.Generic
open System.Net.Http

open Oryx
open Oryx.Cognite

open CogniteSdk

/// Various transformation http handlers.
[<RequireQualifiedAccess>]
module Transformations =
    [<Literal>]
    let Url = "/transformations"

    let retrieve (query: TransformationRetrieve) (source: HttpHandler<unit>) : HttpHandler<Transformation seq> =
        http {
            let url = Url +/ "byids"
            let! ret = 
                source
                |> withLogMessage "Transformations:retrieve"
                |> withCompletion HttpCompletionOption.ResponseHeadersRead
                |> postV10<TransformationRetrieve, ItemsWithoutCursor<Transformation>> query url

            return ret.Items
        }
        

    let filter (query: TransformationFilterQuery) (source: HttpHandler<unit>) : HttpHandler<ItemsWithCursor<Transformation>> =
        let url = Url +/ "filter"
        source
        |> withLogMessage "Transformations:filter"
        |> withCompletion HttpCompletionOption.ResponseHeadersRead
        |> postV10 query url

    let delete (items: TransformationDelete) (source: HttpHandler<unit>) : HttpHandler<EmptyResponse> =
        source
        |> withLogMessage "Transformations:delete"
        |> delete items Url

    let list (query: TransformationQuery) (source: HttpHandler<unit>) : HttpHandler<ItemsWithCursor<Transformation>> =
        source
        |> withLogMessage "Transformations:list"
        |> getWithQuery query Url

    let create (items: TransformationCreate seq) (source: HttpHandler<unit>) : HttpHandler<Transformation seq> =
        source
        |> withLogMessage "Transformations:create"
        |> create items Url

    let update (items: UpdateItem<TransformationUpdate> seq) (source: HttpHandler<unit>) : HttpHandler<Transformation seq> =
        source
        |> withLogMessage "Transformations:update"
        |> update items Url

    let run (item: Identity) (source: HttpHandler<unit>) : HttpHandler<TransformationJob> =
        source
        |> withLogMessage "Transformations:run"
        |> withCompletion HttpCompletionOption.ResponseHeadersRead
        |> postV10 item (Url +/ "run")

    let listJobs (query: TransformationJobQuery) (source: HttpHandler<unit>) : HttpHandler<ItemsWithCursor<TransformationJob>> =
        source
        |> withLogMessage "Transformations:listJobs"
        |> getWithQuery query (Url +/ "jobs")

    let retrieveJobs (items: int64 seq) (ignoreUnknownIds: bool option) (source: HttpHandler<unit>) : HttpHandler<TransformationJob seq> =
        let idts = items |> Seq.map Identity.Create
        source
        |> withLogMessage "Transformations:retrieveJobs"
        |> retrieveIgnoreUnknownIds idts ignoreUnknownIds (Url +/ "jobs")

    let listJobMetrics (id: int64) (source: HttpHandler<unit>) : HttpHandler<TransformationJobMetric seq> =
        http {
            let url = Url +/ $"jobs/{id}/metrics"
            let! ret = 
                source
                |> withLogMessage "Transformations:listJobMetrics"
                |> withCompletion HttpCompletionOption.ResponseHeadersRead
                |> getV10<ItemsWithoutCursor<TransformationJobMetric>> url

            return ret.Items
        }

    let retrieveSchedules (items: Identity seq) (ignoreUnknownIds: bool option) (source: HttpHandler<unit>) : HttpHandler<TransformationSchedule seq> =
        source
        |> withLogMessage "Transformations:retrieveSchedules"
        |> retrieveIgnoreUnknownIds items ignoreUnknownIds (Url +/ "schedules")

