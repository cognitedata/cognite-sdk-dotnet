// Copyright 2020 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System
open System.Collections.Generic

open Oryx
open Oryx.Cognite

open CogniteSdk

/// Various event HTTP handlers.

[<RequireQualifiedAccess>]
module Sequences =
    [<Literal>]
    let Url = "/sequences"

    let RowsUrl = Url +/ "data"

    /// Retrieves list of sequences matching filter, and a cursor if given limit is exceeded.
    let list (query: SequenceQuery) (source: HttpHandler<unit>) : HttpHandler<ItemsWithCursor<Sequence>> =
        source
        |> withLogMessage "Sequences:get"
        |> list query Url

    /// Processes data requests, and returns the result. NB - This operation uses a dynamic limit on the number of rows returned based on the number and type of columns, use the provided cursor to paginate and retrieve all data.
    let listRows (query: SequenceRowQuery) (source: HttpHandler<unit>) : HttpHandler<SequenceData> =
        source
        |> withLogMessage "Sequences:listRows"
        |> HttpHandler.list query RowsUrl

    /// Retrieves number of sequences matching filter.
    let aggregate (query: SequenceQuery) (source: HttpHandler<unit>) : HttpHandler<int32> =
        source
        |> withLogMessage "Sequences:aggregate"
        |> aggregate query Url

    /// Create new sequences in the given project. Returns list of created sequences.
    let create (items: SequenceCreate seq) (source: HttpHandler<unit>) : HttpHandler<Sequence seq> =
        source
        |> withLogMessage "Sequences:create"
        |> create items Url

    let createRows (items: SequenceDataCreate seq) (source: HttpHandler<unit>) : HttpHandler<EmptyResponse> =
        source
        |> withLogMessage "Sequences:createRows"
        |> HttpHandler.createEmpty items RowsUrl

    /// Delete multiple sequences in the same project. Returns empty result.
    let delete (items: Identity seq) (source: HttpHandler<unit>) : HttpHandler<EmptyResponse> =
        let req = ItemsWithoutCursor(Items = items)

        source
        |> withLogMessage "Sequences:delete"
        |> delete req Url

    let deleteRows (items: SequenceRowDelete seq) (source: HttpHandler<unit>) : HttpHandler<EmptyResponse> =
        let req = ItemsWithoutCursor(Items = items)

        source
        |> withLogMessage "Sequences:deleteRows"
        |> HttpHandler.delete req RowsUrl

    /// Retrieves information about multiple sequences in the same project. A maximum of 1000 event IDs may be listed per
    /// request and all of them must be unique. Returns sequences with given ids.
    let retrieve
        (ids: Identity seq)
        (ignoreUnknownIds: Nullable<bool>)
        (source: HttpHandler<unit>)
        : HttpHandler<Sequence seq> =
        source
        |> withLogMessage "Sequences:retrieve"
        |> retrieveIgnoreUnknownIds ids (Option.ofNullable ignoreUnknownIds) Url

    /// Retrieves a list of sequences matching the given criteria. This operation does not support pagination. Returns list of sequences matching given criteria.
    let search (query: SequenceSearch) (source: HttpHandler<unit>) : HttpHandler<Sequence seq> =
        source
        |> withLogMessage "Sequences:search"
        |> search query Url

    /// Update one or more sequences. Supports partial updates, meaning that fields omitted from the requests are not changed. Returns list of updated sequences.</returns>
    let update
        (query: IEnumerable<UpdateItem<SequenceUpdate>>)
        (source: HttpHandler<unit>)
        : HttpHandler<Sequence seq> =
        source
        |> withLogMessage "Sequences:update"
        |> update query Url
