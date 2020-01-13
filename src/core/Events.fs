// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System.Net.Http

open Oryx.Cognite

open CogniteSdk
open CogniteSdk.Events

/// Various event HTTP handlers.

[<RequireQualifiedAccess>]
module Events =
    [<Literal>]
    let Url = "/events"

    /// <summary>
    /// Retrieves information about an asset given an event id.
    /// </summary>
    /// <param name="assetId">The id of the asset to get.</param>
    /// <returns>Event with the given id.</returns>
    let get (eventId: int64) : HttpHandler<HttpResponseMessage, EventReadDto, 'a> =
        get eventId Url

    /// <summary>
    /// Retrieves list of events matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="query">The query to use.</param>
    /// <returns>List of events matching given filters and optional cursor</returns>
    let list (query: EventQuery) : HttpHandler<HttpResponseMessage, ItemsWithCursor<EventReadDto>, 'a> =
        list query Url

    /// <summary>
    /// Create new events in the given project.
    /// </summary>
    /// <param name="items">The assets to create.</param>
    /// <returns>List of created assets.</returns>
    let create (items: ItemsWithoutCursor<EventWriteDto>) : HttpHandler<HttpResponseMessage, ItemsWithoutCursor<EventReadDto>, 'a> =
        create items Url

    /// <summary>
    /// Delete multiple events in the same project, along with all their descendants in the asset hierarchy if recursive is true.
    /// </summary>
    /// <param name="items">The list of events to delete.</param>
    /// <param name="recursive">If true, delete all children recursively.</param>
    /// <returns>Empty result.</returns>
    let delete (items: EventDeleteDto) : HttpHandler<HttpResponseMessage, EmptyResult, 'a> =
        delete items Url

    /// <summary>
    /// Retrieves information about multiple events in the same project. A maximum of 1000 event IDs may be listed per
    /// request and all of them must be unique.
    /// </summary>
    /// <param name="ids">The ids of the assets to get.</param>
    /// <returns>Events with given ids.</returns>
    let retrieve (ids: Identity seq) : HttpHandler<HttpResponseMessage, ItemsWithoutCursor<EventReadDto>, 'a> =
        retrieve ids Url

    /// <summary>
    /// Retrieves a list of events matching the given criteria. This operation does not support pagination.
    /// </summary>
    /// <param name="query">Event search query.</param>
    /// <returns>List of events matching given criteria.</returns>
    let search (query: SearchQueryDto<EventFilterDto>) : HttpHandler<HttpResponseMessage, ItemsWithoutCursor<EventReadDto>, 'a> =
        search query Url

    /// <summary>
    /// Update one or more events. Supports partial updates, meaning that fields omitted from the requests are not changed
    /// </summary>
    /// <param name="query">The list of assets to update.</param>
    /// <returns>List of updated events.</returns>
    let update (query: ItemsWithoutCursor<UpdateItem<EventUpdateDto>>) : HttpHandler<HttpResponseMessage, ItemsWithoutCursor<EventReadDto>, 'a>  =
        update query Url

