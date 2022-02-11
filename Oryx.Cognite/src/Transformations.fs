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

    let listSchedules (query: TransformationScheduleQuery) (source: HttpHandler<unit>) : HttpHandler<ItemsWithCursor<TransformationSchedule>> =
        source
        |> withLogMessage "Transformations:listSchedules"
        |> getWithQuery query (Url +/ "schedules")

    let schedule (items: TransformationScheduleCreate seq) (source: HttpHandler<unit>) : HttpHandler<TransformationSchedule seq> =
        source
        |> withLogMessage "Transformations:schedule"
        |> HttpHandler.create items (Url +/ "schedules")

    let unschedule (items: TransformationScheduleDelete) (source: HttpHandler<unit>) : HttpHandler<EmptyResponse> =
        source
        |> withLogMessage "Transformations:unschedule"
        |> HttpHandler.delete items (Url +/ "schedules")

    let updateSchedules (items: UpdateItem<TransformationSchedule> seq) (source: HttpHandler<unit>) : HttpHandler<TransformationSchedule seq> =
        source
        |> withLogMessage "Transformations:updateSchedules"
        |> HttpHandler.update items (Url +/ "schedules")

    let listNotifications (query: TransformationNotificationQuery) (source: HttpHandler<unit>) : HttpHandler<TransformationNotification seq> =
        source
        |> withLogMessage "Transformations:listNotifications"
        |> getWithQuery query (Url +/ "notifications")

    let subscribe (items: TransformationNotificationCreate seq) (source: HttpHandler<unit>) : HttpHandler<TransformationNotification seq> =
        source
        |> withLogMessage "Transformations:subscribe"
        |> HttpHandler.create items (Url +/ "notifications")

    let deleteNotifications (items: int64 seq) (source: HttpHandler<unit>) : HttpHandler<EmptyResponse> =
        let idts = items |> Seq.map Identity.Create
        source
        |> withLogMessage "Transformations:deleteNotifications"
        |> HttpHandler.delete idts (Url +/ "notifications")

    let preview (query: TransformationPreview) (source: HttpHandler<unit>) : HttpHandler<TransformationPreviewResult> =
        source
        |> withLogMessage "Transformations:preview"
        |> withCompletion HttpCompletionOption.ResponseHeadersRead
        |> postV10 query (Url +/ "query/run")

    let getSchema (schemaType: TransformationDestinationType) (conflictMode: TransformationConflictMode) (source: HttpHandler<unit>) : HttpHandler<TransformationPreviewResult> =
        let query = TransformationSchemaQuery (ConflictMode = conflictMode)
        let url = (Url +/ "schema" +/ schemaType.ToString())
        source
        |> withLogMessage "Transformations:schema"
        |> getWithQuery query url
        
