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

    /// Retrieve a list of transformations, optionally ignoring unknown ids.
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
        
    /// List transformations with pagination and an optional filter.
    let filter (query: TransformationFilterQuery) (source: HttpHandler<unit>) : HttpHandler<ItemsWithCursor<Transformation>> =
        let url = Url +/ "filter"
        source
        |> withLogMessage "Transformations:filter"
        |> withCompletion HttpCompletionOption.ResponseHeadersRead
        |> postV10 query url

    /// Delete a list of transformations, optionally ignoring unknown ids.
    let delete (items: TransformationDelete) (source: HttpHandler<unit>) : HttpHandler<EmptyResponse> =
        source
        |> withLogMessage "Transformations:delete"
        |> delete items Url

    /// List transformations with ability to not include job details.
    let list (query: TransformationQuery) (source: HttpHandler<unit>) : HttpHandler<ItemsWithCursor<Transformation>> =
        source
        |> withLogMessage "Transformations:list"
        |> getWithQuery query Url

    /// Create a list of transformations.
    let create (items: TransformationCreate seq) (source: HttpHandler<unit>) : HttpHandler<Transformation seq> =
        source
        |> withLogMessage "Transformations:create"
        |> create items Url
        
    /// Update a list of transformations.
    let update (items: UpdateItem<TransformationUpdate> seq) (source: HttpHandler<unit>) : HttpHandler<Transformation seq> =
        source
        |> withLogMessage "Transformations:update"
        |> update items Url

    /// Create a job for the specified transformation.
    let run (item: Identity) (source: HttpHandler<unit>) : HttpHandler<TransformationJob> =
        source
        |> withLogMessage "Transformations:run"
        |> withCompletion HttpCompletionOption.ResponseHeadersRead
        |> postV10 item (Url +/ "run")

    /// Retrieve a list of jobs with cursor for pagination.
    let listJobs (query: TransformationJobQuery) (source: HttpHandler<unit>) : HttpHandler<ItemsWithCursor<TransformationJob>> =
        source
        |> withLogMessage "Transformations:listJobs"
        |> getWithQuery query (Url +/ "jobs")

    /// Retrieve jobs by job internal id.
    let retrieveJobs (items: int64 seq) (ignoreUnknownIds: Nullable<bool>) (source: HttpHandler<unit>) : HttpHandler<TransformationJob seq> =
        let idts = items |> Seq.map Identity.Create
        source
        |> withLogMessage "Transformations:retrieveJobs"
        |> retrieveIgnoreUnknownIds idts (Option.ofNullable ignoreUnknownIds) (Url +/ "jobs")

    /// List metrics for the given job, like number of rows read or items created.
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

    /// Retrieve schedules for the given list of transformation ids.
    let retrieveSchedules (items: Identity seq) (ignoreUnknownIds: Nullable<bool>) (source: HttpHandler<unit>) : HttpHandler<TransformationSchedule seq> =
        source
        |> withLogMessage "Transformations:retrieveSchedules"
        |> retrieveIgnoreUnknownIds items (Option.ofNullable ignoreUnknownIds) (Url +/ "schedules")

    /// List schedules for all transformations, with pagination.
    let listSchedules (query: TransformationScheduleQuery) (source: HttpHandler<unit>) : HttpHandler<ItemsWithCursor<TransformationSchedule>> =
        source
        |> withLogMessage "Transformations:listSchedules"
        |> getWithQuery query (Url +/ "schedules")

    /// Create schedules for a list of transformations.
    let schedule (items: TransformationScheduleCreate seq) (source: HttpHandler<unit>) : HttpHandler<TransformationSchedule seq> =
        source
        |> withLogMessage "Transformations:schedule"
        |> HttpHandler.create items (Url +/ "schedules")

    /// Delete schedules for a list of transformations.
    let unschedule (items: TransformationScheduleDelete) (source: HttpHandler<unit>) : HttpHandler<EmptyResponse> =
        source
        |> withLogMessage "Transformations:unschedule"
        |> HttpHandler.delete items (Url +/ "schedules")

    /// Update schedules for a list of transformations.
    let updateSchedules (items: UpdateItem<TransformationScheduleUpdate> seq) (source: HttpHandler<unit>) : HttpHandler<TransformationSchedule seq> =
        source
        |> withLogMessage "Transformations:updateSchedules"
        |> HttpHandler.update items (Url +/ "schedules")

    /// List notifications, optionally restricted to a single transformation, with pagination.
    let listNotifications (query: TransformationNotificationQuery) (source: HttpHandler<unit>) : HttpHandler<ItemsWithCursor<TransformationNotification>> =
        source
        |> withLogMessage "Transformations:listNotifications"
        |> getWithQuery query (Url +/ "notifications")

    /// Create notifications for transformations.
    let subscribe (items: TransformationNotificationCreate seq) (source: HttpHandler<unit>) : HttpHandler<TransformationNotification seq> =
        source
        |> withLogMessage "Transformations:subscribe"
        |> HttpHandler.create items (Url +/ "notifications")

    /// Delete notifications for transformations.
    let deleteNotifications (items: int64 seq) (source: HttpHandler<unit>) : HttpHandler<EmptyResponse> =
        let idts = items |> Seq.map Identity.Create
        let req = ItemsWithoutCursor(Items = idts)
        source
        |> withLogMessage "Transformations:deleteNotifications"
        |> HttpHandler.delete req (Url +/ "notifications")

    /// Run a transformation query to preview the result.
    let preview (query: TransformationPreview) (source: HttpHandler<unit>) : HttpHandler<TransformationPreviewResult> =
        source
        |> withLogMessage "Transformations:preview"
        |> withCompletion HttpCompletionOption.ResponseHeadersRead
        |> postV10 query (Url +/ "query/run")

    /// Get the schema for a given transformation destination type and conflict mode.
    let getSchema (schemaType: TransformationDestinationType) (conflictMode: TransformationConflictMode) (source: HttpHandler<unit>) : HttpHandler<IEnumerable<TransformationColumnType>> =
        let query = TransformationSchemaQuery (ConflictMode = conflictMode)
        let url = (Url +/ "schema" +/ schemaType.ToString())
        http {
            let! result = 
                source
                |> withLogMessage "Transformations:schema"
                |> getWithQuery<ItemsWithoutCursor<TransformationColumnType>> query url
            result.Items
        }
        
        
