// Copyright 2019 Cognite AS
// SPDX-License-Identifier: Apache-2.0

namespace Oryx.Cognite

open System.Collections.Generic
open System.Net.Http

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
    let list (query: SequenceQuery) : HttpHandler<HttpResponseMessage, ItemsWithCursor<Sequence>, 'a> =
        logWithMessage "Sequences:get"
        >=> list query Url

    /// Processes data requests, and returns the result. NB - This operation uses a dynamic limit on the number of rows returned based on the number and type of columns, use the provided cursor to paginate and retrieve all data.
    let listRows (query: SequenceRowQuery) : HttpHandler<HttpResponseMessage, SequenceData, 'a> =
        logWithMessage "Sequences:listRows"
        >=> Handler.list query RowsUrl

    /// Create new sequences in the given project. Returns list of created sequences.
    let create (items: SequenceCreate seq) : HttpHandler<HttpResponseMessage, SequenceData seq, 'a> =
        logWithMessage "Sequences:create"
        >=> create items Url

    let createRows(items: SequenceDataCreate seq) : HttpHandler<HttpResponseMessage, EmptyResponse, 'a> =
        logWithMessage "Sequences:createRows"
        >=> Handler.createEmpty items RowsUrl

    /// Delete multiple sequences in the same project. Returns empty result.
    let delete (items: Identity seq) : HttpHandler<HttpResponseMessage, EmptyResponse, 'a> =
        let req = ItemsWithoutCursor(Items=items)
        logWithMessage "Sequences:delete"
        >=> delete req Url

    let deleteRows (items: SequenceRowDelete seq) : HttpHandler<HttpResponseMessage, EmptyResponse, 'a> =
        let req = ItemsWithoutCursor(Items=items)
        logWithMessage "Sequences:deleteRows"
        >=> Handler.delete req RowsUrl

    /// Retrieves information about multiple sequences in the same project. A maximum of 1000 event IDs may be listed per
    /// request and all of them must be unique. Returns sequences with given ids.
    let retrieve (ids: Identity seq) : HttpHandler<HttpResponseMessage, Sequence seq, 'a> =
        logWithMessage "Sequences:retrieve"
        >=> retrieve ids Url

    /// Retrieves a list of sequences matching the given criteria. This operation does not support pagination. Returns list of sequences matching given criteria.
    let search (query: SequenceSearch) : HttpHandler<HttpResponseMessage, Sequence seq, 'a> =
        logWithMessage "Sequences:search"
        >=> search query Url

    /// Update one or more sequences. Supports partial updates, meaning that fields omitted from the requests are not changed. Returns list of updated sequences.</returns>
    let update (query: IEnumerable<UpdateItem<SequenceUpdate>>) : HttpHandler<HttpResponseMessage, Sequence seq, 'a>  =
        logWithMessage "Sequences:update"
        >=> update query Url

