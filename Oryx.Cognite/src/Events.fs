// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System
open System.Net.Http

open Oryx
open Oryx.Cognite

open CogniteSdk

/// Various event HTTP handlers.

[<RequireQualifiedAccess>]
module Events =
    [<Literal>]
    let Url = "/events"

    /// <summary>
    /// Retrieves information about an event given an event id.
    /// </summary>
    /// <param name="eventId">The id of the event to get.</param>
    /// <returns>Event with the given id.</returns>
    let get (eventId: int64) : IHttpHandler<unit, #Event> =
        withLogMessage "Events:get"
        >=> getById eventId Url

    /// <summary>
    /// Retrieves list of events matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="query">The query to use.</param>
    /// <returns>List of events matching given filters and optional cursor</returns>
    let list (query: EventQuery) : IHttpHandler<unit, ItemsWithCursor<#Event>> =
        withLogMessage "Events:list"
        >=> list query Url

    /// <summary>
    /// Retrieves number of events matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="query">The query to use.</param>
    /// <returns>Number of events matching given filters and optional cursor</returns>
    let aggregate (query: EventQuery) : IHttpHandler<unit, int32> =
        withLogMessage "Events:aggregate"
        >=> aggregate query Url

    /// <summary>
    /// Create new events in the given project.
    /// </summary>
    /// <param name="items">The events to create.</param>
    /// <returns>List of created events.</returns>
    let create (items: EventCreate seq) : IHttpHandler<unit, #Event seq> =
        withLogMessage "Events:create"
        >=> create items Url

    /// <summary>
    /// Delete multiple events in the same project,
    /// </summary>
    /// <param name="items">The list of events to delete.</param>
    /// <returns>Empty result.</returns>
    let delete (items: EventDelete) : IHttpHandler<unit, EmptyResponse> =
        withLogMessage "Events:delete"
        >=> delete items Url

    /// <summary>
    /// Retrieves information about multiple events in the same project. A maximum of 1000 event IDs may be listed per
    /// request and all of them must be unique.
    /// </summary>
    /// <param name="ids">The ids of the events to get.</param>
    /// <param name="ignoreUnknownIds">Ignore IDs and external IDs that are not found. Default: false</param>
    /// <returns>Events with given ids.</returns>
    let retrieve (ids: Identity seq) (ignoreUnknownIds: Nullable<bool>) : IHttpHandler<unit, #Event seq> =
        withLogMessage "Events:retrieve"
        >=> retrieveIgnoreUnkownIds ids (Option.ofNullable ignoreUnknownIds) Url

    /// <summary>
    /// Retrieves a list of events matching the given criteria. This operation does not support pagination.
    /// </summary>
    /// <param name="query">Event search query.</param>
    /// <returns>List of events matching given criteria.</returns>
    let search (query: EventSearch) : IHttpHandler<unit, #Event seq> =
        withLogMessage "Events:search"
        >=> search query Url

    /// <summary>
    /// Update one or more events. Supports partial updates, meaning that fields omitted from the requests are not changed
    /// </summary>
    /// <param name="query">The list of events to update.</param>
    /// <returns>List of updated events.</returns>
    let update (query: UpdateItem<EventUpdate> seq) : IHttpHandler<unit, #Event seq>  =
        withLogMessage "Events:update"
        >=> update query Url

