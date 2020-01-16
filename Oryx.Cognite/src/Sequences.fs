// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System.Net.Http

open Oryx.Cognite

open CogniteSdk
open CogniteSdk.Sequences

/// Various event HTTP handlers.

[<RequireQualifiedAccess>]
module Sequences =
    [<Literal>]
    let Url = "/sequences"

    /// <summary>
    /// Retrieves list of sequences matching filter, and a cursor if given limit is exceeded
    /// </summary>
    /// <param name="query">The query to use.</param>
    /// <returns>List of sequences matching given filters and optional cursor</returns>
    let list (query: SequenceQueryDto) : HttpHandler<HttpResponseMessage, ItemsWithCursor<SequenceReadDto>, 'a> =
        list query Url

    /// <summary>
    /// Create new sequences in the given project.
    /// </summary>
    /// <param name="items">The events to create.</param>
    /// <returns>List of created sequences.</returns>
    let create (items: ItemsWithoutCursor<SequenceWriteDto>) : HttpHandler<HttpResponseMessage, ItemsWithoutCursor<SequenceReadDto>, 'a> =
        create items Url

    /// <summary>
    /// Delete multiple sequences in the same project,
    /// </summary>
    /// <param name="items">The list of events to delete.</param>
    /// <returns>Empty result.</returns>
    let delete (items: ItemsWithoutCursor<Identity>) : HttpHandler<HttpResponseMessage, EmptyResponse, 'a> =
        delete items Url

    /// <summary>
    /// Retrieves information about multiple sequences in the same project. A maximum of 1000 event IDs may be listed per
    /// request and all of them must be unique.
    /// </summary>
    /// <param name="ids">The ids of the sequences to get.</param>
    /// <returns>Sequences with given ids.</returns>
    let retrieve (ids: Identity seq) : HttpHandler<HttpResponseMessage, ItemsWithoutCursor<SequenceReadDto>, 'a> =
        retrieve ids Url

    /// <summary>
    /// Retrieves a list of sequences matching the given criteria. This operation does not support pagination.
    /// </summary>
    /// <param name="query">Sequence search query.</param>
    /// <returns>List of sequences matching given criteria.</returns>
    let search (query: SearchQueryDto<SequenceFilterDto, SearchDto>) : HttpHandler<HttpResponseMessage, ItemsWithoutCursor<SequenceReadDto>, 'a> =
        search query Url

    /// <summary>
    /// Update one or more sequences. Supports partial updates, meaning that fields omitted from the requests are not changed
    /// </summary>
    /// <param name="query">The list of sequences to update.</param>
    /// <returns>List of updated sequences.</returns>
    let update (query: ItemsWithoutCursor<UpdateItem<SequenceUpdateDto>>) : HttpHandler<HttpResponseMessage, ItemsWithoutCursor<SequenceReadDto>, 'a>  =
        update query Url

